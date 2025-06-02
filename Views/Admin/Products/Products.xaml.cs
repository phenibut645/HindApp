using HindApp.Models;
using HindApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HindApp.Views.Admin
{
    public partial class ProductsPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private List<Category> _categories;
        private Product _selectedProduct;

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
                await DisplayAlert("Viga", "Sisesta nimi ja vali kategooria", "ÎÊ");
                return;
            }

            if (_selectedProduct != null)
            {
                _selectedProduct.Name = name;
                _selectedProduct.Description = desc;
                _selectedProduct.CategoryId = categoryId;
                await _dbService.UpdateProductAsync(_selectedProduct);
            }
            else
            {
                var newProduct = new Product
                {
                    Name = name,
                    Description = desc,
                    CategoryId = categoryId
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
            }
        }

        private void ClearForm()
        {
            _selectedProduct = null;
            NameEntry.Text = "";
            DescriptionEntry.Text = "";
            CategoryPicker.SelectedItem = null;
            ProductsList.SelectedItem = null;
        }
        private void OnClearClicked(object sender, EventArgs e)
        {
            ClearForm();
        }

    }
}
