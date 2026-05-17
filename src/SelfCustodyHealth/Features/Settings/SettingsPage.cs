using SelfCustodyHealth.Security;
using SelfCustodyHealth.Shared.Localization;
using SelfCustodyHealth.Shared.Theming;
using SelfCustodyHealth.Shared.Ui;
using SelfCustodyHealth.Storage;

namespace SelfCustodyHealth.Features.Settings;

public sealed class SettingsPage(
	IAppLockService appLockService,
	IDeviceUnlockService deviceUnlockService,
	IBackupService backupService,
	HealthDataService dataService,
	IAppThemeService appThemeService,
	IAppLanguageService appLanguageService) : ThemedContentPage
{
	private static readonly AppThemePreference[] ThemePreferences =
	[
		AppThemePreference.System,
		AppThemePreference.Light,
		AppThemePreference.Dark
	];
	private static readonly AppLanguagePreference[] LanguagePreferences =
	[
		AppLanguagePreference.System,
		AppLanguagePreference.English,
		AppLanguagePreference.French,
		AppLanguagePreference.Arabic
	];

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await RenderAsync();
	}

	private async Task RenderAsync()
	{
		var availability = await deviceUnlockService.GetAvailabilityAsync(CancellationToken.None);
		var backupConfigured = await backupService.IsConfiguredAsync(CancellationToken.None);

		var lockButton = new Button
		{
			Text = appLockService.IsEnabled ? AppText.Get("DisableAppLock") : AppText.Get("EnableAppLock")
		};
		lockButton.Clicked += async (_, _) => await ToggleAppLockAsync();

		var lockNowButton = Ui.SecondaryButton(AppText.Get("LockNow"));
		lockNowButton.IsEnabled = appLockService.IsEnabled;
		lockNowButton.Clicked += async (_, _) =>
		{
			appLockService.LockNow();
			if (Shell.Current is AppShell shell)
			{
				await shell.ShowLockIfRequiredAsync();
			}
		};

		var deleteButton = Ui.DestructiveButton(AppText.Get("DeleteLocalVault"));
		deleteButton.Clicked += async (_, _) => await DeleteLocalVaultAsync();

		var themePicker = CreateThemePicker();
		var languagePicker = CreateLanguagePicker();

		Content = Ui.Scroll(Ui.PageStack(
			Ui.PageTitle(AppText.Get("SettingsTitle")),
			Ui.Body(AppText.Get("PrivacyIntro")),
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 10,
				Children =
				{
					Ui.SectionTitle(AppText.Get("Privacy")),
					Ui.Body(AppText.Get("DashboardNotice")),
					Ui.Muted(AppText.Get("PrivacyNoAdsBody"))
				}
			}),
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 10,
				Children =
				{
					Ui.SectionTitle(AppText.Get("Appearance")),
					Ui.Body(AppText.Get("AppearanceBody")),
					themePicker,
					languagePicker
				}
			}),
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 10,
				Children =
				{
					Ui.SectionTitle(AppText.Get("EncryptionStatus")),
					Ui.Body(dataService.IsDemoData ? AppText.Get("DemoDataOnly") : AppText.Get("EncryptedLocalVault")),
					Ui.Muted(AppText.Get("AuthenticatedEncryptionBody"))
				}
			}),
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 10,
				Children =
				{
					Ui.SectionTitle(AppText.Get("AppLock")),
					Ui.Body(appLockService.IsEnabled ? AppText.Get("AppLockOnBody") : AppText.Get("AppLockOffBody")),
					Ui.Muted(AppText.Format("DeviceUnlockAvailabilityFormat", DeviceUnlockText.Availability(availability))),
					lockButton,
					lockNowButton
				}
			}),
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 10,
				Children =
				{
					Ui.SectionTitle(AppText.Get("Backup")),
					Ui.Body(backupConfigured ? AppText.Get("BackupConfigured") : AppText.Get("BackupPending")),
					Ui.Muted(AppText.Get("BackupFutureBody"))
				}
			}),
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 10,
				Children =
				{
					Ui.SectionTitle(AppText.Get("Data")),
					Ui.Body(AppText.Get("DataBody")),
					deleteButton
				}
			}),
			Ui.Muted(AppText.Get("DiagnosticDisclaimer"))));
	}

	private Picker CreateThemePicker()
	{
		var selectedPreference = appThemeService.Preference;
		var selectedIndex = Array.IndexOf(ThemePreferences, selectedPreference);
		var picker = new Picker
		{
			Title = AppText.Get("ThemePickerTitle"),
			ItemsSource = ThemePreferences.Select(GetThemePreferenceLabel).ToArray(),
			SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0
		};

		picker.SelectedIndexChanged += (_, _) =>
		{
			if (picker.SelectedIndex < 0 || picker.SelectedIndex >= ThemePreferences.Length)
			{
				return;
			}

			appThemeService.SetPreference(ThemePreferences[picker.SelectedIndex]);
		};

		return picker;
	}

	private Picker CreateLanguagePicker()
	{
		var selectedPreference = appLanguageService.Preference;
		var selectedIndex = Array.IndexOf(LanguagePreferences, selectedPreference);
		var picker = new Picker
		{
			Title = AppText.Get("LanguagePickerTitle"),
			ItemsSource = LanguagePreferences.Select(GetLanguagePreferenceLabel).ToArray(),
			SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0
		};

		picker.SelectedIndexChanged += async (_, _) =>
		{
			if (picker.SelectedIndex < 0 || picker.SelectedIndex >= LanguagePreferences.Length)
			{
				return;
			}

			appLanguageService.SetPreference(LanguagePreferences[picker.SelectedIndex]);
			await RenderAsync();
		};

		return picker;
	}

	private async Task ToggleAppLockAsync()
	{
		if (appLockService.IsEnabled)
		{
			appLockService.Disable();
			await RenderAsync();
			return;
		}

		var result = await appLockService.EnableAsync(CancellationToken.None);
		if (!result.Succeeded)
		{
			await DisplayAlertAsync(AppText.Get("AppLock"), result.Message, AppText.Get("CommonOk"));
		}

		await RenderAsync();
	}

	private async Task DeleteLocalVaultAsync()
	{
		var confirmed = await DisplayAlertAsync(
			AppText.Get("DeleteLocalVault"),
			AppText.Get("DeleteVaultConfirmMessage"),
			AppText.Get("CommonDelete"),
			AppText.Get("CommonCancel"));

		if (!confirmed)
		{
			return;
		}

		await dataService.DeleteLocalVaultAsync();
		await RenderAsync();
	}

	private static string GetThemePreferenceLabel(AppThemePreference preference) =>
		preference switch
		{
			AppThemePreference.Light => AppText.Get("ThemeLight"),
			AppThemePreference.Dark => AppText.Get("ThemeDark"),
			_ => AppText.Get("ThemeSystem")
		};

	private static string GetLanguagePreferenceLabel(AppLanguagePreference preference) =>
		preference switch
		{
			AppLanguagePreference.English => AppText.Get("LanguageEnglish"),
			AppLanguagePreference.French => AppText.Get("LanguageFrench"),
			AppLanguagePreference.Arabic => AppText.Get("LanguageArabic"),
			_ => AppText.Get("LanguageSystem")
		};
}
