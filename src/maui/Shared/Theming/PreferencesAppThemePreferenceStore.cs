namespace SelfCustodyHealth.Shared.Theming;

public sealed class PreferencesAppThemePreferenceStore : IAppThemePreferenceStore
{
	private const string ThemePreferenceKey = "self_custody_health_theme_preference";

	public AppThemePreference Preference
	{
		get => Parse(Preferences.Default.Get(ThemePreferenceKey, AppThemePreference.System.ToString()));
		set => Preferences.Default.Set(ThemePreferenceKey, Normalize(value).ToString());
	}

	private static AppThemePreference Parse(string? value) =>
		Enum.TryParse<AppThemePreference>(value, ignoreCase: true, out var preference) && Enum.IsDefined(preference)
			? preference
			: AppThemePreference.System;

	private static AppThemePreference Normalize(AppThemePreference preference) =>
		preference switch
		{
			AppThemePreference.System or AppThemePreference.Light or AppThemePreference.Dark => preference,
			_ => AppThemePreference.System
		};
}
