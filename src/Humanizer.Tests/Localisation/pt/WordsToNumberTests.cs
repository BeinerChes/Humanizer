namespace pt;

[UseCulture("pt")]
public class WordsToNumberTests
{
    [Theory]
    [InlineData("zero", 0)]
    [InlineData("um", 1)]
    [InlineData("dois", 2)]
    [InlineData("três", 3)]
    [InlineData("quatro", 4)]
    [InlineData("cinco", 5)]
    [InlineData("seis", 6)]
    [InlineData("sete", 7)]
    [InlineData("oito", 8)]
    [InlineData("nove", 9)]
    [InlineData("dez", 10)]
    [InlineData("onze", 11)]
    [InlineData("doze", 12)]
    [InlineData("treze", 13)]
    [InlineData("quatorze", 14)]
    [InlineData("quinze", 15)]
    [InlineData("dezasseis", 16)]
    [InlineData("dezassete", 17)]
    [InlineData("dezoito", 18)]
    [InlineData("dezanove", 19)]
    [InlineData("vinte", 20)]
    [InlineData("trinta", 30)]
    [InlineData("quarenta", 40)]
    [InlineData("cinquenta", 50)]
    [InlineData("sessenta", 60)]
    [InlineData("setenta", 70)]
    [InlineData("oitenta", 80)]
    [InlineData("noventa", 90)]
    [InlineData("cem", 100)]
    [InlineData("duzentos", 200)]
    [InlineData("trezentos", 300)]
    [InlineData("quatrocentos", 400)]
    [InlineData("quinhentos", 500)]
    [InlineData("seiscentos", 600)]
    [InlineData("setecentos", 700)]
    [InlineData("oitocentos", 800)]
    [InlineData("novecentos", 900)]
    [InlineData("mil", 1000)]
    [InlineData("vinte e um", 21)]
    [InlineData("trinta e sete", 37)]
    [InlineData("cinquenta e um", 51)]
    [InlineData("sessenta e seis", 66)]
    [InlineData("cento e onze", 111)]
    [InlineData("cento e vinte e dois", 122)]
    [InlineData("cento e vinte e três", 123)]
    [InlineData("duzentos e onze", 211)]
    [InlineData("duzentos e vinte e um", 221)]
    [InlineData("seiscentos e trinta e sete", 637)]
    [InlineData("mil cento e onze", 1111)]
    [InlineData("mil duzentos e trinta e quatro", 1234)]
    [InlineData("mil seiscentos e trinta e sete", 1637)]
    [InlineData("mil novecentos e noventa e nove", 1999)]
    [InlineData("dois mil", 2000)]
    [InlineData("dois mil e quatorze", 2014)]
    [InlineData("dois mil e quarenta e oito", 2048)]
    [InlineData("três mil quinhentos e um", 3501)]
    [InlineData("oito mil e cem", 8100)]
    [InlineData("dez mil", 10000)]
    [InlineData("doze mil trezentos e quarenta e cinco", 12345)]
    [InlineData("sessenta e um mil seiscentos e trinta e sete", 61637)]
    [InlineData("cem mil", 100000)]
    [InlineData("cento e vinte e três mil quatrocentos e cinquenta e seis", 123456)]
    [InlineData("um milhão", 1000000)]
    [InlineData("um milhão duzentos e trinta e quatro mil quinhentos e sessenta e sete", 1234567)]
    [InlineData("dois milhões", 2000000)]
    [InlineData("dez milhões", 10000000)]
    [InlineData("cem milhões", 100000000)]
    [InlineData("menos cinco", -5)]
    [InlineData("menos vinte e um", -21)]
    [InlineData("menos cento e onze", -111)]
    public void ToNumber(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));

    [Theory]
    [InlineData("zero", 0, null)]
    [InlineData("um", 1, null)]
    [InlineData("vinte e um", 21, null)]
    [InlineData("cento e vinte e três", 123, null)]
    [InlineData("mil duzentos e trinta e quatro", 1234, null)]
    [InlineData("menos cinco", -5, null)]
    public void TryToNumber_ValidInput(string words, int expectedNumber, string? expectedUnrecognizedWord)
    {
        Assert.True(words.TryToNumber(out var parsedNumber, CultureInfo.CurrentCulture, out var unrecognizedWord));
        Assert.Equal(unrecognizedWord, expectedUnrecognizedWord);
        Assert.Equal(expectedNumber, parsedNumber);
    }

    [Theory]
    [InlineData("vinte nove olá", 0, "olá")]
    [InlineData("senhor três", 0, "senhor")]
    [InlineData("dezz", 0, "dezz")]
    [InlineData("invalidinput", 0, "invalidinput")]
    public void TryToNumber_InvalidInput(string words, int expectedNumber, string? expectedUnrecognizedWord)
    {
        Assert.False(words.TryToNumber(out var parsedNumber, CultureInfo.CurrentCulture, out var unrecognizedWord));
        Assert.Equal(unrecognizedWord, expectedUnrecognizedWord);
        Assert.Equal(expectedNumber, parsedNumber);
    }

    [Theory]
    [InlineData("primeiro", 1)]
    [InlineData("segundo", 2)]
    [InlineData("terceiro", 3)]
    [InlineData("quarto", 4)]
    [InlineData("quinto", 5)]
    [InlineData("sexto", 6)]
    [InlineData("sétimo", 7)]
    [InlineData("oitavo", 8)]
    [InlineData("nono", 9)]
    [InlineData("décimo", 10)]
    [InlineData("vigésimo", 20)]
    [InlineData("trigésimo", 30)]
    [InlineData("centésimo", 100)]
    [InlineData("milésimo", 1000)]
    public void ToNumber_Ordinals(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));

    [Theory]
    [InlineData("primeira", 1)]
    [InlineData("segunda", 2)]
    [InlineData("terceira", 3)]
    [InlineData("décima", 10)]
    [InlineData("vigésima", 20)]
    [InlineData("centésima", 100)]
    public void ToNumber_FeminineOrdinals(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));

    [Theory]
    [InlineData("uma", 1)]
    [InlineData("duas", 2)]
    [InlineData("duzentas", 200)]
    [InlineData("trezentas", 300)]
    public void ToNumber_FeminineCardinals(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));
}
