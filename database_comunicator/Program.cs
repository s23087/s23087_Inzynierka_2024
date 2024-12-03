using database_communicator.Data;
using database_communicator.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromXml("nlog.config").GetCurrentClassLogger();
try
{
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
    builder.Services.AddScoped<ICreditNoteServices, CreditNoteServices>();
    builder.Services.AddScoped<IRequestServices, RequestServices>();
    builder.Services.AddScoped<IProformaServices, ProformaServices>();
    builder.Services.AddScoped<IDeliveryServices, DeliveryServices>();
    builder.Services.AddScoped<IOutsideItemServices, OutsideItemServices>();
    builder.Services.AddScoped<IOfferServices, OfferServices>();
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

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

}
catch (Exception ex)
{
    logger.Error(ex, "NLog setup error.");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}