namespace Services;

/// <summary>Optional capability; does not change IMayCansService or the firmware protocol.</summary>
public interface IHqFirmwareIdentitySender
{
    Task<bool> SendHqIdentityAsync(string machine, string connectionId, string employee,
        string employeeCode, string name, CancellationToken cancellation);
}
