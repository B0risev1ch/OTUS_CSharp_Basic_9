using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static Program;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    static async Task Main()
    {
        var downloadTasks = new List<Task<bool>>();
        using var cts = new CancellationTokenSource();
        using var imageDownloader = new ImageDownloader();
        var token = cts.Token;
        imageDownloader.DownloadStarted += (string URL) => { Console.WriteLine($"Начал скачивать файл: " + URL); };
        imageDownloader.DownloadCompleted += (string fileName, int fileSize) => { Console.WriteLine($"Скачал файл: " + fileName + ". Размер: " + fileSize); };

        for (int i = 1; i <= 10; i++)
        {
            var downloadTask = imageDownloader.DownloadAsync("https://img.dummy-image-generator.com/abstract/dummy-4000x4000-Stones.jpg", $"bigimage_{i}.jpg", token);
            downloadTasks.Add(downloadTask);
        }

        while (true)
        {
            var keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.A)
            {
                Console.WriteLine("\nОстанавливаю...");
                cts.Cancel();
                Console.WriteLine($"Послал токен отмены?: {cts.IsCancellationRequested}");
            }
            else
            {
                Console.WriteLine($"Был запрос отмены?: {cts.IsCancellationRequested}\nСостояние загрузки:\n");

                foreach (var downloadTask in downloadTasks)
                {
                    Console.WriteLine($"TaskID =\"{downloadTask.Id}\" IsCompleted?: {downloadTask.IsCompleted};");
                }
            }
            if (keyInfo.Key == ConsoleKey.X)
            {
                break;
            }
        }

        foreach (var downloadTask in downloadTasks)
        {
            Console.WriteLine($"TaskID =\"{downloadTask.Id}\" Result: {downloadTask.Result};");
            downloadTask.Dispose();
        }
    }

}
