using LLC.Common.LogHeleper;
using LLC.Common.Tool.Configs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LLC.Common.RabbitMq
{
    public class RabbitMqConsumer
    {

        private ConnectionFactory factory = new ConnectionFactory()
        {

            HostName = AppSettingUtil.GetSectionValue("RabbitMQ:Host"),
            UserName = AppSettingUtil.GetSectionValue("RabbitMQ:UserName"),
            Password = AppSettingUtil.GetSectionValue("RabbitMQ:Password"),
            Port = int.Parse(AppSettingUtil.GetSectionValue("RabbitMQ:Port")),
            VirtualHost = AppSettingUtil.GetSectionValue("RabbitMQ:VirtualHost")
        };

        /// <summary>
        ///  消费者--   Direct模式 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        public void DirectConsumerReceived(string queueName, string exchangeName, string routingKey)
        {
            try
            {
                //创建连接
                using (IConnection con = factory.CreateConnection())//创建连接对象
                {
                    //创建信道
                    using (IModel channel = con.CreateModel())//创建连接会话对象
                    {
                        //申明direct类型exchange
                        channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
                        //绑定队列到direct类型exchange，需指定路由键routingKey
                        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

                        // 构造消费者实例
                        var consumer = new EventingBasicConsumer(channel);
                        //声明为手动确认
                        channel.BasicQos(0, 1, false);
                        //绑定消息接收后的事件委托
                        consumer.Received += (model, ea) =>
                        {
                            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                            Console.WriteLine(" [x] Received {0}", message);
                            //业务处理时间
                            Thread.Sleep(1000);

                            //发送消息确认信号（手动消息确认）
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false) ;
                        };

                        // 启动消费者
                        //autoAck:true；自动进行消息确认，当消费端接收到消息后，就自动发送ack信号，不管消息是否正确处理完毕
                        //autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认
                        channel.BasicConsume( queueName, false,  consumer);
                       // channel.BasicConsume(queueName, true, consumer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Util.Error("RabbitMQClient ConsumerReceived ex：" + ex.ToString());
            }

        }


        public void SimpleConsumerReceived(string queueName)
        {
            try
            {
                //2. 建立连接
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
                }
            }
            catch (Exception ex)
            { 
            
            }
        }
    }
}
