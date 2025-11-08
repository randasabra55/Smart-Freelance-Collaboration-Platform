namespace Smart_Freelance_Core.Behaviors
{
    public interface IAuditableRequest
    {
        string GetAuditAction(object? response, Exception? exception);
        string GetEntityName();
        long? GetEntityId();
    }
}
