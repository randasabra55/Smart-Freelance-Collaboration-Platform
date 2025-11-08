using MediatR;
using Microsoft.AspNetCore.Http;
using Smart_Freelance_Core.Behaviors;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Infrastructure.Data;

public class AuditLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly Context _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLoggingBehavior(Context context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        string action = typeof(TRequest).Name;
        string entityName = typeof(TRequest).DeclaringType?.Name ?? "Unknown";
        long? entityId = null;

        try
        {
            var response = await next();

            if (request is IAuditableRequest auditable)
            {
                action = auditable.GetAuditAction(response, null);
                entityName = auditable.GetEntityName();
                entityId = auditable.GetEntityId();
            }

            await _context.AuditLogs.AddAsync(new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                UserId = long.TryParse(userId, out var id) ? id : null,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            if (request is IAuditableRequest auditable)
            {
                action = auditable.GetAuditAction(null, ex);
                entityName = auditable.GetEntityName();
                entityId = auditable.GetEntityId();
            }

            await _context.AuditLogs.AddAsync(new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                UserId = long.TryParse(userId, out var id) ? id : null,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            throw;
        }
    }
}
