namespace JJPoems.Models
{
    public class UserWriting
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string Mood { get; set; } = "";
        public string Section { get; set; } = "Basnicky";
        public string PaletteKey { get; set; } = "sand";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
    public class UserAccount
    {
        public string Id { get; set; } = "test-profile";
        public string DisplayName { get; set; } = "Test";
        public string Tagline { get; set; } = "";
        public string PreferredPaletteKey { get; set; } = "sand";
    }
}
namespace JJPoems.Services
{
    public static class FileSystem
    {
        public static string AppDataDirectory { get; set; } = "";
    }
    public interface IUserAccountService
    {
        Task<JJPoems.Models.UserAccount?> GetCurrentAccountAsync();
    }
    public class TestAccountService : IUserAccountService
    {
        public Task<JJPoems.Models.UserAccount?> GetCurrentAccountAsync() => Task.FromResult<JJPoems.Models.UserAccount?>(new());
    }
}
