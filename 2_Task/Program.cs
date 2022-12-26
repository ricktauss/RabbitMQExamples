using RabbitMQ.Client;
using System.Text;

string[] cliArgs = Environment.GetCommandLineArgs();


var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var message = GetMessage(cliArgs);
    var body = Encoding.UTF8.GetBytes(message);

    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    channel.BasicPublish(exchange: "",
                         routingKey: "hello",
                         basicProperties: properties,
                         body: body);
    Console.WriteLine(" [x] Sent {0}", message);
}


static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args[1]) : "Hello World!");
}
