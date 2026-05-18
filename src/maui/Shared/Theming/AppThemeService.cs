namespace SelfCustodyHealth.Shared.Theming;

public interface IAppThemeService
{
	AppThemePreference Preference { get; }

	void ApplySavedTheme();

	void SetPreference(AppThemePreference preference);
}

public sealed class AppThemeService(IAppThemePreferenceStore preferenceStore) : IAppThemeService
{
	public AppThemePreference Preference => preferenceStore.Preference;

	public void ApplySavedTheme() => Apply(preferenceStore.Preference);

	public void SetPreference(AppThemePreference preference)
	{
		var normalizedPreference = Normalize(preference);
		if (normalizedPreference == preferenceStore.Preference)
		{
			Apply(normalizedPreference);
			return;
		}

		preferenceStore.Preference = normalizedPreference;
		Apply(normalizedPreference);
	}

	private static void Apply(AppThemePreference preference)
	{
		if (Application.Current is not { } application)
		{
			return;
		}

		application.UserAppTheme = preference switch
		{
			AppThemePreference.Light => AppTheme.Light,
			AppThemePreference.Dark => AppTheme.Dark,
			_ => AppTheme.Unspecified
		};
	}

	private static AppThemePreference Normalize(AppThemePreference preference) =>
		preference switch
		{
			AppThemePreference.System or AppThemePreference.Light or AppThemePreference.Dark => preference,
			_ => AppThemePreference.System
		};
}
