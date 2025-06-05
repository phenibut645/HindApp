using HindApp.Models;
using HindApp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HindApp.Views.Admin
{
    public partial class ProductsPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private List<Category> _categories;
        private Product _selectedProduct;
        private string _imagePath;

        public ProductsPage(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            LoadData();
        }

        private async void LoadData()
        {
            var products = await _dbService.GetAllProductsAsync();
            ProductsList.ItemsSource = products;

            _categories = await _dbService.GetAllCategoriesAsync();
            CategoryPicker.ItemsSource = _categories;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text;
            string desc = DescriptionEntry.Text;
            var selectedCategory = CategoryPicker.SelectedItem as Category;
            int? categoryId = selectedCategory?.Id;

            if (string.IsNullOrWhiteSpace(name) || categoryId == null)
            {
                await DisplayAlert("Viga", "Sisesta nimi ja vali kategooria", "OK");
                return;
            }

            if (_selectedProduct != null)
            {
                _selectedProduct.Name = name;
                _selectedProduct.Description = desc;
                _selectedProduct.CategoryId = categoryId;

                if (!string.IsNullOrEmpty(_imagePath))
                    _selectedProduct.ImagePath = _imagePath;

                await _dbService.UpdateProductAsync(_selectedProduct);
            }
            else
            {
                var newProduct = new Product
                {
                    Name = name,
                    Description = desc,
                    CategoryId = categoryId,
                    ImagePath = _imagePath
                };
                await _dbService.AddProductAsync(newProduct);
            }

            ClearForm();
            LoadData();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_selectedProduct != null)
            {
                bool confirm = await DisplayAlert("Kustutamine", $"Kustuta {_selectedProduct.Name}?", "Jah", "Ei");
                if (confirm)
                {
                    await _dbService.DeleteProductAsync(_selectedProduct);
                    ClearForm();
                    LoadData();
                }
            }
        }

        private void OnProductSelected(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = e.CurrentSelection.FirstOrDefault() as Product;
            if (_selectedProduct != null)
            {
                NameEntry.Text = _selectedProduct.Name;
                DescriptionEntry.Text = _selectedProduct.Description;

                if (_selectedProduct.CategoryId.HasValue)
                {
                    var category = _categories.FirstOrDefault(c => c.Id == _selectedProduct.CategoryId.Value);
                    CategoryPicker.SelectedItem = category;
                }

                // Показываем изображение, если есть
                if (!string.IsNullOrEmpty(_selectedProduct.ImagePath) && File.Exists(_selectedProduct.ImagePath))
                {
                    ProductImage.Source = ImageSource.FromFile(_selectedProduct.ImagePath);
                    _imagePath = _selectedProduct.ImagePath;
                }
                else
                {
                    ProductImage.Source = null;
                    _imagePath = null;
                }
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedProduct = null;
            NameEntry.Text = "";
            DescriptionEntry.Text = "";
            CategoryPicker.SelectedItem = null;
            ProductsList.SelectedItem = null;
            ProductImage.Source = null;
            _imagePath = null;
        }

        private async void OnPickImageClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Vali pilt"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
                    var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                    File.WriteAllBytes(filePath, bytes);

                    _imagePath = filePath;
                    ProductImage.Source = ImageSource.FromFile(_imagePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Viga", $"Pildi valimine ebaõnnestus: {ex.Message}", "OK");
            }
        }
    }
}
