namespace Liewald.TruckService.Domain.Models;

public sealed class Driver
{
    public string Id { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Location Location { get; set; } = null!;
}
