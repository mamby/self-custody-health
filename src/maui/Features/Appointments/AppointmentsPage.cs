using SelfCustodyHealth.Domain;
using SelfCustodyHealth.Shared;
using SelfCustodyHealth.Shared.Localization;
using SelfCustodyHealth.Shared.Theming;
using SelfCustodyHealth.Shared.Ui;
using SelfCustodyHealth.Storage;

namespace SelfCustodyHealth.Features.Appointments;

public sealed class AppointmentsPage(HealthDataService dataService) : ThemedContentPage
{
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		var snapshot = await dataService.GetSnapshotAsync();
		var now = DateTimeOffset.Now;
		var upcoming = snapshot.Appointments.Where(appointment => appointment.StartsAt >= now).OrderBy(appointment => appointment.StartsAt).ToArray();
		var past = snapshot.Appointments.Where(appointment => appointment.StartsAt < now).OrderByDescending(appointment => appointment.StartsAt).ToArray();

		var stack = Ui.PageStack(
			Ui.PageTitle(AppText.Get("AppointmentsTitle")),
			Ui.Body(AppText.Get("AppointmentsIntro")),
			Ui.SectionTitle(AppText.Get("Upcoming")));

		AddAppointments(stack, upcoming, AppText.Get("NoUpcomingAppointments"));
		stack.Children.Add(Ui.SectionTitle(AppText.Get("Past")));
		AddAppointments(stack, past, AppText.Get("NoPastAppointments"));

		Content = Ui.Scroll(stack);
	}

	private static void AddAppointments(VerticalStackLayout stack, IReadOnlyList<Appointment> appointments, string emptyText)
	{
		if (appointments.Count is 0)
		{
			stack.Children.Add(Ui.Card(Ui.Muted(emptyText)));
			return;
		}

		foreach (var appointment in appointments)
		{
			stack.Children.Add(Ui.Card(new VerticalStackLayout
			{
				Spacing = 8,
				Children =
				{
					Ui.SectionTitle(appointment.Title),
					Ui.Body(appointment.Clinician),
					Ui.Muted(HealthText.FormatDateTime(appointment.StartsAt)),
					Ui.Muted(appointment.Location),
					Ui.Muted(GetRelatedDocumentsText(appointment.RelatedDocumentIds.Count))
				}
			}));
		}
	}

	private static string GetRelatedDocumentsText(int count) =>
		count switch
		{
			0 => AppText.Get("NoRelatedDocuments"),
			1 => AppText.Get("RelatedDocumentOne"),
			_ => AppText.Format("RelatedDocumentsManyFormat", count.ToString(AppText.Culture))
		};
}
