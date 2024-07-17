using WpfNeolant.Model;
using WpfNeolant.ViewModel;
using System.Windows;

namespace WpfNeolant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMainWindowViewModel _viewModel;

        public MainWindow(IMainWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            _viewModel = vm;

            // Load data when the window is loaded
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Загрузка данных в PostgreSQL
            await _viewModel.LoadDataToPostgreAsync();
            // Загрузка данных из базы данных после вставки
            _viewModel.LoadDataFromPostgres();
        }

        private void lbAlbums_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!(lbAlbums.SelectedItem is Album a)) return;

            _viewModel.UpdatePhotos(a);
        }
    }
}
