namespace Services;

/// <summary>HQ stores net kg and tare separately as decimal(18,3). Never subtract tare twice.</summary>
public static class HqWeightValues
{
    private const decimal StorageLimit = 1000000000000000m;

    public static bool WithinStorageRange(decimal value) => value > -StorageLimit && value < StorageLimit;

    public static bool HasStoragePrecision(decimal value) => decimal.Round(value, 3) == value;

    public static bool IsValidTicket(decimal netWeight, decimal tare) =>
        netWeight > 0 && tare >= 0 && WithinStorageRange(netWeight) && WithinStorageRange(tare) &&
        HasStoragePrecision(netWeight) && HasStoragePrecision(tare);
}
