using Inheritance.DAL;
using Inheritance.Models;
using Microsoft.EntityFrameworkCore;

// Create the builder  
var builder = WebApplication.CreateBuilder(args);

// Configure the DbContext with the connection string  
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(@"Server=.;Database=Inheritance;Integrated Security=true"));

// Build the app  
var app = builder.Build();

// Create scope for seeding data  
using (var scope = app.Services.CreateScope())
{
    // Ensure the database is created  
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Apply migrations and create database if not exist  
    //context.Database.Migrate();

    var peymentCounts= context.BankTransferPayments.Count();
    // Check if there are any data already (optional)  
    if (!context.Payments.Any())
    {
        // Seed sample data (adjust to your actual domain model)
        // If you have a derived CreditCardPayment and BankTransferPayment, you can seed them like this:

        // Sample CreditCardPayment
        var ccPayment = new CreditCardPayment
        {
            // Populate with available properties. Example placeholders:
            // PaymentId is auto-generated
            Amount = 100.00m,
            PaymentDate = DateTime.UtcNow,
            CardNumber = "4111111111111111",
            ExpiryDate = "12/29",
            Cvv = "123"
        };

        // Sample BankTransferPayment
        var bankTransfer = new BankTransferPayment
        {
            Amount = 250.00m,
            PaymentDate = DateTime.UtcNow,
            AccountNumber = "123456789",
            SortCode = "00-00-00"
        };

        // Add to context
        context.Payments.Add(ccPayment);
        context.Payments.Add(bankTransfer);
        context.SaveChanges();

        Console.WriteLine("Sample data added successfully");
    }
}

// Map route  
app.MapGet("/", () => "Hello World!");

// Run the app  
app.Run();