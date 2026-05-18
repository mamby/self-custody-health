namespace PersonalHealthVault.Shared.Localization;

internal interface IAppLanguagePreferenceStore
{
	AppLanguagePreference Preference { get; set; }
}
