using Android.App;
using Android.Content.PM;
using Android.OS;

namespace PersonalHealthVault;

[Activity(Label = "@string/app_name", Theme = "@style/PersonalHealthVaultTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
