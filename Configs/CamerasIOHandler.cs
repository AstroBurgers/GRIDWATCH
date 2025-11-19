using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rage;
using static GRIDWATCH.Utils.Logger;

namespace GRIDWATCH.Configs;

public static class CamerasIOHandler
{
    private static readonly string FilePath =
        @"plugins\LSPDFR\GRIDWATCH\cameras.json";

    private static FileSystemWatcher _watcher;


    internal static void EnableHotReload()
    {
        _watcher = new FileSystemWatcher(@"plugins\LSPDFR\GRIDWATCH", "cameras.json")
        {
            NotifyFilter = NotifyFilters.LastWrite
        };

        _watcher.Changed += (s, e) =>
        {
            GameFiber.StartNew(() =>
            {
                GameFiber.Sleep(500); // debounce
                ReadJson();
                Game.DisplayNotification("CHAR_CALL911", "CHAR_CALL911",
                    "GRIDWATCH", "~g~Reloaded Cameras!",
                    "Your cameras.json was modified and automatically reloaded.");
            });
        };
        _watcher.EnableRaisingEvents = true;

        Info("Camera hot-reload watcher enabled.");
    }
    
    public static List<CameraData> Cameras { get; private set; } = new();

    internal static bool DoesJsonFileExist(string filename)
    {
        var fullPath = Path.Combine(@"plugins\LSPDFR\GRIDWATCH", filename);

        if (File.Exists(fullPath))
            return true;

        Warn($"Failed to load because {filename} does not exist.");
        Game.DisplayNotification(
            "commonmenu",
            "mp_alerttriangle",
            "GRIDWATCH",
            "~r~Missing files!",
            $"Could not find ~y~{filename}~s~!\n" +
            "Please verify you have installed GRIDWATCH properly."
        );

        return false;
    }

    internal static void ReadJson()
    {
        if (!DoesJsonFileExist("cameras.json"))
            return;

        try
        {
            var json = File.ReadAllText(FilePath);
            var data = JsonConvert.DeserializeObject<List<CameraData>>(json);

            if (data == null || data.Count == 0)
            {
                Warn("Cameras.json is empty or invalid JSON.");
                return;
            }

            Cameras = data;
            Info($"Loaded {Cameras.Count} camera(s) from cameras.json successfully.");
        }
        catch (JsonException e)
        {
            Warn($"Failed to parse cameras.json: {e.Message}");
        }
        catch (IOException e)
        {
            Warn($"I/O error while reading cameras.json: {e.Message}");
        }
    }
}

public class CameraData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Position Position { get; set; }
}

public class Position
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}