using PersonalHealthVault.Shared.Theming;

namespace PersonalHealthVault.Shared.Ui;

internal enum UiTone
{
	Primary,
	Info,
	Success,
	Warning,
	Danger,
	Muted
}

internal static class Ui
{
	public static Label PageTitle(string text) =>
		StyledLabel(text, "PageTitleLabel");

	public static Label SectionTitle(string text) =>
		StyledLabel(text, "SectionTitleLabel");

	public static Label Body(string text) =>
		StyledLabel(text, "BodyTextLabel");

	public static Label Muted(string text) =>
		StyledLabel(text, "MutedTextLabel");

	public static Label Metric(string text) =>
		StyledLabel(text, "MetricValueLabel");

	public static Border Card(View content)
	{
		var border = new Border
		{
			Content = content
		};
		ApplyStyle(border, "HealthCardBorder");
		return border;
	}

	public static Border Badge(string text, UiTone tone = UiTone.Primary)
	{
		var label = new Label
		{
			Text = text,
			FontAttributes = FontAttributes.Bold,
			FontSize = 12,
			TextColor = ThemeResources.Color("AppOnStatusText")
		};

		var border = new Border
		{
			Content = label,
			BackgroundColor = ThemeResources.Color(GetToneColorKey(tone))
		};
		ApplyStyle(border, "StatusBadgeBorder");
		return border;
	}

	public static Border Row(View content)
	{
		var border = new Border
		{
			Content = content
		};
		ApplyStyle(border, "DocumentRowBorder");
		return border;
	}

	public static Button SecondaryButton(string text) =>
		new()
		{
			Text = text,
			Style = GetStyle("SecondaryButton")
		};

	public static Button DestructiveButton(string text) =>
		new()
		{
			Text = text,
			Style = GetStyle("DestructiveSecondaryButton")
		};

	public static VerticalStackLayout PageStack(params View[] children)
	{
		var stack = new VerticalStackLayout
		{
			Padding = ThemeResources.Thickness("PagePadding"),
			Spacing = 16
		};

		foreach (var child in children)
		{
			stack.Children.Add(child);
		}

		return stack;
	}

	public static ScrollView Scroll(View content)
	{
		var scrollView = new ScrollView
		{
			Content = content,
			VerticalScrollBarVisibility = ScrollBarVisibility.Never
		};
		ThemeResources.ApplyPageBackground(scrollView);
		return scrollView;
	}

	public static Border SoftContainer(View content)
	{
		var border = new Border
		{
			Content = content
		};
		ApplyStyle(border, "SoftContainerBorder");
		return border;
	}

	private static Label StyledLabel(string text, string styleKey) =>
		new()
		{
			Text = text,
			Style = GetStyle(styleKey)
		};

	private static void ApplyStyle(VisualElement element, string styleKey) =>
		element.Style = GetStyle(styleKey);

	private static Style GetStyle(string styleKey) =>
		ThemeResources.Style(styleKey);

	private static string GetToneColorKey(UiTone tone) =>
		tone switch
		{
			UiTone.Info => "AppInfo",
			UiTone.Success => "AppSuccess",
			UiTone.Warning => "AppWarning",
			UiTone.Danger => "AppDanger",
			UiTone.Muted => "AppMuted",
			_ => "AppPrimary"
		};
}
