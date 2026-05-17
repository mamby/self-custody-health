using SelfCustodyHealth.Security;
using SelfCustodyHealth.Shared.Localization;
using SelfCustodyHealth.Shared.Theming;
using SelfCustodyHealth.Shared.Ui;

namespace SelfCustodyHealth.Features.Lock;

public sealed class LockPage : ThemedContentPage
{
	private readonly IAppLockService _appLockService;
	private readonly Label _message = Ui.Muted(string.Empty);
	private readonly Button _unlockButton = new()
	{
		Text = AppText.Get("Unlock")
	};

	private CancellationTokenSource? _unlockCancellation;
	private int _unlockAttemptId;
	private bool _unlockInProgress;

	public LockPage(IAppLockService appLockService)
	{
		_appLockService = appLockService;
		NavigationPage.SetHasNavigationBar(this, false);
		_unlockButton.Clicked += async (_, _) => await UnlockAsync();

		Content = Ui.Scroll(Ui.PageStack(
			Ui.PageTitle(AppText.Get("VaultLocked")),
			Ui.Body(AppText.Get("UnlockPageIntro")),
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 12,
				Children =
				{
					Ui.SectionTitle(AppText.Get("EncryptedLocalVault")),
					Ui.Body(AppText.Get("DashboardNotice")),
					_message,
					_unlockButton
				}
			})));
	}

	public event EventHandler? Unlocked;

	public Task StartAutomaticUnlockAsync() => UnlockAsync();

	protected override bool OnBackButtonPressed() => true;

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (!_unlockInProgress)
		{
			_unlockButton.IsEnabled = true;
		}
	}

	protected override void OnDisappearing()
	{
		CancelActiveUnlock();
		base.OnDisappearing();
	}

	private async Task UnlockAsync()
	{
		if (_unlockInProgress)
		{
			return;
		}

		var unlockCancellation = new CancellationTokenSource();
		var unlockAttemptId = ++_unlockAttemptId;
		_unlockCancellation = unlockCancellation;
		_unlockInProgress = true;
		_unlockButton.IsEnabled = false;
		_message.Text = string.Empty;

		var unlocked = false;
		try
		{
			var result = await _appLockService.UnlockAsync(unlockCancellation.Token);
			if (unlockAttemptId != _unlockAttemptId)
			{
				return;
			}

			if (result.Succeeded)
			{
				unlocked = true;
				Unlocked?.Invoke(this, EventArgs.Empty);
				await Navigation.PopModalAsync();
				return;
			}

			_message.Text = result.Message;
		}
		finally
		{
			var isCurrentAttempt = unlockAttemptId == _unlockAttemptId;
			if (ReferenceEquals(_unlockCancellation, unlockCancellation))
			{
				_unlockCancellation = null;
			}

			unlockCancellation.Dispose();
			if (isCurrentAttempt)
			{
				_unlockInProgress = false;
				_unlockButton.IsEnabled = !unlocked;
			}
		}
	}

	private void CancelActiveUnlock()
	{
		_unlockCancellation?.Cancel();
		_appLockService.CancelPendingUnlock();
	}
}
