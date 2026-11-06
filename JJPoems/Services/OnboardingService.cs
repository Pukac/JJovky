using Microsoft.Maui.Storage;

namespace JJPoems.Services;

public class OnboardingService : IOnboardingService
{
	private const string OnboardingCompletedKey = "onboarding-completed";

	public bool HasCompletedOnboarding()
	{
		return Preferences.Default.Get(OnboardingCompletedKey, false);
	}

	public void CompleteOnboarding()
	{
		Preferences.Default.Set(OnboardingCompletedKey, true);
	}
}
