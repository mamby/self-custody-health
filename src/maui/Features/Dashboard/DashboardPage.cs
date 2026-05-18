using PersonalHealthVault.Domain;
using PersonalHealthVault.Security;
using PersonalHealthVault.Shared;
using PersonalHealthVault.Shared.Localization;
using PersonalHealthVault.Shared.Theming;
using PersonalHealthVault.Shared.Ui;
using PersonalHealthVault.Storage;

namespace PersonalHealthVault.Features.Dashboard;

public sealed class DashboardPage(
	HealthDataService dataService,
	IAppLockService appLockService) : ThemedContentPage
{
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await RenderAsync();
	}

	private async Task RenderAsync()
	{
		var snapshot = await dataService.GetSnapshotAsync();
		var nextReminder = snapshot.Reminders
			.Select(reminder => new
			{
				Reminder = reminder,
				Next = ReminderScheduler.GetNextOccurrence(reminder, DateTimeOffset.Now)
			})
			.Where(item => item.Next is not null)
			.OrderBy(item => item.Next)
			.FirstOrDefault();

		var nextAppointment = snapshot.Appointments.OrderBy(a => a.StartsAt).FirstOrDefault();

		Content = Ui.Scroll(Ui.PageStack(
			Ui.PageTitle(AppText.Format("DashboardGreetingFormat", snapshot.Profile.DisplayName)),
			Ui.Body(AppText.Get("DashboardNotice")),
			CreatePrivacyCard(),
			CreateMetrics(snapshot),
			CreateSummaryCard(snapshot),
			CreateNextCard(
				AppText.Get("NextAppointment"),
				nextAppointment?.Title ?? AppText.Get("NoUpcomingAppointmentsShort"),
				nextAppointment is null ? AppText.Get("AddOneWhenReady") : AppText.FormatDateTime(nextAppointment.StartsAt)),
			CreateNextCard(
				AppText.Get("NextReminder"),
				nextReminder?.Reminder.Title ?? AppText.Get("NoActiveReminders"),
				nextReminder?.Next is { } reminderTime ? AppText.FormatDateTime(reminderTime) : AppText.Get("RemindersStayLocal")),
			CreateAiCard()));
	}

	private Border CreatePrivacyCard()
	{
		var status = appLockService.IsEnabled ? AppText.Get("AppLockEnabled") : AppText.Get("AppLockAvailable");
		var badge = Ui.Badge(status, appLockService.IsEnabled ? UiTone.Success : UiTone.Info);

		var header = new Grid
		{
			ColumnDefinitions =
			{
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto)
			},
			Children =
			{
				Ui.SectionTitle(AppText.Get("EncryptedLocalVault")),
				badge
			}
		};
		Grid.SetColumn(badge, 1);

		return Ui.Card(new VerticalStackLayout
		{
			Spacing = 12,
			Children =
			{
				header,
				Ui.Body(AppText.Get("HealthRecordsLocalBody")),
				Ui.Muted(AppText.Get("BiometricGateBody"))
			}
		});
	}

	private static Grid CreateMetrics(HealthVaultSnapshot snapshot)
	{
		var grid = new Grid
		{
			ColumnDefinitions =
			{
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Star)
			},
			RowDefinitions =
			{
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Auto)
			},
			ColumnSpacing = 12,
			RowSpacing = 12
		};

		AddMetric(grid, AppText.Get("DocumentsMetric"), snapshot.Documents.Count.ToString(AppText.Culture), 0, 0);
		AddMetric(grid, AppText.Get("MedicationsMetric"), snapshot.Medications.Count(m => m.IsActive).ToString(AppText.Culture), 1, 0);
		AddMetric(grid, AppText.Get("AppointmentsMetric"), snapshot.Appointments.Count(a => a.StartsAt >= DateTimeOffset.Now).ToString(AppText.Culture), 0, 1);
		AddMetric(grid, AppText.Get("RemindersMetric"), snapshot.Reminders.Count(r => r.IsEnabled).ToString(AppText.Culture), 1, 1);

		return grid;
	}

	private static Border CreateSummaryCard(HealthVaultSnapshot snapshot) =>
		Ui.Card(new VerticalStackLayout
		{
			Spacing = 10,
			Children =
			{
				Ui.SectionTitle(AppText.Get("QuickHealthSummary")),
				Ui.Body(AppText.Format("BloodTypeFormat", snapshot.Summary.BloodType)),
				Ui.Body(AppText.Format("AllergiesFormat", AppText.FormatList(snapshot.Summary.Allergies))),
				Ui.Muted(AppText.Format("LastUpdatedFormat", AppText.FormatDate(snapshot.Summary.LastUpdatedAt)))
			}
		});

	private static Border CreateNextCard(string title, string value, string detail) =>
		Ui.Card(new VerticalStackLayout
		{
			Spacing = 6,
			Children =
			{
				Ui.SectionTitle(title),
				Ui.Body(value),
				Ui.Muted(detail)
			}
		});

	private static Border CreateAiCard() =>
		Ui.Card(new VerticalStackLayout
		{
			Spacing = 8,
			Children =
			{
				new HorizontalStackLayout
				{
					Spacing = 10,
					Children =
					{
						Ui.SectionTitle(AppText.Get("LocalAiAssistant")),
						Ui.Badge(AppText.Get("Pending"), UiTone.Warning)
					}
				},
				Ui.Body(AppText.Get("LocalAiBody")),
				Ui.Muted(AppText.Get("NoCloudAi"))
			}
		});

	private static void AddMetric(Grid grid, string label, string value, int column, int row)
	{
		var card = Ui.Card(new VerticalStackLayout
		{
			Spacing = 4,
			Children =
			{
				Ui.Metric(value),
				Ui.Muted(label)
			}
		});

		Grid.SetColumn(card, column);
		Grid.SetRow(card, row);
		grid.Children.Add(card);
	}
}
