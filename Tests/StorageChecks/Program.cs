using JJPoems.Models;
using JJPoems.Services;
using System.Text.Json;

FileSystem.AppDataDirectory = Path.Combine(Path.GetTempPath(), "JJPoems-checks-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(FileSystem.AppDataDirectory);
var path = Path.Combine(FileSystem.AppDataDirectory, "storage.json");
void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
    Console.WriteLine("PASS: " + name);
}
await SafeJsonStorage.WriteAsync(path, new UserProfile { Bio = "Prvy text" });
await SafeJsonStorage.WriteAsync(path, new UserProfile { Bio = "Ticho\n\nDusa a nadej" });
Check((await SafeJsonStorage.ReadAsync<UserProfile>(path))!.Bio.Contains("\n\n"), "Verse spacing round trip");
await File.WriteAllTextAsync(path, "{broken");
Check((await SafeJsonStorage.ReadAsync<UserProfile>(path))!.Bio == "Prvy text", "Recover corrupt primary from backup");
await SafeJsonStorage.WriteAsync(path, new UserProfile { Bio = "Recovered" });
Check(JsonSerializer.Deserialize<UserProfile>(await File.ReadAllTextAsync(path + ".bak"))!.Bio == "Prvy text", "Preserve valid backup during recovery");

var workspace = new UserWorkspaceService(new TestAccountService());
await Task.WhenAll(Enumerable.Range(0, 20).Select(i => workspace.SaveWritingAsync(new UserWriting { Id = "id-" + i, Title = "Text " + i, Content = "Obsah" })));
Check((await workspace.GetWritingsAsync()).Count == 20, "Concurrent saves retain all writings");
var exported = await workspace.ExportAsync();
Check(await workspace.ImportAsync(exported) == 0, "Repeated import is idempotent");
var imported = new UserWorkspaceData { Writings = [new UserWriting { Id = "id-0", Title = "Do not overwrite", Content = "Other" }, new UserWriting { Id = "new", Title = "New", Content = "Text", CreatedAtUtc = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc) }] };
Check(await workspace.ImportAsync(JsonSerializer.Serialize(imported)) == 1, "Import adds only missing writings");
Check((await workspace.GetWritingByIdAsync("id-0"))!.Title == "Text 0", "Import preserves existing content");
Check((await workspace.GetWritingByIdAsync("new"))!.CreatedAtUtc.Year == 2020, "Import preserves creation date");
try { await workspace.ImportAsync("{broken"); throw new Exception("Invalid import accepted"); }
catch (JsonException) { Check((await workspace.GetWritingsAsync()).Count == 21, "Invalid backup leaves workspace intact"); }
Console.WriteLine("Checks complete. Temporary data: " + FileSystem.AppDataDirectory);
