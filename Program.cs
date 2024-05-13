using System.Reflection.Metadata.Ecma335;
using ebooks_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Web;
using System.Xml.Schema;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

//var ebooks = app.MapGroup("api/ebook");


// TODO: Add more methods
app.MapPost("/ebook", async (NewEbook newebook , DataContext  db) => {
    var Createebook = new EBook{
        Title = newebook.Title,
        Author = newebook.Author,
        Genre = newebook.Genre,
        Format = newebook.Format,
        Price = newebook.Price,
        IsAvailable = true,
        Stock = 0
        };
    db.EBooks.Add(Createebook);
    await db.SaveChangesAsync();
    return Results.Created($"/ebook/{Createebook.Id}", newebook);
});

app.MapGet("/ebook/get", async (string genre, string author, string format, DataContext db) => {
    if (genre is not null){
        await db.EBooks.Where(c => c.Genre == genre).ToListAsync();
    }
    if (author is not null){
        await db.EBooks.Where(c => c.Author == author).ToListAsync();
    }
    if (format is not null){
        await db.EBooks.Where(c => c.Format == format).ToListAsync();
    }
    await db.EBooks.ToListAsync();
});

app.MapPut("/ebook/{id}", async (int id,EBook inputEbook ,DataContext db) => {
    var ebook = await db.EBooks.FindAsync(id);
    if (ebook is null){
        return Results.NotFound();
    }
    if (ebook.Title != inputEbook.Title){
        ebook.Title = inputEbook.Title;
    }
    if (ebook.Author != inputEbook.Author){
        ebook.Author = inputEbook.Author;
    }
    if (ebook.Genre != inputEbook.Genre){
        ebook.Genre = inputEbook.Genre;
    }
    if (ebook.Format != inputEbook.Format){
        ebook.Format = inputEbook.Format;
    }
    if (0 < inputEbook.Price){
        ebook.Price = inputEbook.Price;
    }
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/ebook/{id}/change-availability", async (int id, DataContext db) => {
    var ebook = await db.EBooks.FindAsync(id);
    if (ebook is null){
        return Results.NotFound();
    }
    if (ebook.IsAvailable == true){
        ebook.IsAvailable = false;
        await db.SaveChangesAsync();
    }
    if (ebook.IsAvailable == false){
        ebook.IsAvailable = true;
        await db.SaveChangesAsync();
    }
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/ebook/{id}/increment-stock", async (int id, StockDTO stockdto, DataContext db) => {
    var ebook = await db.EBooks.FindAsync(id);
    if (ebook is null){
        return Results.NotFound();
    }
    ebook.Stock += stockdto.Stock;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("/ebook/purchase", async (PurchaseDTO purchasedto, DataContext db) => {
    var ebook = await db.EBooks.FindAsync(purchasedto.Id);
    if (ebook is null){
        return Results.NotFound();
    }
    if (ebook.IsAvailable == false){
        return TypedResults.BadRequest("No disponible");
    }
    if (ebook.Stock < purchasedto.Quantity){
        return TypedResults.BadRequest("No hay suficiente stock");
    }
    int total = purchasedto.Quantity * ebook.Price;
    if (total > purchasedto.Amount){
        return TypedResults.BadRequest("Muy pobre");
    }
    ebook.Stock -= purchasedto.Quantity;
    await db.SaveChangesAsync();
    return TypedResults.Ok("Compra realizada");
});

app.MapGet("/ebook", async (DataContext db) =>
 await db.EBooks.ToListAsync());

app.MapDelete("/ebook/{id}", async (int id , DataContext db ) => {
    if (await db.EBooks.FindAsync(id) is EBook ebook){
        db.EBooks.Remove(ebook);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();
