namespace ebooks_dotnet7_api;
using System.ComponentModel.DataAnnotations;

public class NewEbook{
    [StringLength(256, MinimumLength = 3)]
    public required string Title { get; set; }

    [StringLength(256, MinimumLength = 3)]
    public required string Author { get; set; }

    [StringLength(256, MinimumLength = 3)]
    public required string Genre { get; set; }

    [StringLength(256, MinimumLength = 3)]
    public required string Format { get; set; }

    [Range(1, int.MaxValue)]
    public required int Price { get; set; }
}