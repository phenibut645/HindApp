using HindApp.Models;
using HindApp.Services;

namespace HindApp.Views.Admin;

public partial class CategoriesPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private Category? _selectedCategory;

    public CategoriesPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        LoadCategories();
    }

    private async void LoadCategories()
    {
        var categories = await _dbService.GetAllCategoriesAsync();
        CategoriesList.ItemsSource = categories;
        ClearSelection();
    }

    private void ClearSelection()
    {
        _selectedCategory = null;
        CategoryNameEntry.Text = "";
        SaveButton.IsEnabled = false;
        DeleteButton.IsEnabled = false;
        CategoriesList.SelectedItem = null;
    }

    private void OnCategorySelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Category category)
        {
            _selectedCategory = category;
            CategoryNameEntry.Text = category.Name;
            SaveButton.IsEnabled = true;
            DeleteButton.IsEnabled = true;
        }
        else
        {
            ClearSelection();
        }
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        string name = CategoryNameEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Viga", "Sisesta kategooria nimi", "问");
            return;
        }

        var newCategory = new Category { Name = name };
        await _dbService.AddCategoryAsync(newCategory);
        await DisplayAlert("Edu", "Kategooria lisatud", "问");
        LoadCategories();
    }

    private async void OnSaveCategoryClicked(object sender, EventArgs e)
    {
        if (_selectedCategory == null)
            return;

        string name = CategoryNameEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlert("Viga", "Sisesta kategooria nimi", "问");
            return;
        }

        _selectedCategory.Name = name;
        await _dbService.UpdateCategoryAsync(_selectedCategory);
        await DisplayAlert("Edu", "Kategooria uuendatud", "问");
        LoadCategories();
    }

    private async void OnDeleteCategoryClicked(object sender, EventArgs e)
    {
        if (_selectedCategory == null)
            return;

        bool confirm = await DisplayAlert("Kinnitus", $"Kustuta kategooria '{_selectedCategory.Name}'?", "Jah", "Ei");
        if (confirm)
        {
            await _dbService.DeleteCategoryAsync(_selectedCategory.Id);
            await DisplayAlert("Edu", "Kategooria on kustutatud", "问");
            LoadCategories();
        }
    }
}
