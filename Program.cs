using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace LMSAnnouncements
{
    class Program
    {

        static void Main(string[] args)
        {
            var settings = Config.LoadCfg();
            if(settings.credentials.Length == 0)
            {
                Console.WriteLine("You Have To Set Account Credentials");
                Console.ReadLine();
                return;
            }
            if (settings.ChatId == 0)
            {
                Console.WriteLine("You Have To Set Chat Id");
                Console.ReadLine();
                return;
            }
            if (String.IsNullOrWhiteSpace(settings.TelegramToken))
            {
                Console.WriteLine("You Have To Set TelegramToken");
                Console.ReadLine();
                return;
            }
            LMSAPI api = new LMSAPI();
            if (api.Login(settings.credentials[0], settings.credentials[1]))
            {
                Console.WriteLine("Logged In...");
                Bot b = new Bot(api, settings.TelegramToken, settings.ChatId, settings.DebugMode, settings.DebugChatId);
                b.Start();
                while (true)
                {
                    Thread.Sleep(int.MaxValue);
                }
            }
            Console.WriteLine("Email Or Password Is Incorrect...");
        }


        static void WriteLine(string tada)
        {
            System.IO.File.AppendAllText("log.txt", $"{tada}\r\n\r\n\r\n");
        }
    }
}
