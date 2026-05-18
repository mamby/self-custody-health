using SelfCustodyHealth.Shared.Localization;

namespace SelfCustodyHealth.Security;

internal static class DeviceUnlockText
{
	public static string Availability(DeviceUnlockAvailability availability) =>
		availability switch
		{
			DeviceUnlockAvailability.Available => AppText.Get("DeviceUnlockAvailabilityAvailable"),
			DeviceUnlockAvailability.NotEnrolled => AppText.Get("DeviceUnlockAvailabilityNotEnrolled"),
			DeviceUnlockAvailability.DisabledByPolicy => AppText.Get("DeviceUnlockAvailabilityDisabledByPolicy"),
			DeviceUnlockAvailability.LockedOut => AppText.Get("DeviceUnlockAvailabilityLockedOut"),
			_ => AppText.Get("DeviceUnlockAvailabilityUnavailable")
		};
}
