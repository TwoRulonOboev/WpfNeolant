using WpfNeolant.Model;
using WpfNeolant.ViewModel;
using System.Windows;
using System.Windows.Controls;
using WpfNeolant.Data.Interfaces;

namespace WpfNeolant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMainWindowViewModel _viewModel;
        private readonly IMongoDbDataLoader _mongoDbDataLoader;

        public MainWindow(IMainWindowViewModel vm, IMongoDbDataLoader mongoDbDataLoader)
        {
            InitializeComponent();
            DataContext = vm;
            _viewModel = vm;
            _mongoDbDataLoader = mongoDbDataLoader;

            // Load data when the window is loaded
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Загрузка данных в PostgreSQL
            await _viewModel.LoadDataToPostgreAsync();
            // Загрузка данных из базы данных после вставки
            _viewModel.LoadDataFromPostgres();

            // Удаление и создание коллекции в MongoDB
            await _mongoDbDataLoader.LoadDbAsync();
        }

        private void lbAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _viewModel.SelectedAlbum = e.AddedItems[0] as Album;
                _viewModel.LoadPhotosForAlbum();
            }
        }
    }
}