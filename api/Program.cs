using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // adds UI at /scalar/v1
}

app.UseExceptionHandler(exceptionHandlerApp =>
    exceptionHandlerApp.Run(async context =>
    {
        var ex = context
            .Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()
            ?.Error;
        context.Response.StatusCode = ex is ArgumentException ? 400 : 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync(ex?.Message ?? "An error occurred.");
    })
);

app.MapPost("/api/hands/eval", Evaluate.EvaluateHand);

app.Run();
