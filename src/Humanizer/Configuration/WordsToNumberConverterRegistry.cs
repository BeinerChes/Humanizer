namespace Humanizer;

internal class WordsToNumberConverterRegistry : LocaliserRegistry<IWordsToNumberConverter>
{
    public WordsToNumberConverterRegistry()
        : base(culture => culture.TwoLetterISOLanguageName switch
        {
            "en" => new EnglishWordsToNumberConverter(),
            "es" => new SpanishWordsToNumberConverter(),
            "pt" => new PortugueseWordsToNumberConverter(),
            _ => new DefaultWordsToNumberConverter(culture)
        })
    {
        Register("en", _ => new EnglishWordsToNumberConverter());
        Register("es", _ => new SpanishWordsToNumberConverter());
        Register("pt", _ => new PortugueseWordsToNumberConverter());
    }
}
