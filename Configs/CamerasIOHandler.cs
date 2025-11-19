namespace GRIDWATCH.Configs;

/*
public static class CamerasIoHandler
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

        _watcher.Changed += (_, _) =>
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

    internal static void WriteJson(List<CameraData> cameras)
    {
        try
        {
            var json = JsonConvert.SerializeObject(
                cameras,
                Formatting.Indented // keeps it readable
            );

            File.WriteAllText(FilePath, json);
            Info($"Wrote {cameras.Count} camera(s) to cameras.json successfully.");
        }
        catch (JsonException e)
        {
            Warn($"Failed to serialize cameras.json: {e.Message}");
        }
        catch (IOException e)
        {
            Warn($"I/O error while writing cameras.json: {e.Message}");
        }
    }

    internal static void AppendCamera(CameraData newCamera)
    {
        try
        {
            List<CameraData> current = new();

            if (DoesJsonFileExist("cameras.json"))
            {
                var json = File.ReadAllText(FilePath);
                var data = JsonConvert.DeserializeObject<List<CameraData>>(json);
                if (data != null)
                    current = data;
            }

            current.Add(newCamera);

            var updatedJson = JsonConvert.SerializeObject(
                current,
                Formatting.Indented
            );

            File.WriteAllText(FilePath, updatedJson);
            Info($"Appended camera (ID: {newCamera.Id}) – total {current.Count} camera(s) now.");
        }
        catch (JsonException e)
        {
            Warn($"Failed to append to cameras.json: {e.Message}");
        }
        catch (IOException e)
        {
            Warn($"I/O error while appending cameras.json: {e.Message}");
        }
    }
}*/

public class CameraData
{
    public int Id { get; set; }
    public Vector3 Position { get; set; }

    public CameraData()
    {
    }

    public CameraData(int id, Vector3 position)
    {
        Id = id;
        Position = position;
    }
}