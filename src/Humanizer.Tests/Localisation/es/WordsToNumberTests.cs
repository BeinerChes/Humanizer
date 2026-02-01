namespace es;

[UseCulture("es-ES")]
public class WordsToNumberTests
{
    [Theory]
    [InlineData("cero", 0)]
    [InlineData("uno", 1)]
    [InlineData("dos", 2)]
    [InlineData("tres", 3)]
    [InlineData("cuatro", 4)]
    [InlineData("cinco", 5)]
    [InlineData("seis", 6)]
    [InlineData("siete", 7)]
    [InlineData("ocho", 8)]
    [InlineData("nueve", 9)]
    [InlineData("diez", 10)]
    [InlineData("once", 11)]
    [InlineData("doce", 12)]
    [InlineData("trece", 13)]
    [InlineData("catorce", 14)]
    [InlineData("quince", 15)]
    [InlineData("dieciséis", 16)]
    [InlineData("diecisiete", 17)]
    [InlineData("dieciocho", 18)]
    [InlineData("diecinueve", 19)]
    [InlineData("veinte", 20)]
    [InlineData("veintiuno", 21)]
    [InlineData("veintidós", 22)]
    [InlineData("veintitrés", 23)]
    [InlineData("veinticuatro", 24)]
    [InlineData("veinticinco", 25)]
    [InlineData("veintiséis", 26)]
    [InlineData("veintisiete", 27)]
    [InlineData("veintiocho", 28)]
    [InlineData("veintinueve", 29)]
    [InlineData("treinta", 30)]
    [InlineData("treinta y cinco", 35)]
    [InlineData("cuarenta", 40)]
    [InlineData("cincuenta", 50)]
    [InlineData("sesenta", 60)]
    [InlineData("setenta", 70)]
    [InlineData("ochenta", 80)]
    [InlineData("noventa", 90)]
    [InlineData("cien", 100)]
    [InlineData("ciento once", 111)]
    [InlineData("ciento veintidós", 122)]
    [InlineData("doscientos", 200)]
    [InlineData("trescientos", 300)]
    [InlineData("cuatrocientos", 400)]
    [InlineData("quinientos", 500)]
    [InlineData("seiscientos", 600)]
    [InlineData("setecientos", 700)]
    [InlineData("ochocientos", 800)]
    [InlineData("novecientos", 900)]
    [InlineData("mil", 1000)]
    [InlineData("mil ciento once", 1111)]
    [InlineData("mil doscientos treinta y cuatro", 1234)]
    [InlineData("mil novecientos noventa y nueve", 1999)]
    [InlineData("dos mil", 2000)]
    [InlineData("dos mil catorce", 2014)]
    [InlineData("dos mil cuarenta y ocho", 2048)]
    [InlineData("tres mil quinientos uno", 3501)]
    [InlineData("diez mil", 10000)]
    [InlineData("doce mil trescientos cuarenta y cinco", 12345)]
    [InlineData("cien mil", 100000)]
    [InlineData("ciento veintitrés mil cuatrocientos cincuenta y seis", 123456)]
    [InlineData("un millón", 1000000)]
    [InlineData("un millón doscientos treinta y cuatro mil quinientos sesenta y siete", 1234567)]
    [InlineData("dos millones", 2000000)]
    [InlineData("diez millones", 10000000)]
    [InlineData("cien millones", 100000000)]
    [InlineData("menos cinco", -5)]
    [InlineData("menos quince", -15)]
    [InlineData("menos ciento veintitrés", -123)]
    public void ToNumber(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));

    [Theory]
    [InlineData("cero", 0, null)]
    [InlineData("uno", 1, null)]
    [InlineData("veintiuno", 21, null)]
    [InlineData("ciento veintitrés", 123, null)]
    [InlineData("mil doscientos treinta y cuatro", 1234, null)]
    [InlineData("menos cinco", -5, null)]
    public void TryToNumber_ValidInput(string words, int expectedNumber, string? expectedUnrecognizedWord)
    {
        Assert.True(words.TryToNumber(out var parsedNumber, CultureInfo.CurrentCulture, out var unrecognizedWord));
        Assert.Equal(unrecognizedWord, expectedUnrecognizedWord);
        Assert.Equal(expectedNumber, parsedNumber);
    }

    [Theory]
    [InlineData("veinte nueve hola", 0, "hola")]
    [InlineData("señor tres", 0, "señor")]
    [InlineData("diezz", 0, "diezz")]
    [InlineData("invalidinput", 0, "invalidinput")]
    public void TryToNumber_InvalidInput(string words, int expectedNumber, string? expectedUnrecognizedWord)
    {
        Assert.False(words.TryToNumber(out var parsedNumber, CultureInfo.CurrentCulture, out var unrecognizedWord));
        Assert.Equal(unrecognizedWord, expectedUnrecognizedWord);
        Assert.Equal(expectedNumber, parsedNumber);
    }

    [Theory]
    [InlineData("primero", 1)]
    [InlineData("segundo", 2)]
    [InlineData("tercero", 3)]
    [InlineData("cuarto", 4)]
    [InlineData("quinto", 5)]
    [InlineData("sexto", 6)]
    [InlineData("séptimo", 7)]
    [InlineData("octavo", 8)]
    [InlineData("noveno", 9)]
    [InlineData("décimo", 10)]
    [InlineData("vigésimo", 20)]
    [InlineData("trigésimo", 30)]
    [InlineData("centésimo", 100)]
    [InlineData("milésimo", 1000)]
    public void ToNumber_Ordinals(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));

    [Theory]
    [InlineData("primera", 1)]
    [InlineData("segunda", 2)]
    [InlineData("tercera", 3)]
    [InlineData("décima", 10)]
    [InlineData("vigésima", 20)]
    [InlineData("centésima", 100)]
    public void ToNumber_FeminineOrdinals(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));

    [Theory]
    [InlineData("una", 1)]
    [InlineData("veintiuna", 21)]
    [InlineData("doscientas", 200)]
    [InlineData("trescientas", 300)]
    public void ToNumber_FeminineCardinals(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));

    [Theory]
    [InlineData("primer", 1)]
    [InlineData("tercer", 3)]
    public void ToNumber_AbbreviatedOrdinals(string words, int expectedNumber) =>
        Assert.Equal(expectedNumber, words.ToNumber(CultureInfo.CurrentCulture));
}
