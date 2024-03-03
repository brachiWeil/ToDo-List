using Microsoft.OpenApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.36-mysql")), ServiceLifetime.Singleton);

builder.Services.AddCors(options =>
{
    
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
        options.RoutePrefix = string.Empty;
    });

app.MapGet("/todos", async(ToDoDbContext dbContext) => 
{
    var items =await dbContext.Items.ToListAsync();

    return items;
});
app.MapGet("/todo/{id}", async(int id,ToDoDbContext dbContext) =>
{
        var items =await dbContext.Items.ToListAsync();
        var i = items.Find(x => x.Id == id);
        return i;
});
app.MapPost("/todo", async(Item item, ToDoDbContext dbContext) =>
{
        await dbContext.Items.AddAsync(item);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/", item);
});
app.MapPut("/todos/{id}", async (int id, Item item, ToDoDbContext dbContext) =>
    {
        var i = await dbContext.Items.FindAsync(id);
        if (i == null)
            return Results.BadRequest("There is no such item!!!");

        i.Name = item.Name;
        i.IsComplete = item.IsComplete;

        await dbContext.SaveChangesAsync();
        return Results.Created($"/", i);
    }
);
app.MapDelete("/todo/{id}", async (int id, ToDoDbContext dbContext) =>
    {
        var i = await dbContext.Items.FindAsync(id);
        if (i != null)
        {
            dbContext.Items.Remove(i);
            await dbContext.SaveChangesAsync();
        }
    }
);

app.UseCors();
app.Run();