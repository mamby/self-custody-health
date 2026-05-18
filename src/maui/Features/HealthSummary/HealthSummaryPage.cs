using PersonalHealthVault.Shared;
using PersonalHealthVault.Shared.Localization;
using PersonalHealthVault.Shared.Theming;
using PersonalHealthVault.Shared.Ui;
using PersonalHealthVault.Storage;

namespace PersonalHealthVault.Features.HealthSummary;

public sealed class HealthSummaryPage(HealthDataService dataService) : ThemedContentPage
{
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		var snapshot = await dataService.GetSnapshotAsync();
		var summary = snapshot.Summary;

		var contacts = new VerticalStackLayout
		{
			Spacing = 10,
			Children = { Ui.SectionTitle(AppText.Get("EmergencyContacts")) }
		};

		foreach (var contact in summary.EmergencyContacts)
		{
			contacts.Children.Add(Ui.Body(AppText.Format("ContactSummaryFormat", contact.Name, contact.Relationship, contact.PhoneNumber)));
		}

		Content = Ui.Scroll(Ui.PageStack(
			Ui.PageTitle(AppText.Get("HealthSummaryTitle")),
			Ui.Body(AppText.Get("HealthSummaryIntro")),
			Ui.Card(CreateList(AppText.Get("BloodType"), [summary.BloodType])),
			Ui.Card(CreateList(AppText.Get("Allergies"), summary.Allergies)),
			Ui.Card(CreateList(AppText.Get("ChronicConditions"), summary.ChronicConditions)),
			Ui.Card(CreateList(AppText.Get("Surgeries"), summary.Surgeries)),
			Ui.Card(contacts),
			Ui.Muted(AppText.Format("LastUpdatedFormat", AppText.FormatDate(summary.LastUpdatedAt)))));
	}

	private static VerticalStackLayout CreateList(string title, IReadOnlyList<string> items)
	{
		var stack = new VerticalStackLayout
		{
			Spacing = 8,
			Children = { Ui.SectionTitle(title) }
		};

		foreach (var item in items.DefaultIfEmpty(AppText.Get("NotSet")))
		{
			stack.Children.Add(Ui.Body(item));
		}

		return stack;
	}
}
