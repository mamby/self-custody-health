using System.Text.RegularExpressions;

namespace SelfCustodyHealth.Tests;

public sealed partial class UiThemeArchitectureTests
{
	private static readonly string[] ForbiddenCSharpColorMarkers =
	[
		"Color.FromArgb(",
		"Color.FromRgba(",
		"Colors."
	];

	[Fact]
	public void ProductionUi_DoesNotUseHardcodedColorsOutsideThemePalette()
	{
		var repoRoot = FindRepoRoot();
		var sourceRoot = Path.Combine(repoRoot, "src", "SelfCustodyHealth");
		string[] checkedRoots =
		[
			Path.Combine(sourceRoot, "Features"),
			Path.Combine(sourceRoot, "Shared"),
			Path.Combine(sourceRoot, "Resources", "Styles")
		];

		var violations = checkedRoots
			.SelectMany(EnumerateSourceFiles)
			.Concat(new[] { Path.Combine(sourceRoot, "AppShell.xaml") })
			.SelectMany(file => FindViolations(repoRoot, file))
			.ToArray();

		Assert.True(
			violations.Length is 0,
			"Raw UI colors must be centralized in Resources/Styles/Colors.xaml and consumed through theme resources or shared UI helpers."
			+ Environment.NewLine
			+ string.Join(Environment.NewLine, violations));
	}

	[Fact]
	public void FeaturePages_UseThemedContentPage()
	{
		var repoRoot = FindRepoRoot();
		var featuresRoot = Path.Combine(repoRoot, "src", "SelfCustodyHealth", "Features");
		var violations = Directory
			.EnumerateFiles(featuresRoot, "*Page.cs", SearchOption.AllDirectories)
			.Where(file => DirectContentPageInheritanceRegex().IsMatch(File.ReadAllText(file)))
			.Select(file => Path.GetRelativePath(repoRoot, file))
			.ToArray();

		Assert.True(
			violations.Length is 0,
			"Feature pages must inherit ThemedContentPage so modal and Shell-hosted pages receive the same runtime theme background."
			+ Environment.NewLine
			+ string.Join(Environment.NewLine, violations));
	}

	private static IEnumerable<string> EnumerateSourceFiles(string root) =>
		Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories)
			.Where(file => file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
				|| file.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
			.Where(file => !file.EndsWith(Path.Combine("Resources", "Styles", "Colors.xaml"), StringComparison.OrdinalIgnoreCase));

	private static IEnumerable<string> FindViolations(string repoRoot, string file)
	{
		var lineNumber = 0;
		foreach (var line in File.ReadLines(file))
		{
			lineNumber++;
			if (IsHardcodedColor(line, file))
			{
				yield return $"{Path.GetRelativePath(repoRoot, file)}:{lineNumber}: {line.Trim()}";
			}
		}
	}

	private static bool IsHardcodedColor(string line, string file) =>
		file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
			? ForbiddenCSharpColorMarkers.Any(marker => line.Contains(marker, StringComparison.Ordinal))
			: HexColorRegex().IsMatch(line) || NamedColorAttributeRegex().IsMatch(line);

	private static string FindRepoRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null)
		{
			if (File.Exists(Path.Combine(directory.FullName, "SelfCustodyHealth.sln")))
			{
				return directory.FullName;
			}

			directory = directory.Parent;
		}

		throw new DirectoryNotFoundException("Could not locate the repository root.");
	}

	[GeneratedRegex("#[0-9A-Fa-f]{3,8}", RegexOptions.CultureInvariant)]
	private static partial Regex HexColorRegex();

	[GeneratedRegex("\\b(?:BackgroundColor|TextColor|Stroke|Color|TitleColor|PlaceholderColor|CancelButtonColor)\\s*=\\s*\"(?:White|Black|Red|Green|Blue|Yellow|Purple|Gray|Grey|Transparent)\"", RegexOptions.CultureInvariant)]
	private static partial Regex NamedColorAttributeRegex();

	[GeneratedRegex(":\\s*ContentPage\\b", RegexOptions.CultureInvariant)]
	private static partial Regex DirectContentPageInheritanceRegex();
}
