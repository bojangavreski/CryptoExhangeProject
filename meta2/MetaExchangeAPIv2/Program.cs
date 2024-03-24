var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/execute/order", async (OrderRequest request) =>
{
    if (request.Amount <= 0)
    {
        return Results.BadRequest("Amount must be greater than 0");
    }

    if (!new[] { "buy", "sell" }.Contains(request.OrderType))
    {
        return Results.BadRequest("Invalid order type");
    }

    var bestOrders = await MetaExchangeService.GetBestOrders(request.Amount, request.OrderType);

    return Results.Json(bestOrders);
})
.WithName("GetOptimalOrders")
.WithOpenApi();

app.Run();
