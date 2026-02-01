using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SecureAccess.Api.Repositories;

namespace SecureAccess.Api.Services;

public sealed class AuditConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AuditConsumer(IServiceScopeFactory scopeFactory, ILogger<AuditConsumer> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
    }
    private readonly ILogger<AuditConsumer> _log;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // IMPORTANT: match your publisher (durable should be true if you set it true there)
        await channel.QueueDeclareAsync(
            queue: "audit.userlogins",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var payload = Encoding.UTF8.GetString(ea.Body.ToArray());

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

                var evt = System.Text.Json.JsonSerializer.Deserialize<SecureAccess.Api.Contracts.UserLoggedInEvent>(payload);
                var occurredAt = evt?.OccurredAtUtc ?? DateTime.UtcNow;

                repo.Add("UserLoggedIn", payload, occurredAt);


                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                _log.LogInformation("Audit saved + acked. tag={Tag}", ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Audit consume failed. tag={Tag}", ea.DeliveryTag);

                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false, cancellationToken: stoppingToken);
            }
        };


        await channel.BasicConsumeAsync(
            queue: "audit.userlogins",
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        // keep the background service alive until shutdown
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
