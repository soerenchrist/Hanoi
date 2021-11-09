namespace Hanoi.Services.Abstractions
{
    public interface ISettingsService
    {
        bool IsPro { get; set; }
        string ProReceipt { get; set; }
        bool ShowNumbers { get; set; }
        bool HasReviewed { get; set; }
    }
}
