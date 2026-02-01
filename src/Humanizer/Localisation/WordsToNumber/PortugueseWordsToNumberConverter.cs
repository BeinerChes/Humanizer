namespace Humanizer;

internal partial class PortugueseWordsToNumberConverter : GenderlessWordsToNumberConverter
{
    private static readonly FrozenDictionary<string, int> NumbersMap = new Dictionary<string, int>
    {
        {"zero", 0}, {"um", 1}, {"uma", 1}, {"dois", 2}, {"duas", 2}, {"três", 3}, {"tres", 3},
        {"quatro", 4}, {"cinco", 5}, {"seis", 6}, {"sete", 7}, {"oito", 8}, {"nove", 9},
        {"dez", 10}, {"onze", 11}, {"doze", 12}, {"treze", 13}, {"quatorze", 14}, {"catorze", 14},
        {"quinze", 15}, {"dezasseis", 16}, {"dezesseis", 16}, {"dezassete", 17}, {"dezessete", 17},
        {"dezoito", 18}, {"dezanove", 19}, {"dezenove", 19},
        {"vinte", 20}, {"trinta", 30}, {"quarenta", 40}, {"cinquenta", 50}, {"cinqüenta", 50},
        {"sessenta", 60}, {"setenta", 70}, {"oitenta", 80}, {"noventa", 90},
        {"cem", 100}, {"cento", 100},
        {"duzentos", 200}, {"duzentas", 200}, {"trezentos", 300}, {"trezentas", 300},
        {"quatrocentos", 400}, {"quatrocentas", 400}, {"quinhentos", 500}, {"quinhentas", 500},
        {"seiscentos", 600}, {"seiscentas", 600}, {"setecentos", 700}, {"setecentas", 700},
        {"oitocentos", 800}, {"oitocentas", 800}, {"novecentos", 900}, {"novecentas", 900},
        {"mil", 1000}, {"milhão", 1_000_000}, {"milhao", 1_000_000}, {"milhões", 1_000_000}, {"milhoes", 1_000_000},
        {"bilhão", 1_000_000_000}, {"bilhao", 1_000_000_000}, {"bilhões", 1_000_000_000}, {"bilhoes", 1_000_000_000}
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, int> OrdinalsMap = new Dictionary<string, int>
    {
        {"primeiro", 1}, {"primeira", 1}, {"segundo", 2}, {"segunda", 2},
        {"terceiro", 3}, {"terceira", 3}, {"quarto", 4}, {"quarta", 4},
        {"quinto", 5}, {"quinta", 5}, {"sexto", 6}, {"sexta", 6},
        {"sétimo", 7}, {"setimo", 7}, {"sétima", 7}, {"setima", 7},
        {"oitavo", 8}, {"oitava", 8}, {"nono", 9}, {"nona", 9},
        {"décimo", 10}, {"decimo", 10}, {"décima", 10}, {"decima", 10},
        {"vigésimo", 20}, {"vigesimo", 20}, {"vigésima", 20}, {"vigesima", 20},
        {"trigésimo", 30}, {"trigesimo", 30}, {"trigésima", 30}, {"trigesima", 30},
        {"quadragésimo", 40}, {"quadragesimo", 40}, {"quadragésima", 40}, {"quadragesima", 40},
        {"quinquagésimo", 50}, {"quinquagesimo", 50}, {"quinquagésima", 50}, {"quinquagesima", 50},
        {"sexagésimo", 60}, {"sexagesimo", 60}, {"sexagésima", 60}, {"sexagesima", 60},
        {"septuagésimo", 70}, {"septuagesimo", 70}, {"septuagésima", 70}, {"septuagesima", 70},
        {"octogésimo", 80}, {"octogesimo", 80}, {"octogésima", 80}, {"octogesima", 80},
        {"nonagésimo", 90}, {"nonagesimo", 90}, {"nonagésima", 90}, {"nonagesima", 90},
        {"centésimo", 100}, {"centesimo", 100}, {"centésima", 100}, {"centesima", 100},
        {"milésimo", 1000}, {"milesimo", 1000}, {"milésima", 1000}, {"milesima", 1000}
    }.ToFrozenDictionary();

    private static readonly HashSet<string> Connectors = ["e"];

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
                result += (current == 0 ? 1 : current) * 1000;
                current = 0;
            }
            else if (value >= 1_000_000)
            {
                result += (current == 0 ? 1 : current) * value;
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
