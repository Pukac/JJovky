namespace JJPoems.Services;

public interface IOnboardingService
{
	bool HasCompletedOnboarding();
	void CompleteOnboarding();
}
