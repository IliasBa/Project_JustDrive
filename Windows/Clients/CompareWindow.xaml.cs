using Project_JustDrive.Models;
using Project_JustDrive.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_JustDrive.Windows.Clients
{
    public partial class CompareWindow : Window
    {
        private List<int> _carIds;

        public CompareWindow(List<int> carIds)
        {
            InitializeComponent();
            _carIds = carIds;
            LoadComparison();
        }

        private void LoadComparison()
        {
            CarService service = new CarService();
            var cars = new List<CarCompareModel>();

            foreach (var id in _carIds)
            {
                var car = service.GetCarById(id);
                if (car != null) cars.Add(car);
            }

            BuildCompareGrid(cars);
        }

        private void BuildCompareGrid(List<CarCompareModel> cars)
        {
            var grid = CompareGrid;
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            grid.Children.Clear();

            // Label column + one column per car
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
            foreach (var car in cars)
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            // Rows
            var labels = new[]
            {
                "Auto", "Prijs / dag", "Borg", "Brandstof",
                "Transmissie", "Type", "Verbruik", "Locatie"
            };

            foreach (var label in labels)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(52) });

            // Header row — car names
            AddCell(grid, "", 0, 0, true);
            for (int i = 0; i < cars.Count; i++)
                AddCell(grid, $"{cars[i].CarBrand} {cars[i].Model}", 0, i + 1, true);

            // Data rows
            var rows = new[]
            {
                cars.ConvertAll(c => $"{c.CarBrand} {c.Model}"),
                cars.ConvertAll(c => $"€ {c.PricePerDay}"),
                cars.ConvertAll(c => $"€ {c.Deposit}"),
                cars.ConvertAll(c => c.Fuel),
                cars.ConvertAll(c => c.Transmission),
                cars.ConvertAll(c => c.Type),
                cars.ConvertAll(c => $"{c.PricePerKm} L/100km"),
                cars.ConvertAll(c => c.City)
            };

            // Find cheapest car index
            int cheapestIndex = 0;
            decimal lowestPrice = cars[0].PricePerDay;
            for (int i = 1; i < cars.Count; i++)
            {
                if (cars[i].PricePerDay < lowestPrice)
                {
                    lowestPrice = cars[i].PricePerDay;
                    cheapestIndex = i;
                }
            }

            for (int row = 0; row < labels.Length; row++)
            {
                AddLabel(grid, labels[row], row, 0);

                for (int col = 0; col < cars.Count; col++)
                {
                    // Highlight cheapest price in green
                    bool isGreen = row == 1 && col == cheapestIndex;
                    AddCell(grid, rows[row][col], row, col + 1, false, row % 2 == 0, isGreen);
                }
            }

            // Add button row
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
            int buttonRow = labels.Length;

            AddLabel(grid, "", buttonRow, 0);

            for (int col = 0; col < cars.Count; col++)
            {
                var car = cars[col];
                var button = new Button
                {
                    Content = "Huur nu",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F26B1F")),
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.SemiBold,
                    Padding = new Thickness(16, 8, 16, 8),
                    Cursor = Cursors.Hand,
                    Tag = car.Id
                };

                button.Click += (s, e) =>
                {
                    int carId = (int)((Button)s).Tag;
                    CarDetail detail = new CarDetail(carId, Session.CurrentUser.UserId);
                    detail.Show();
                    this.Close();
                };

                var border = new Border { Padding = new Thickness(8), VerticalAlignment = VerticalAlignment.Center };
                border.Child = button;
                Grid.SetRow(border, buttonRow);
                Grid.SetColumn(border, col + 1);
                grid.Children.Add(border);
            }
        }

        private void AddLabel(Grid grid, string text, int row, int col)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 13
            };
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            grid.Children.Add(tb);
        }

        private void AddCell(Grid grid, string text, int row, int col, bool isHeader, bool shaded = false, bool highlight = false)
        {
            var border = new Border
            {
                Background = shaded
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F9FAFB"))
                    : Brushes.White,
                Padding = new Thickness(8)
            };

            var tb = new TextBlock
            {
                Text = text,
                FontSize = isHeader ? 15 : 13,
                FontWeight = highlight ? FontWeights.Bold : (isHeader ? FontWeights.Bold : FontWeights.Normal),
                Foreground = highlight
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F1B2D")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = tb;
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            grid.Children.Add(border);
        }

        private void Terug_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
