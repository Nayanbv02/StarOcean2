using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using StarOcean2;
using System.IO;

namespace StarOcean2
{

	public partial class MainWindow : Window
	{

        public uint ID { get; set; }
        private uint _selectedItemId = 0;
        private readonly Dictionary<uint, string> _catById = new();
        private List<string> _categories = new();


        public MainWindow()
		{
			InitializeComponent();
            LoadCategoriesOnce();
            BindCategoryCombos();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateItemList("");
            RefreshFinderFilter();
            foreach (var item in ListBoxItem.Items)
            {
                NameValueInfo info = item as NameValueInfo;
                if (info.Value == ID)
                {
                    ListBoxItem.SelectedItem = item;
                    ListBoxItem.ScrollIntoView(item);
                    break;
                }
            }
        }

        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(ListBoxItem.ItemsSource);
            if (view == null) return;

            var needle = (TextBoxFilter.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(needle))
            {
                view.Filter = null;
                view.Refresh();
                return;
            }

            view.Filter = o =>
            {
                if (o is NameValueInfo nvi && !string.IsNullOrEmpty(nvi.Name))
                    return nvi.Name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
                return false;
            };
            view.Refresh();
            RefreshFinderFilter();
        }

        private void ListBoxItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var nvi = ListBoxItem.SelectedItem as NameValueInfo;
            _selectedItemId = (nvi != null) ? (uint)nvi.Value : 0;
            ButtonDecision.IsEnabled = _selectedItemId != 0;
        }

        private async void ButtonDecision_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ViewModel;
            vm?.ApplyItemChoice(_selectedItemId);

            AddOkTick.Visibility = Visibility.Visible;
            await Task.Delay(1500);
            AddOkTick.Visibility = Visibility.Collapsed;
        }

        private void CreateItemList(string filter)
        {
            ListBoxItem.Items.Clear();
            List<NameValueInfo> items = Info.Instance().Item;

            foreach (var item in items)
            {
                if (string.Equals(item.Name, "???", StringComparison.Ordinal))
                    continue;

                if (string.IsNullOrEmpty(filter) ||
                    item.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ListBoxItem.Items.Add(item);
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void InventoryFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(ListBoxInventory.ItemsSource);
            if (view == null) return;

            var text = InventoryFilterBox.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                view.Filter = null;
                view.Refresh();
                return;
            }

            var converter = (ItemIDConverter)ListBoxInventory.Resources["ItemIDConverter"];
            var culture = CultureInfo.CurrentUICulture;
            var needle = text.Trim();

            view.Filter = o =>
            {
                if (o == null) return false;

                var idProp = o.GetType().GetProperty("ID");
                if (idProp == null) return false;

                var idValue = idProp.GetValue(o);
                var nameObj = converter.Convert(idValue, typeof(string), null, culture);
                var name = nameObj?.ToString() ?? string.Empty;

                return name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
            };

            view.Refresh();
        }

        private void LoadCategoriesOnce()
        {
            _catById.Clear();
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "info", "item.txt");
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('\t');
                if (parts.Length < 3) continue;

                if (uint.TryParse(parts[0], out var id))
                {
                    var category = parts[2].Trim();
                    _catById[id] = category;
                    set.Add(category);
                }
            }

            _categories = set.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
        }


        private void BindCategoryCombos()
        {
            var src = new List<string> { "All" };
            src.AddRange(_categories);

            if (InventoryCategoryBox != null)
            {
                InventoryCategoryBox.ItemsSource = src;
                InventoryCategoryBox.SelectedIndex = 0;
            }
            if (FinderCategoryBox != null)
            {
                FinderCategoryBox.ItemsSource = src;
                FinderCategoryBox.SelectedIndex = 0;
            }
        }

        private void InventoryCategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInventoryFilter();
        }

        private void RefreshInventoryFilter()
        {
            var view = CollectionViewSource.GetDefaultView(ListBoxInventory.ItemsSource);
            if (view == null) return;

            var selectedCat = InventoryCategoryBox?.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedCat) || selectedCat == "All")
            {
                view.Filter = null;
                view.Refresh();
                return;
            }

            view.Filter = o =>
            {
                if (o == null) return false;

                var idProp = o.GetType().GetProperty("ID");
                if (idProp == null) return true;

                var idVal = idProp.GetValue(o);
                if (idVal == null) return true;

                uint id = Convert.ToUInt32(idVal);
                return _catById.TryGetValue(id, out var cat) &&
                       string.Equals(cat, selectedCat, StringComparison.OrdinalIgnoreCase);
            };

            view.Refresh();
        }

        private void FinderCategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshFinderFilter();
        }

        private void RefreshFinderFilter()
        {
            var view = CollectionViewSource.GetDefaultView(
                (object)ListBoxItem.ItemsSource ?? ListBoxItem.Items);
            if (view == null) return;

            var needle = (TextBoxFilter?.Text ?? string.Empty).Trim();
            var selectedCat = FinderCategoryBox?.SelectedItem as string;

            view.Filter = o =>
            {
                if (o is not NameValueInfo nvi) return false;
                if (IsUnknownName(nvi.Name)) return false;

                if (!string.IsNullOrEmpty(needle))
                {
                    if (string.IsNullOrEmpty(nvi.Name) ||
                        nvi.Name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) < 0)
                        return false;
                }

                if (!string.IsNullOrEmpty(selectedCat) && selectedCat != "All")
                {
                    uint id = Convert.ToUInt32(nvi.Value);
                    return _catById.TryGetValue(id, out var cat) &&
                           string.Equals(cat, selectedCat, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            };

            view.Refresh();
        }

        private void AllClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "All items in your inventory will be deleted. Do you wish to continue?",
                "Clear confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            if (DataContext is ViewModel vm && vm.AllItemClearCommand != null)
            {
                if (vm.AllItemClearCommand.CanExecute(null))
                    vm.AllItemClearCommand.Execute(null);
            }
        }

        private static bool IsUnknownName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            foreach (var ch in name)
                if (ch != '?' && !char.IsWhiteSpace(ch)) return false;
            return true;
        }

        private void AllItemBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = sender as TextBox;
            string newText = tb.Text.Insert(tb.SelectionStart, e.Text);

            if (int.TryParse(newText, out int value))
            {
                e.Handled = value < 0 || value > 20;
            }
            else
            {
                e.Handled = true;
            }
        }

    }
}