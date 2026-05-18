using System.Globalization;

namespace SelfCustodyHealth.Shared.Localization;

/// <summary>
/// Applies the selected application language and layout direction.
/// </summary>
public interface IAppLanguageService
{
	/// <summary>Gets the saved language preference.</summary>
	AppLanguagePreference Preference { get; }

	/// <summary>Gets the culture currently applied to application resources.</summary>
	CultureInfo EffectiveCulture { get; }

	/// <summary>Gets the layout direction for the effective culture.</summary>
	FlowDirection FlowDirection { get; }

	/// <summary>Raised after the language preference changes.</summary>
	event EventHandler? LanguageChanged;

	/// <summary>Applies the saved language preference at startup.</summary>
	void ApplySavedLanguage();

	/// <summary>Applies the effective layout direction to a window.</summary>
	void ApplyFlowDirection(Window window);

	/// <summary>Saves and applies a language preference.</summary>
	void SetPreference(AppLanguagePreference preference);
}

internal sealed class AppLanguageService(IAppLanguagePreferenceStore preferenceStore) : IAppLanguageService
{
	private readonly CultureInfo _deviceCulture = CultureInfo.CurrentCulture;
	private readonly CultureInfo _deviceUiCulture = CultureInfo.CurrentUICulture;

	public AppLanguagePreference Preference => preferenceStore.Preference;

	public CultureInfo EffectiveCulture { get; private set; } = CultureInfo.CurrentUICulture;

	public FlowDirection FlowDirection =>
		EffectiveCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

	public event EventHandler? LanguageChanged;

	public void ApplySavedLanguage() =>
		Apply(preferenceStore.Preference, notify: false);

	public void ApplyFlowDirection(Window window) =>
		window.FlowDirection = FlowDirection;

	public void SetPreference(AppLanguagePreference preference)
	{
		var normalizedPreference = Normalize(preference);
		if (normalizedPreference == preferenceStore.Preference)
		{
			Apply(normalizedPreference, notify: false);
			return;
		}

		preferenceStore.Preference = normalizedPreference;
		Apply(normalizedPreference, notify: true);
	}

	private void Apply(AppLanguagePreference preference, bool notify)
	{
		var (culture, uiCulture) = GetCultures(preference);

		CultureInfo.CurrentCulture = culture;
		CultureInfo.CurrentUICulture = uiCulture;
		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = uiCulture;
		EffectiveCulture = uiCulture;
		ApplyFlowDirectionToCurrentWindows();

		if (notify)
		{
			LanguageChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void ApplyFlowDirectionToCurrentWindows()
	{
		if (Application.Current is not { } application)
		{
			return;
		}

		foreach (var window in application.Windows)
		{
			ApplyFlowDirection(window);
		}
	}

	private (CultureInfo Culture, CultureInfo UiCulture) GetCultures(AppLanguagePreference preference) =>
		Normalize(preference) switch
		{
			AppLanguagePreference.English => CreateCultures("en"),
			AppLanguagePreference.French => CreateCultures("fr"),
			AppLanguagePreference.Arabic => CreateCultures("ar"),
			_ when IsSupportedCulture(_deviceUiCulture) => (_deviceCulture, _deviceUiCulture),
			_ => CreateCultures("en")
		};

	private static (CultureInfo Culture, CultureInfo UiCulture) CreateCultures(string name)
	{
		var culture = CultureInfo.GetCultureInfo(name);
		return (culture, culture);
	}

	private static bool IsSupportedCulture(CultureInfo culture) =>
		culture.TwoLetterISOLanguageName is "en" or "fr" or "ar";

	private static AppLanguagePreference Normalize(AppLanguagePreference preference) =>
		preference switch
		{
			AppLanguagePreference.System or AppLanguagePreference.English or AppLanguagePreference.French or AppLanguagePreference.Arabic => preference,
			_ => AppLanguagePreference.System
		};
}
