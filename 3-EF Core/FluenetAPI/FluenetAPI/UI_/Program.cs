using DAL;

DatabaseContext context = new DatabaseContext();

context.Database.EnsureDeleted();
context.Database.EnsureCreated();


var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
