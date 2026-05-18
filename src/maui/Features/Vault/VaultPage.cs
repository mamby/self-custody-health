using PersonalHealthVault.Domain;
using PersonalHealthVault.Shared;
using PersonalHealthVault.Shared.Localization;
using PersonalHealthVault.Shared.Sheets;
using PersonalHealthVault.Shared.Theming;
using PersonalHealthVault.Shared.Ui;
using PersonalHealthVault.Storage;

namespace PersonalHealthVault.Features.Vault;

public sealed class VaultPage(IDocumentStore documentStore, HealthDataService dataService) : ThemedContentPage
{
	private readonly BottomSheetView _detailsSheet = new();
	private readonly VerticalStackLayout _documentList = new() { Spacing = 10 };
	private readonly SearchBar _searchBar = new();
	private DocumentCategory _selectedCategory = DocumentCategory.All;
	private bool _searchHandlerAttached;

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		Build();
		await LoadDocumentsAsync();
	}

	public void Build()
	{
		_searchBar.Placeholder = AppText.Get("SearchLocalDocuments");
		if (!_searchHandlerAttached)
		{
			_searchBar.TextChanged += async (_, _) => await LoadDocumentsAsync();
			_searchHandlerAttached = true;
		}

		var content = Ui.PageStack(
			Ui.PageTitle(AppText.Get("VaultTitle")),
			Ui.Body(AppText.Get("VaultIntro")),
			_searchBar,
			CreateCategoryBar(),
			_documentList,
			Ui.Card(new VerticalStackLayout
			{
				Spacing = 8,
				Children =
				{
					Ui.SectionTitle(AppText.Get("AddDocument")),
					Ui.Body(AppText.Get("AddDocumentBody")),
					Ui.Muted(AppText.Get("NoPlaintextSensitiveData"))
				}
			}));

		Content = new Grid
		{
			Children =
			{
				Ui.Scroll(content),
				_detailsSheet
			}
		};
	}

	private ScrollView CreateCategoryBar()
	{
		var row = new HorizontalStackLayout { Spacing = 8 };
		foreach (var category in Enum.GetValues<DocumentCategory>())
		{
			var button = Ui.SecondaryButton(HealthText.CategoryName(category));
			button.Clicked += async (_, _) =>
			{
				_selectedCategory = category;
				await LoadDocumentsAsync();
			};
			row.Children.Add(button);
		}

		return new ScrollView
		{
			Orientation = ScrollOrientation.Horizontal,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
			Content = row
		};
	}

	private async Task LoadDocumentsAsync()
	{
		if (Content is null)
		{
			Build();
		}

		var documents = await documentStore.SearchAsync(_selectedCategory, _searchBar.Text, CancellationToken.None);
		_documentList.Children.Clear();

		var snapshot = await dataService.GetSnapshotAsync();
		if (dataService.IsDemoData)
		{
			_documentList.Children.Add(Ui.SoftContainer(Ui.Muted(AppText.Get("DemoDataOnlyVaultCreated"))));
		}

		foreach (var document in documents)
		{
			_documentList.Children.Add(CreateDocumentRow(document));
		}

		if (documents.Count is 0)
		{
			_documentList.Children.Add(Ui.Card(Ui.Body(AppText.Get("NoDocumentsMatchFilter"))));
		}
	}

	private Border CreateDocumentRow(MedicalDocument document)
	{
		var badge = Ui.Badge(document.IsDemo ? AppText.Get("BadgeDemo") : AppText.Get("BadgeLocal"), document.IsDemo ? UiTone.Warning : UiTone.Success);
		var grid = new Grid
		{
			ColumnDefinitions =
			{
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(GridLength.Auto)
			},
			ColumnSpacing = 12,
			Children =
			{
				new VerticalStackLayout
				{
					Spacing = 4,
					Children =
					{
						Ui.SectionTitle(document.Title),
						Ui.Muted($"{HealthText.CategoryName(document.Category)} / {HealthText.FormatDate(document.DocumentDate)}"),
						Ui.Muted(document.Source)
					}
				},
				badge
			}
		};

		Grid.SetColumn(badge, 1);
		var row = Ui.Row(grid);

		row.GestureRecognizers.Add(new TapGestureRecognizer
		{
			Command = new Command(async () => await ShowDocumentAsync(document))
		});

		return row;
	}

	private async Task ShowDocumentAsync(MedicalDocument document)
	{
		var content = new VerticalStackLayout
		{
			Spacing = 14,
			Children =
			{
				Ui.Muted(HealthText.CategoryName(document.Category).ToUpper(AppText.Culture)),
				Ui.PageTitle(document.Title),
				Ui.Body(document.Notes ?? AppText.Get("NoNotes")),
				Ui.Muted(AppText.Format("SourceLabelFormat", document.Source)),
				Ui.Muted(AppText.Format("DateLabelFormat", HealthText.FormatDate(document.DocumentDate))),
				Ui.SoftContainer(Ui.Muted(AppText.Get("RecordAdviceDisclaimer")))
			}
		};

		await _detailsSheet.ShowAsync(content);
	}
}
