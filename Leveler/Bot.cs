﻿using Guilded;
using Leveler.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Websocket.Client;

namespace Leveler
{
    public class Bot 
    {
        private static readonly string? json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "config.json"));
        private static readonly string? token = JsonSerializer.Deserialize<ConfigJson>(json!)!.Token!;
        private static readonly string? prefix = JsonSerializer.Deserialize<ConfigJson>(json!)!.Prefix!;
        private static readonly string? timePattern = "hh:mm:ss tt";

        public async Task RunAsync()
        {
            await using var client = new GuildedBotClient(token!);

            client.Prepared
                .Subscribe(async me =>
                {
                    var time = DateTime.Now.ToString(timePattern);
                    var date = DateTime.Now.ToShortDateString();
                    Console.WriteLine($"[{date}][{time}][INFO]  [{me.ParentClient.Name}] connecting to gateway...");
                });

            client.Disconnected
                .Where(e => e.Type != DisconnectionType.NoMessageReceived)
                .Subscribe(me =>
                {
                    var time = DateTime.Now.ToString(timePattern);
                    var date = DateTime.Now.ToShortDateString();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[{date}][{time}][ERROR] [{client.Name}] disconnected from gateway...");
                });

            client.Reconnected
                .Where(x => x.Type != ReconnectionType.Initial)
                .Where(x => x.Type != ReconnectionType.NoMessageReceived)
                .Subscribe(me =>
                {

                    var time = DateTime.Now.ToString(timePattern);
                    var date = DateTime.Now.ToShortDateString();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"[{date}][{time}][INFO]  [{client.Name}] reconnected to gateway...");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"[{date}][{time}][INFO]  [{client.Name}] listening for events...");
                });


            //connect bot to Guilded
            await client.ConnectAsync();
            await client.SetStatusAsync("Leveling", 90002579);
            var time = DateTime.Now.ToString(timePattern);
            var date = DateTime.Now.ToShortDateString();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[{date}][{time}][INFO]  [{client.Name}] connected...");
            Console.WriteLine($"[{date}][{time}][INFO]  [{client.Name}] registering command modules...");
            await Task.Delay(200);
            Console.WriteLine($"[{date}][{time}][INFO]  [{client.Name}] listening for events...");
            await Task.Delay(-1);//keep bot alive
        }
    }
}
