using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Cygni.Snake.Client;

namespace Cygni.Snake.SampleBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = SnakeClient.Connect(new Uri("ws://snake.cygni.se:80/training"), new GamePrinter());
            client.Start(new MySnakeBot("DotNet")
            {
                AutoStart = false
            }, new GameSettings
            {
                MaxNoofPlayers = 6
            });
            Console.ReadLine();
        }
    }
}