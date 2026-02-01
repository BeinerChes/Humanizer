namespace Humanizer;

internal class SpanishWordsToNumberConverter : GenderlessWordsToNumberConverter
{
    private static readonly FrozenDictionary<string, int> NumbersMap = new Dictionary<string, int>
    {
        {"cero", 0}, {"uno", 1}, {"un", 1}, {"una", 1}, {"dos", 2}, {"tres", 3},
        {"cuatro", 4}, {"cinco", 5}, {"seis", 6}, {"siete", 7}, {"ocho", 8}, {"nueve", 9},
        {"diez", 10}, {"once", 11}, {"doce", 12}, {"trece", 13}, {"catorce", 14},
        {"quince", 15}, {"dieciséis", 16}, {"dieciseis", 16}, {"diecisiete", 17},
        {"dieciocho", 18}, {"diecinueve", 19},
        {"veinte", 20}, {"veintiuno", 21}, {"veintiún", 21}, {"veintiun", 21}, {"veintiuna", 21},
        {"veintidós", 22}, {"veintidos", 22}, {"veintitrés", 23}, {"veintitres", 23},
        {"veinticuatro", 24}, {"veinticinco", 25}, {"veintiséis", 26}, {"veintiseis", 26},
        {"veintisiete", 27}, {"veintiocho", 28}, {"veintinueve", 29},
        {"treinta", 30}, {"cuarenta", 40}, {"cincuenta", 50}, {"sesenta", 60},
        {"setenta", 70}, {"ochenta", 80}, {"noventa", 90},
        {"cien", 100}, {"ciento", 100},
        {"doscientos", 200}, {"doscientas", 200}, {"trescientos", 300}, {"trescientas", 300},
        {"cuatrocientos", 400}, {"cuatrocientas", 400}, {"quinientos", 500}, {"quinientas", 500},
        {"seiscientos", 600}, {"seiscientas", 600}, {"setecientos", 700}, {"setecientas", 700},
        {"ochocientos", 800}, {"ochocientas", 800}, {"novecientos", 900}, {"novecientas", 900},
        {"mil", 1000},
        {"millón", 1_000_000}, {"millon", 1_000_000}, {"millones", 1_000_000}
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, int> OrdinalsMap = new Dictionary<string, int>
    {
        {"primero", 1}, {"primer", 1}, {"primera", 1},
        {"segundo", 2}, {"segunda", 2},
        {"tercero", 3}, {"tercer", 3}, {"tercera", 3},
        {"cuarto", 4}, {"cuarta", 4},
        {"quinto", 5}, {"quinta", 5},
        {"sexto", 6}, {"sexta", 6},
        {"séptimo", 7}, {"septimo", 7}, {"séptima", 7}, {"septima", 7},
        {"octavo", 8}, {"octava", 8},
        {"noveno", 9}, {"novena", 9},
        {"décimo", 10}, {"decimo", 10}, {"décima", 10}, {"decima", 10},
        {"vigésimo", 20}, {"vigesimo", 20}, {"vigésima", 20}, {"vigesima", 20},
        {"trigésimo", 30}, {"trigesimo", 30}, {"trigésima", 30}, {"trigesima", 30},
        {"cuadragésimo", 40}, {"cuadragesimo", 40}, {"cuadragésima", 40}, {"cuadragesima", 40},
        {"quincuagésimo", 50}, {"quincuagesimo", 50}, {"quincuagésima", 50}, {"quincuagesima", 50},
        {"sexagésimo", 60}, {"sexagesimo", 60}, {"sexagésima", 60}, {"sexagesima", 60},
        {"septuagésimo", 70}, {"septuagesimo", 70}, {"septuagésima", 70}, {"septuagesima", 70},
        {"octogésimo", 80}, {"octogesimo", 80}, {"octogésima", 80}, {"octogesima", 80},
        {"nonagésimo", 90}, {"nonagesimo", 90}, {"nonagésima", 90}, {"nonagesima", 90},
        {"centésimo", 100}, {"centesimo", 100}, {"centésima", 100}, {"centesima", 100},
        {"milésimo", 1000}, {"milesimo", 1000}, {"milésima", 1000}, {"milesima", 1000}
    }.ToFrozenDictionary();

    private static readonly HashSet<string> Connectors = ["y"];

    public override int Convert(string words)
    {
        if (!TryConvert(words, out var result, out var unrecognizedWord))
            throw new ArgumentException($"Unrecognized number word: {unrecognizedWord}");

        return result;
    }

    public override bool TryConvert(string words, out int result) => TryConvert(words, out result, out _);

    public override bool TryConvert(string words, out int parsedValue, out string? unrecognizedWord)
    {
        if (string.IsNullOrWhiteSpace(words))
            throw new ArgumentException("Input words cannot be empty.");

        unrecognizedWord = null;
        words = words.ToLowerInvariant().Trim();

        var isNegative = words.StartsWith("menos ");
        if (isNegative)
            words = words.Substring(6).Trim();

        words = words.Replace("-", " ");

        if (int.TryParse(words, out var numericValue))
        {
            parsedValue = isNegative ? -numericValue : numericValue;
            return true;
        }

        if (OrdinalsMap.TryGetValue(words, out var ordinalValue))
        {
            parsedValue = isNegative ? -ordinalValue : ordinalValue;
            return true;
        }

        if (TryConvertWordsToNumber(words, out var numberValue, out var unrecognizedNumberWord))
        {
            parsedValue = isNegative ? -numberValue : numberValue;
            return true;
        }

        unrecognizedWord = unrecognizedNumberWord;
        parsedValue = default;
        return false;
    }

    private static bool TryConvertWordsToNumber(string words, out int result, out string? unrecognizedWord)
    {
        var wordsArray = words.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        result = 0;
        unrecognizedWord = null;
        var current = 0;
        var hasOrdinal = false;

        foreach (var word in wordsArray)
        {
            if (Connectors.Contains(word))
                continue;

            if (OrdinalsMap.TryGetValue(word, out var ordinalValue))
            {
                result += current + ordinalValue;
                hasOrdinal = true;
                break;
            }

            if (!NumbersMap.TryGetValue(word, out var value))
            {
                unrecognizedWord = word;
                return false;
            }

            if (value == 100)
                current = (current == 0 ? 1 : current) * 100;
            else if (value >= 200 && value <= 900)
                current += value;
            else if (value == 1000)
            {
                current = (current == 0 ? 1 : current) * 1000;
                result += current;
                current = 0;
            }
            else if (value >= 1_000_000)
            {
                current = current == 0 ? 1 : current;
                result += current * value;
                current = 0;
            }
            else
                current += value;
        }

        if (!hasOrdinal)
            result += current;

        return true;
    }
}
