namespace SecureAccess.Api.Services
{
    public interface IRabbitPublisher
    {
        Task PublishAsync(string queue, string message);
    }
}
