using JJPoems.Models;

namespace JJPoems.Services;

public interface IQuickCaptureDraftService
{
	Task<QuickCaptureDraft?> GetDraftAsync(string? writingId = null);
	Task SaveDraftAsync(QuickCaptureDraft draft);
	Task ClearDraftAsync(string? writingId = null);
}
