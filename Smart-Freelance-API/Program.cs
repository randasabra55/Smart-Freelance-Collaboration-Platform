
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Core;
using Smart_Freelance_Core.Behaviors;
using Smart_Freelance_Core.Middleware;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Infrastructure;
using Smart_Freelance_Infrastructure.Data;
using Smart_Freelance_Infrastructure.Notifications;
using Smart_Freelance_Infrastructure.Seeders;
using Smart_Freelance_Service;
using Smart_Freelance_Service.Consumer;
using Smart_Freelance_Service.Jobs;
using System.Reflection;
using X.Paymob.CashIn;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Connection To SQL Server
builder.Services.AddDbContext<Context>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
});

builder.Services.AddInfrastructureDependencies()
                .AddServiceDependencies()
                .AddCoreDependencies()
                .AddServiceRegisteration(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

#region settup of hangfire

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("cs")));
builder.Services.AddHangfireServer();

//regiser jobs

//هعملهم كومنت دلوقتى عشان اعمل بابلش ع سمارتر 
builder.Services.AddHostedService<ProjectCreationConsumerJob>();
builder.Services.AddHostedService<ProposalNotificationConsumerJob>();
builder.Services.AddHostedService<CollaborationRoomConsumer>();
builder.Services.AddHostedService<PaymentConsumer>();
builder.Services.AddHostedService<CompletionPaymentConsumer>();

///////////////////////////////////////////////////////////////////////
#endregion

builder.Services.AddHttpClient();

#region settup of paymob

builder.Services.AddPaymobCashIn(config =>
{
    config.ApiKey = builder.Configuration["Paymob:ApiKey"]!;
    config.Hmac = builder.Configuration["Paymob:Hmac"]!;
});
#endregion
builder.Services.AddTransient<DeliveryDeadlineReminderJob>();
builder.Services.AddTransient<CleanupPendingProjectsJob>();
#region allow cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endregion
var app = builder.Build();

//app.MapEndpointsWithMediatR(Assembly.GetExecutingAssembly());  //



//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//    await RoleSeeder.SeedAsync(roleManager);
//    await UserSeeder.SeedAsync(userManager);
//}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // 1. Apply all pending migrations
    var context = services.GetRequiredService<Context>();
    await context.Database.MigrateAsync();

    // 2. Seed Roles
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    await RoleSeeder.SeedAsync(roleManager);

    // 3. Seed Users
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await UserSeeder.SeedAsync(userManager);
}
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHangfireDashboard("/hangfire");

//register job not as hostedService but as hangfire job that happen in كل فترة
RecurringJob.AddOrUpdate<CleanupPendingProjectsJob>(
    "cleanup-pending-projects",
    job => job.RunAsync(),
    "0 0 */3 * *");//كل تلات ايام 

RecurringJob.AddOrUpdate<DeliveryDeadlineReminderJob>(
    "delivery-deadline-reminder",
    job => job.RunAsync(),
    Cron.Daily); // كل يوم

///////////////////////////////////////////////////////////////////////
//for reminder
//HostedServices = تشتغل طول الوقت(RabbitMQ Consumers)

//Hangfire Jobs = تتنفذ حسب الجدولة (كل يوم او كل 3 أيام)

///////////////////////////////////////////////////////////////////////////
app.UseCors("AllowAll");
app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});
//app.MapEndpoints();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<CollaborationHub>("/collaborationHub");


app.Run();
