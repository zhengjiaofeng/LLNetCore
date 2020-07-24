using LLC.Common.LogHeleper;
using LLC.Common.Tool.Configs;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLC.Common.RabbitMq
{
    public class RabbitMqClient
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
        /// 生产者---Direct模式
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        public void DirectPushMessage(string routingKey, string message)
        {
            try
            {
                string exchangeName = "direct_ll_test";
                string queueName = "queue_ll_test";
                string routingKeyName = "route_ll";
                using (IConnection con = factory.CreateConnection())//创建连接对象
                {
                    using (IModel channel = con.CreateModel())//创建连接会话对象
                    {

                        //申明队列(指定durable:true,告知rabbitmq对消息进行持久化)
                        channel.QueueDeclare(
                          queue: queueName,//消息队列名称
                          durable: true,//是否持久化
                          exclusive: false,
                          autoDelete: false,
                          arguments: null
                           );
                        //使用direct exchange type，指定exchange名称
                        channel.ExchangeDeclare(exchange: exchangeName, type: "direct");

                        //将队列绑定到交换机
                        channel.QueueBind(queueName, exchangeName, routingKeyName, null);

                        //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //消息内容
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        //发送消息
                        channel.BasicPublish(exchange: exchangeName, routingKey: routingKeyName, basicProperties: properties, body: body);
                        Console.WriteLine("成功发送消息:" + message);


                       
                    }
                }

            }

            catch (Exception ex)
            {
                Log4Util.Error("RabbitMQClient PushMessage ex：" + ex.ToString());
            }


        }



        public void SimplePushMessage(string message)
        {
            try
            {
                string queueName = "hello";
                using (IConnection con = factory.CreateConnection())//创建连接对象
                {
                    using (IModel channel = con.CreateModel())//创建连接会话对象
                    {

                        //申明队列(指定durable:true,告知rabbitmq对消息进行持久化)
                        channel.QueueDeclare(
                          queue: queueName,//消息队列名称
                          durable: false,//是否持久化
                          exclusive: false,
                          autoDelete: false,
                          arguments: null
                           );
                      

                        //将消息标记为持久性 - 将IBasicProperties.SetPersistent设置为true
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //消息内容
                        byte[] body = Encoding.UTF8.GetBytes(message);
                        //发送消息
                        channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                        Console.WriteLine("成功发送消息:" + message);



                    }
                }

            }

            catch (Exception ex)
            {
                Log4Util.Error("RabbitMQClient PushMessage ex：" + ex.ToString());
            }
        }

    }
}
