using System.Text.Json;

namespace JJPoems.Services;

internal static class SafeJsonStorage
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public static async Task<T?> ReadAsync<T>(string path)
    {
        if (!File.Exists(path)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(path).ConfigureAwait(false), Options)
                ?? throw new JsonException("Empty storage document.");
        }
        catch (JsonException) when (File.Exists(path + ".bak"))
        {
            return JsonSerializer.Deserialize<T>(await File.ReadAllTextAsync(path + ".bak").ConfigureAwait(false), Options)
                ?? throw new JsonException("Empty backup document.");
        }
    }

    public static async Task WriteAsync<T>(string path, T value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporary = path + ".tmp";
        using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await JsonSerializer.SerializeAsync(stream, value, Options).ConfigureAwait(false);
            stream.Flush(flushToDisk: true);
        }
        if (File.Exists(path))
        {
            // Keep the last valid document even if the current file was damaged.
            try
            {
                using var document = JsonDocument.Parse(await File.ReadAllTextAsync(path).ConfigureAwait(false));
                File.Copy(path, path + ".bak", overwrite: true);
            }
            catch (JsonException) { }
        }
        File.Move(temporary, path, overwrite: true);
    }
}
