using SelfCustodyHealth.Shared;
using SelfCustodyHealth.Shared.Localization;
using SelfCustodyHealth.Shared.Theming;
using SelfCustodyHealth.Shared.Ui;
using SelfCustodyHealth.Storage;

namespace SelfCustodyHealth.Features.Medications;

public sealed class MedicationsPage(HealthDataService dataService) : ThemedContentPage
{
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		var snapshot = await dataService.GetSnapshotAsync();
		var stack = Ui.PageStack(
			Ui.PageTitle(AppText.Get("MedicationsTitle")),
			Ui.Body(AppText.Get("MedicationsIntro")));

		foreach (var medication in snapshot.Medications.OrderByDescending(m => m.IsActive).ThenBy(m => m.Name))
		{
			stack.Children.Add(Ui.Card(new VerticalStackLayout
			{
				Spacing = 8,
				Children =
				{
					new HorizontalStackLayout
					{
						Spacing = 10,
						Children =
						{
							Ui.SectionTitle(medication.Name),
							Ui.Badge(medication.IsActive ? AppText.Get("Active") : AppText.Get("Inactive"), medication.IsActive ? UiTone.Success : UiTone.Muted)
						}
					},
					Ui.Body(medication.Dose),
					Ui.Muted(medication.Instructions),
					Ui.Muted(AppText.Format(
						"ReminderScheduleAtFormat",
						HealthText.RecurrenceName(medication.Schedule.Recurrence),
						AppText.FormatTime(medication.Schedule.TimeOfDay)))
				}
			}));
		}

		Content = Ui.Scroll(stack);
	}
}
