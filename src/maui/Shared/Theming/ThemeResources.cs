namespace SelfCustodyHealth.Shared.Theming;

internal static class ThemeResources
{
	public static Color Color(string key) =>
		(Color)Application.Current!.Resources[key];

	public static Style Style(string key) =>
		(Style)Application.Current!.Resources[key];

	public static Thickness Thickness(string key) =>
		(Thickness)Application.Current!.Resources[key];

	public static void ApplyPageBackground(VisualElement element) =>
		element.SetAppThemeColor(
			VisualElement.BackgroundColorProperty,
			Color("AppPageBackgroundLight"),
			Color("AppPageBackgroundDark"));

	public static void ApplyShellChrome(BindableObject element)
	{
		element.SetAppThemeColor(
			Shell.BackgroundColorProperty,
			Color("AppSurfaceLight"),
			Color("AppPageBackgroundDark"));
		element.SetAppThemeColor(
			Shell.ForegroundColorProperty,
			Color("AppTextPrimaryLight"),
			Color("AppTextPrimaryDark"));
		element.SetAppThemeColor(
			Shell.TitleColorProperty,
			Color("AppTextPrimaryLight"),
			Color("AppTextPrimaryDark"));
		element.SetAppThemeColor(
			Shell.DisabledColorProperty,
			Color("AppTextMutedLight"),
			Color("AppTextMutedDark"));
		element.SetAppThemeColor(
			Shell.UnselectedColorProperty,
			Color("AppTextMutedLight"),
			Color("AppTextMutedDark"));
		element.SetAppThemeColor(
			Shell.FlyoutBackgroundColorProperty,
			Color("AppSurfaceLight"),
			Color("AppSurfaceDark"));
	}

	public static void ApplyTabBarChrome(BindableObject element)
	{
		element.SetValue(Shell.TabBarForegroundColorProperty, Color("AppPrimary"));
		element.SetValue(Shell.TabBarTitleColorProperty, Color("AppPrimary"));
		element.SetAppThemeColor(
			Shell.TabBarBackgroundColorProperty,
			Color("AppSurfaceLight"),
			Color("AppSurfaceDark"));
		element.SetAppThemeColor(
			Shell.TabBarUnselectedColorProperty,
			Color("AppTextMutedLight"),
			Color("AppTextMutedDark"));
	}
}
