namespace SelfCustodyHealth.Shared.Localization;

internal sealed class PreferencesAppLanguagePreferenceStore : IAppLanguagePreferenceStore
{
	private const string LanguagePreferenceKey = "self_custody_health_language_preference";

	public AppLanguagePreference Preference
	{
		get => Parse(Preferences.Default.Get(LanguagePreferenceKey, AppLanguagePreference.System.ToString()));
		set => Preferences.Default.Set(LanguagePreferenceKey, Normalize(value).ToString());
	}

	private static AppLanguagePreference Parse(string? value) =>
		Enum.TryParse<AppLanguagePreference>(value, ignoreCase: true, out var preference) && Enum.IsDefined(preference)
			? preference
			: AppLanguagePreference.System;

	private static AppLanguagePreference Normalize(AppLanguagePreference preference) =>
		preference switch
		{
			AppLanguagePreference.System or AppLanguagePreference.English or AppLanguagePreference.French or AppLanguagePreference.Arabic => preference,
			_ => AppLanguagePreference.System
		};
}
