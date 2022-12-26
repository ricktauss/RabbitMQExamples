using RabbitMQ.Client;
using System.Text;
using System.Threading;
using System.Timers;

System.Timers.Timer _timer;

Console.WriteLine("Publisher Application started at: {0}", DateTime.Now);

string? hostname = Environment.GetEnvironmentVariable("RMQ_HOSTNAME");
Console.WriteLine("Hostname used: " + hostname);

var factory = new ConnectionFactory() { HostName = hostname };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "myMessages", type: ExchangeType.Fanout);

    // Create a timer with a two second interval.
    _timer = new System.Timers.Timer(1000);

    string id = Guid.NewGuid().ToString();
    int i = 0;

    // Hook up the Elapsed event for the timer. 
    _timer.Elapsed += (sender, e) =>
    {
        i++;
        var message = String.Format("msg[{0}]: Hello World from {1}!", i, id);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "myMessages",
                         routingKey: "",
                         basicProperties: null,
                         body: body);
        Console.WriteLine(" [x] Sent: {0}", message);
    };

    _timer.AutoReset = true;
    _timer.Enabled = true;

    Console.WriteLine("Press [enter] to exit.\n");
    Console.ReadLine();
}

_timer.Stop();
_timer.Dispose();

Console.WriteLine("Press [enter] to exit.\n");
Console.ReadLine();

Console.WriteLine("Terminating the application...");


