namespace ebooks_dotnet7_api;
using System.ComponentModel.DataAnnotations;

public class PurchaseDTO{
    public required int Id {get; set;}
    public required int Quantity { get; set; }
    public required int Amount { get; set; }
}