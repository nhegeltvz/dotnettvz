new int[] { 11, 22, 31 }.Aggregate((acc, n) =>
{
    var temp = n;
    var numOfDigits = 0;
    while (temp > 0)
    {
        var digit = n % 10;
        temp /= 10;
        numOfDigits += digit % 1 == 0 ? 1 : 0;
    }
    return acc + numOfDigits;
});