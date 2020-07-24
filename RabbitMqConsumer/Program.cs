using LLC.Common.Tool.Configs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMqConsumer
{
    class Program
    {
        private static ConnectionFactory factory = new ConnectionFactory()
        {

            HostName = AppSettingUtil.GetSectionValue("RabbitMQ:Host"),
            UserName = AppSettingUtil.GetSectionValue("RabbitMQ:UserName"),
            Password = AppSettingUtil.GetSectionValue("RabbitMQ:Password"),
            Port = int.Parse(AppSettingUtil.GetSectionValue("RabbitMQ:Port")),
            VirtualHost = AppSettingUtil.GetSectionValue("RabbitMQ:VirtualHost")
        };
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");
            AppSettingUtil appSettingUtil = new AppSettingUtil();
            //DirectConsumerReceived();
            //SimpleConsumerReceived();

            using (var connection = factory.CreateConnection())
            {
                //3. 创建信道
                using (var channel = connection.CreateModel())
                {
                    //4. 申明队列
                    channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //5. 构造消费者实例
                    var consumer = new EventingBasicConsumer(channel);
                    //6. 绑定消息接收后的事件委托
                    consumer.Received += (model, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine(" [x] Received {0}", message);
                        Thread.Sleep(1000);//模拟耗时
                        Console.WriteLine(" [x] Done");
                    };
                    //7. 启动消费者
                    channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
                }
                Console.ReadKey();


            }
        }

        static void DirectConsumerReceived()
        {
            string exchangeName = "direct_ll_test";
            string queueName = "queue_ll_test";
            string routingKeyName = "route_ll";

            new LLC.Common.RabbitMq.RabbitMqConsumer().DirectConsumerReceived(queueName, exchangeName, routingKeyName);
        }

        static void SimpleConsumerReceived()
        {
            string queueName = "hello";
            new LLC.Common.RabbitMq.RabbitMqConsumer().SimpleConsumerReceived(queueName);
        }
    }
}
