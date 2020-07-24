using LLC.Common.RabbitMq;
using LLC.Common.Tool.Configs;
using System;

namespace RabbitMqProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Producer!");

            AppSettingUtil appSettingUtil = new AppSettingUtil();
            while (true)
            {
                string msg = Console.ReadLine();

                //DirectPushMessage( msg);
                SimplePushMessage(msg);
                Console.ReadKey();

            }

        }

        /// <summary>
        /// 
        /// </summary>

        private static void DirectPushMessage(string msg)
        {

            new RabbitMqClient().DirectPushMessage("", msg);
        }

        private static void SimplePushMessage(string msg)
        {
            new RabbitMqClient().SimplePushMessage(msg);
        }
    }
}
