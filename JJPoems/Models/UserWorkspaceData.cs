namespace JJPoems.Models;

public class UserWorkspaceData
{
	public UserProfile Profile { get; set; } = new();
	public List<UserWriting> Writings { get; set; } = [];
}
