using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    IDictionary<string, object> arguments = new Dictionary<string, object>();
    arguments.Add("x-message-ttl", 120000);
    //arguments.Add("x-single-active-consumer", true); //first consumer receives all messages

    Console.Write("Enter queue name: ");
    string? queueName = Console.ReadLine();

    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments);

    string id = Guid.NewGuid().ToString();
    int count = 0;

    do
    {
        string message = String.Format("Message - {0} - {1}", count, id);

        IBasicProperties props = channel.CreateBasicProperties();
        props.ContentType = "text/plain";
        props.Type = "test.created";
        props.Priority = 3;

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: queueName, props, body: body);
        count++;
        Console.WriteLine("Published: " + message, id);

        Console.WriteLine("->Press [enter] to exit.");

    } while (Console.ReadKey().Key != ConsoleKey.Enter);

}
