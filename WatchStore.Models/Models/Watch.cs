namespace WatchStore.Core.Models;

public class Watch
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ProductionYear { get; set; }
    public decimal CaseDiameterMm { get; set; }
    public Guid? OwnerId { get; set; }
}