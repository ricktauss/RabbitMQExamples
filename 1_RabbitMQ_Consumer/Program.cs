using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Numerics;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    IDictionary<string, object> arguments = new Dictionary<string, object>();   
    arguments.Add("x-message-ttl", 120000);
    //arguments.Add("x-single-active-consumer", true);

    Console.Write("Enter queue name: ");
    string? queueName = Console.ReadLine();

    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments);
    string consumerTag;

    do
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received: {0}",message);
        };
        consumerTag = channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine("->Press [enter] to exit.");

    } while (Console.ReadKey().Key != ConsoleKey.Enter);

    channel.BasicCancel(consumerTag);

}
