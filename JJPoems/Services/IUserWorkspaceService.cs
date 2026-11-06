using JJPoems.Models;

namespace JJPoems.Services;

public interface IUserWorkspaceService
{
	Task EnsureWorkspaceAsync();
	Task<UserProfile> GetProfileAsync();
	Task SaveProfileAsync(UserProfile profile);
	Task<IReadOnlyList<UserWriting>> GetWritingsAsync();
	Task<UserWriting?> GetWritingByIdAsync(string writingId);
	Task SaveWritingAsync(UserWriting writing);
	Task DeleteWritingAsync(string writingId);
	Task<string> ExportAsync();
	Task<int> ImportAsync(string json);
}
