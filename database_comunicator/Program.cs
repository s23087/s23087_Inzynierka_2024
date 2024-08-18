using database_comunicator.dbContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HandlerContext>((ServiceProvider, options) =>
{
    var httpContextAccessor = ServiceProvider.GetRequiredService<IHttpContextAccessor>();
    var dbName = httpContextAccessor.HttpContext.Request.Path.Value.Split('/')[1];
    var connection_string = builder.Configuration["ConnectionStrings:flexible"].Replace("db_name", dbName);
    options.UseSqlServer(connection_string);
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
