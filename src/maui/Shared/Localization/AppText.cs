using System.Globalization;
using System.Resources;

namespace SelfCustodyHealth.Shared.Localization;

internal static class AppText
{
	private static readonly ResourceManager Resources = new(
		"SelfCustodyHealth.Resources.Localization.AppResources",
		typeof(AppText).Assembly);

	public static string Get(string name) =>
		Resources.GetString(name, CultureInfo.CurrentUICulture) ?? name;

	public static CultureInfo Culture => CultureInfo.CurrentCulture;

	public static string Format(string name, params object?[] args) =>
		string.Format(CultureInfo.CurrentCulture, Get(name), args);

	public static string FormatDate(DateOnly date) =>
		date.ToString("d", CultureInfo.CurrentCulture);

	public static string FormatDate(DateTimeOffset dateTime) =>
		dateTime.ToString("d", CultureInfo.CurrentCulture);

	public static string FormatDateTime(DateTimeOffset dateTime) =>
		dateTime.ToString("g", CultureInfo.CurrentCulture);

	public static string FormatTime(TimeOnly time) =>
		time.ToString("t", CultureInfo.CurrentCulture);

	public static string FormatList(IEnumerable<string> values) =>
		string.Join(Get("ListSeparator"), values);
}
