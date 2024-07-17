using WpfNeolant.Data.Interfaces;
using WpfNeolant.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace WpfNeolant.ViewModel
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<Photo> Photos { get; }
        ObservableCollection<Album> Albums { get; }

        Task LoadDataToMongoDBAsync();
        Task LoadDataToPostgreAsync();

        void LoadDataFromMongoDB();
        void LoadDataFromPostgres();

        void UpdateAlbums();
        void UpdatePhotos(Album album);
    }
}
