namespace Services;

public static class DeviceSettingsAddress
{
    public static Uri Parse(string? address)
    {
        var value = address?.Trim() ?? "";
        if (!value.Contains("://", StringComparison.Ordinal)) value = "http://" + value;
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || string.IsNullOrWhiteSpace(uri.Host) || Uri.CheckHostName(uri.Host) == UriHostNameType.Unknown
            || !string.IsNullOrEmpty(uri.UserInfo) || uri.AbsolutePath != "/" || uri.Query.Length > 0 || uri.Fragment.Length > 0)
            throw new ArgumentException("Địa chỉ IP/host của cân trống hoặc không hợp lệ. Kiểm tra lại địa chỉ cân.");
        return new Uri(uri, "/api/w/posts");
    }
}
