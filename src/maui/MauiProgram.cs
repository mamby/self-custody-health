using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PersonalHealthVault.Crypto;
using PersonalHealthVault.Features.Appointments;
using PersonalHealthVault.Features.Dashboard;
using PersonalHealthVault.Features.HealthSummary;
using PersonalHealthVault.Features.Lock;
using PersonalHealthVault.Features.Medications;
using PersonalHealthVault.Features.Settings;
using PersonalHealthVault.Features.Vault;
using PersonalHealthVault.Security;
using PersonalHealthVault.Shared.Localization;
using PersonalHealthVault.Shared.Theming;
using PersonalHealthVault.Storage;

namespace PersonalHealthVault;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();
		builder.Services.AddSingleton<IVaultKeyProvider, SecureStorageVaultKeyProvider>();
		builder.Services.AddSingleton<IVaultStore>(services =>
		{
			var vaultPath = Path.Combine(
				FileSystem.AppDataDirectory,
				"vault",
				"self-custody-health.vault");

			return new EncryptedVaultStore(
				vaultPath,
				services.GetRequiredService<IVaultKeyProvider>(),
				services.GetRequiredService<IEncryptionService>());
		});
		builder.Services.AddSingleton<HealthDataService>();
		builder.Services.AddSingleton<IDocumentStore, DocumentSearchService>();
		builder.Services.AddSingleton<IBackupService, PendingBackupService>();
		builder.Services.AddSingleton<ILocalDocumentClassifier, PendingLocalDocumentClassifier>();
		builder.Services.AddSingleton<ILocalOcrService, PendingLocalOcrService>();
		builder.Services.AddSingleton<ILocalSummaryService, PendingLocalSummaryService>();
		builder.Services.AddSingleton<IDeviceUnlockService, DeviceUnlockService>();
		builder.Services.AddSingleton<IAppLockSettingsStore, PreferencesAppLockSettingsStore>();
		builder.Services.AddSingleton<IAppLockService, AppLockService>();
		builder.Services.AddSingleton<AppLockCoordinator>();
		builder.Services.AddSingleton<IAppLanguagePreferenceStore, PreferencesAppLanguagePreferenceStore>();
		builder.Services.AddSingleton<IAppLanguageService, AppLanguageService>();
		builder.Services.AddSingleton<IAppThemePreferenceStore, PreferencesAppThemePreferenceStore>();
		builder.Services.AddSingleton<IAppThemeService, AppThemeService>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<VaultPage>();
		builder.Services.AddTransient<HealthSummaryPage>();
		builder.Services.AddTransient<MedicationsPage>();
		builder.Services.AddTransient<AppointmentsPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<LockPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
