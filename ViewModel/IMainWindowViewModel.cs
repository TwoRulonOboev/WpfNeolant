using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WpfNeolant.Model;

namespace WpfNeolant.ViewModel
{
    public interface IMainWindowViewModel
    {
        // Коллекция альбомов
        ObservableCollection<Album> Albums { get; }

        // Коллекция фотографий
        ObservableCollection<Photo> Photos { get; }

        // Выбранный альбом
        Album SelectedAlbum { get; set; }

        // Загрузка данных в MongoDB
        Task LoadDataToMongoDBAsync();

        // Загрузка данных в Postgres
        Task LoadDataToPostgreAsync();

        // Загрузка данных из MongoDB
        void LoadDataFromMongoDB();

        // Загрузка данных из Postgres
        void LoadDataFromPostgres();

        // Загрузка фотографий для альбома
        void LoadPhotosForAlbum();
    }
}