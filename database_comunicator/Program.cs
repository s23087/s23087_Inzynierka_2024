using database_comunicator.Data;
using database_comunicator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddUserSecrets<Program>(true);
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContext<HandlerContext>();
builder.Services.AddScoped<IRegistrationServices, RegistrationServices>();
builder.Services.AddScoped<IOrganizationServices, OrganizationServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IItemServices, ItemServices>();
builder.Services.AddScoped<ILogServices, LogServices>();
builder.Services.AddScoped<IRolesServices, RolesServices>();
builder.Services.AddScoped<IInvoiceServices, InvoiceServices>();
builder.Services.AddScoped<INotificationServices, NotificationServices>();

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
