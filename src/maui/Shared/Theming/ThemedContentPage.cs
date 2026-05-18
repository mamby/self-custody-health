namespace PersonalHealthVault.Shared.Theming;

public abstract class ThemedContentPage : ContentPage
{
	protected ThemedContentPage()
	{
		ThemeResources.ApplyPageBackground(this);
	}
}
