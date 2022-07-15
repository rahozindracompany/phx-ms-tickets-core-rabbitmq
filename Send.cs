using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;

namespace phx_ms_tickets_core_rabbitmq
{
    class Send
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = Protocols.DefaultProtocol.DefaultPort,
                UserName = "admin",
                Password = "admin",
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Simulate send to filesystem microservices
                channel.QueueDeclare(queue: "crete_file_queueu",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Simulate send to audit microservices
                channel.QueueDeclare(queue: "create_log_queueu",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                for (int i = 1; i <= 100; i++)
                {
                    string message1 = "crete_file_queueu: Crear archivos en el microservicio filesystem #" + i;
                    var body1 = Encoding.UTF8.GetBytes(message1);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "crete_file_queueu",
                                         basicProperties: null,
                                         body: body1);
                    Console.WriteLine(" [x] Sent {0}", message1);
                    Thread.Sleep(500);

                    string message2 = "create_log_queueu: Dejar traza de auditoria en el microservicio audit #" + i;
                    var body = Encoding.UTF8.GetBytes(message2);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "create_log_queueu",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message2);
                    Thread.Sleep(500);
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
