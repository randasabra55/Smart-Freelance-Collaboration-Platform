using Microsoft.Extensions.DependencyInjection;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Infrastructure.Data;

namespace Smart_Freelance_Infrastructure.Services
{
    public class AuditLoggerService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        //private readonly Context _context;

        public AuditLoggerService(IServiceScopeFactory serviceScopeFactory)
        {
            _scopeFactory = serviceScopeFactory;
        }

        public async Task LogAsync(string action, string entityName, long? entityId = null, long? userId = null)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            var log = new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await context.AuditLogs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}
