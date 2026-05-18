using Microsoft.Extensions.DependencyInjection;
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

namespace PersonalHealthVault;

public partial class AppShell : Shell
{
	private readonly IServiceProvider _services;
	private readonly AppLockCoordinator _appLockCoordinator;
	private readonly IAppLanguageService _languageService;
	private readonly SemaphoreSlim _lockPresentation = new(1, 1);
	private TabBar? _mainTabs;

	public AppShell(IServiceProvider services)
	{
		_services = services;
		_appLockCoordinator = services.GetRequiredService<AppLockCoordinator>();
		_languageService = services.GetRequiredService<IAppLanguageService>();
		InitializeComponent();
		Title = AppText.Get("AppName");
		ThemeResources.ApplyShellChrome(this);
		ThemeResources.ApplyTabBarChrome(this);
		Items.Add(CreateMainTabs());
		_languageService.LanguageChanged += (_, _) => MainThread.BeginInvokeOnMainThread(RefreshLocalizedText);
	}

	private void RefreshLocalizedText()
	{
		Title = AppText.Get("AppName");

		if (_mainTabs is null)
		{
			return;
		}

		foreach (var item in _mainTabs.Items)
		{
			item.Title = item.Route switch
			{
				"dashboard" => AppText.Get("ShellDashboard"),
				"vault" => AppText.Get("ShellVault"),
				"health-summary" => AppText.Get("ShellSummary"),
				"medications" => AppText.Get("ShellMedications"),
				"appointments" => AppText.Get("ShellAppointments"),
				"settings" => AppText.Get("ShellSettings"),
				_ => item.Title
			};
		}
	}

	public Task ShowLockIfRequiredAsync()
	{
		var decision = _appLockCoordinator.RequestPresentation();
		return ApplyLockPresentationAsync(decision);
	}

	public async Task ApplyLockPresentationAsync(AppLockPresentationDecision decision)
	{
		if (decision is { ShouldShowLock: false, ShouldStartAutomaticUnlock: false })
		{
			return;
		}

		LockPage? lockPageToUnlock = null;

		await _lockPresentation.WaitAsync();
		try
		{
			if (decision.ShouldShowLock)
			{
				var lockPage = _services.GetRequiredService<LockPage>();
				lockPage.Unlocked += (_, _) => _appLockCoordinator.NotifyLockDismissed();

				try
				{
					await Navigation.PushModalAsync(lockPage);
					lockPageToUnlock = decision.ShouldStartAutomaticUnlock ? lockPage : null;
				}
				catch
				{
					_appLockCoordinator.NotifyLockPresentationFailed();
					throw;
				}
			}
			else if (decision.ShouldStartAutomaticUnlock)
			{
				lockPageToUnlock = Navigation.ModalStack.OfType<LockPage>().LastOrDefault();
			}
		}
		finally
		{
			_lockPresentation.Release();
		}

		if (lockPageToUnlock is not null)
		{
			lockPageToUnlock.Dispatcher.Dispatch(() =>
			{
				_ = lockPageToUnlock.StartAutomaticUnlockAsync();
			});
		}
	}

	private TabBar CreateMainTabs()
	{
		var tabs = new TabBar();
		_mainTabs = tabs;
		ThemeResources.ApplyShellChrome(tabs);
		ThemeResources.ApplyTabBarChrome(tabs);
		tabs.Items.Add(CreateContent<DashboardPage>(AppText.Get("ShellDashboard"), "dashboard"));
		tabs.Items.Add(CreateContent<VaultPage>(AppText.Get("ShellVault"), "vault"));
		tabs.Items.Add(CreateContent<HealthSummaryPage>(AppText.Get("ShellSummary"), "health-summary"));
		tabs.Items.Add(CreateContent<MedicationsPage>(AppText.Get("ShellMedications"), "medications"));
		tabs.Items.Add(CreateContent<AppointmentsPage>(AppText.Get("ShellAppointments"), "appointments"));
		tabs.Items.Add(CreateContent<SettingsPage>(AppText.Get("ShellSettings"), "settings"));

		return tabs;
	}

	private ShellContent CreateContent<TPage>(string title, string route)
		where TPage : Page
	{
		var content = new ShellContent
		{
			Title = title,
			Route = route,
			ContentTemplate = new DataTemplate(() => _services.GetRequiredService<TPage>())
		};
		ThemeResources.ApplyShellChrome(content);
		ThemeResources.ApplyTabBarChrome(content);
		return content;
	}
}
