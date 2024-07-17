using WpfNeolant.Data;
using WpfNeolant.Data.Interfaces;
using WpfNeolant.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfNeolant.ViewModel
{
    public class MainWindowViewModel : IMainWindowViewModel
    {
        private readonly IMongoDbDataLoader _mongoDBDataLoader;
        private readonly IPostgresDataLoader _postgresDataLoader;

        public ObservableCollection<Photo> Photos { get; private set; }
        public ObservableCollection<Album> Albums { get; private set; }
        public List<Album> CachedAlbums { get; private set; }

        public MainWindowViewModel(IPostgresDataLoader postgres, IMongoDbDataLoader mongoDb)
        {
            _mongoDBDataLoader = mongoDb ?? throw new ArgumentNullException(nameof(mongoDb));
            _postgresDataLoader = postgres ?? throw new ArgumentNullException(nameof(postgres));

            Photos = new ObservableCollection<Photo>();
            Albums = new ObservableCollection<Album>();

            CachedAlbums = new List<Album>();

            // Удаление и создание базы данных при запуске
            InitializeDatabase();
            LoadDataFromMongoDB();
        }

        private async void InitializeDatabase()
        {
            try
            {
                using (PracticeContext context = new PracticeContext())
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}");
            }
        }

        public async Task LoadDataToMongoDBAsync()
        {
            try
            {
                await _mongoDBDataLoader.LoadDbAsync();
                LoadDataFromMongoDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task LoadDataToPostgreAsync()
        {
            try
            {
                // Сначала загружаем альбомы
                await _postgresDataLoader.LoadAlbumsPostgresAsync();

                // Затем загружаем фотографии
                await _postgresDataLoader.LoadPhotoPostgresAsync();

                LoadDataFromPostgres();
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    MessageBox.Show(innerException.Message);
                    innerException = innerException.InnerException;
                }
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadDataFromMongoDB()
        {
            if (_mongoDBDataLoader == null)
            {
                throw new InvalidOperationException("MongoDB Data Loader is not initialized.");
            }

            CachedAlbums = _mongoDBDataLoader.GetAlbumsFromMongoDb() ?? new List<Album>();
            UpdateAlbums();
        }

        public void LoadDataFromPostgres()
        {
            using (PracticeContext context = new PracticeContext())
            {
                var albums = context.Albums.ToList() ?? new List<Album>();
                if (!albums.SequenceEqual(CachedAlbums))
                {
                    CachedAlbums = albums;
                    UpdateAlbums();
                }
            }
        }


        public void UpdateAlbums()
        {
            Albums.Clear();
            CachedAlbums.ForEach(Albums.Add);
        }

        public void UpdatePhotos(Album album)
        {
            if (album == null)
            {
                throw new ArgumentNullException(nameof(album), "Album is null.");
            }

            if (album.Photos == null || !album.Photos.Any())
            {
                Photos.Clear();
                return;
            }

            if (!album.Photos.SequenceEqual(Photos))
            {
                Photos.Clear();
                foreach (var p in album.Photos) Photos.Add(p);
            }
        }

    }
}
