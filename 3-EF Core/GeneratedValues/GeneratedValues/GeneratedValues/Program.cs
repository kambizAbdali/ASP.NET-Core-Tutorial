using GeneratedValues.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


// Part 1: EF Core Console App - (As before)  
var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
optionsBuilder.UseSqlServer(@"Server =.; Database = TestDB; Integrated Security = true");


using (var context = new ApplicationContext(optionsBuilder.Options))
{
    // Ensure the database is created  
    context.Database.EnsureCreated();
}



    app.MapGet("/", () => "Hello World!");

app.Run();
