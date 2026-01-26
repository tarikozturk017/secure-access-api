using System.Text;
using RabbitMQ.Client;

public sealed class RabbitPublisher
{
    private readonly string _host = "localhost";

    public async Task PublishAsync(string queue, string message)
    {
        var factory = new ConnectionFactory { HostName = _host };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(message);
        var props = new BasicProperties
        {
            ContentType = "text/plain",
            DeliveryMode = DeliveryModes.Persistent
        };


        // default exchange ("") routes by queue name
        await channel.BasicPublishAsync(exchange: "", routingKey: queue, mandatory: false, basicProperties: props, body: body);
    }
}
