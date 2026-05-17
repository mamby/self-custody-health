using SelfCustodyHealth.Domain;
using SelfCustodyHealth.Shared.Localization;

namespace SelfCustodyHealth.Shared;

internal static class HealthText
{
	public static string CategoryName(DocumentCategory category) =>
		category switch
		{
			DocumentCategory.All => AppText.Get("CategoryAll"),
			DocumentCategory.LabResults => AppText.Get("CategoryLabResults"),
			DocumentCategory.Prescriptions => AppText.Get("CategoryPrescriptions"),
			DocumentCategory.Reports => AppText.Get("CategoryReports"),
			DocumentCategory.Vaccinations => AppText.Get("CategoryVaccinations"),
			DocumentCategory.Other => AppText.Get("CategoryOther"),
			_ => AppText.Get("CategoryOther")
		};

	public static string RecurrenceName(ReminderRecurrence recurrence) =>
		recurrence switch
		{
			ReminderRecurrence.None => AppText.Get("RecurrenceOnce"),
			ReminderRecurrence.Daily => AppText.Get("RecurrenceDaily"),
			ReminderRecurrence.Weekly => AppText.Get("RecurrenceWeekly"),
			ReminderRecurrence.Monthly => AppText.Get("RecurrenceMonthly"),
			_ => AppText.Get("RecurrenceScheduled")
		};

	public static string FormatDate(DateOnly date) =>
		AppText.FormatDate(date);

	public static string FormatDateTime(DateTimeOffset dateTime) =>
		AppText.FormatDateTime(dateTime);
}
