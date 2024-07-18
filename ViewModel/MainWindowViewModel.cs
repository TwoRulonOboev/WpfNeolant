using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfNeolant.Data;
using WpfNeolant.Data.Interfaces;
using WpfNeolant.Model;

namespace WpfNeolant.ViewModel
{
    // ViewModel для главного окна приложения
    public class MainWindowViewModel : IMainWindowViewModel
    {
        // Поле для загрузки данных из PostgreSQL
        private readonly IPostgresDataLoader _postgresDataLoader;

        // Поле для загрузки данных из MongoDB
        private readonly IMongoDbDataLoader _mongoDBDataLoader;

        // Коллекция альбомов
        public ObservableCollection<Album> Albums { get; private set; }

        // Коллекция фотографий
        public ObservableCollection<Photo> Photos { get; private set; }

        // Выбранный альбом
        public Album SelectedAlbum { get; set; }

        // Конструктор ViewModel
        public MainWindowViewModel(IPostgresDataLoader postgres, IMongoDbDataLoader mongoDb)
        {
            // Инициализация полей для загрузки данных
            _mongoDBDataLoader = mongoDb ?? throw new ArgumentNullException(nameof(mongoDb));
            _postgresDataLoader = postgres ?? throw new ArgumentNullException(nameof(postgres));

            // Инициализация коллекций
            Albums = new ObservableCollection<Album>();
            Photos = new ObservableCollection<Photo>();

            // Инициализация базы данных
            InitializeDatabase();
        }

        // Метод инициализации базы данных
        private async void InitializeDatabase()
        {
            try
            {
                // Создание контекста базы данных
                using (PracticeContext context = new PracticeContext())
                {
                    // Удаление и создание базы данных
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                }
            }
            catch (Exception ex)
            {
                // Отображение ошибки инициализации базы данных
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}");
            }
        }

        // Метод загрузки данных в MongoDB
        public async Task LoadDataToMongoDBAsync()
        {
            try
            {
                // Загрузка данных в MongoDB
                await _mongoDBDataLoader.LoadDbAsync();
                // Загрузка данных из MongoDB
                LoadDataFromMongoDB();
            }
            catch (Exception ex)
            {
                // Отображение ошибки загрузки данных в MongoDB
                MessageBox.Show(ex.Message);
            }
        }

        // Метод загрузки данных в PostgreSQL
        public async Task LoadDataToPostgreAsync()
        {
            try
            {
                // Загрузка альбомов в PostgreSQL
                await _postgresDataLoader.LoadAlbumsPostgresAsync();
                // Загрузка фотографий в PostgreSQL
                await _postgresDataLoader.LoadPhotoPostgresAsync();
                // Загрузка данных из PostgreSQL
                LoadDataFromPostgres();
            }
            catch (Exception ex)
            {
                // Отображение ошибки загрузки данных в PostgreSQL
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    MessageBox.Show(innerException.Message);
                    innerException = innerException.InnerException;
                }
                MessageBox.Show(ex.Message);
            }
        }

        // Метод загрузки данных из MongoDB
        public void LoadDataFromMongoDB()
        {
            if (_mongoDBDataLoader == null)
            {
                throw new InvalidOperationException("MongoDB Data Loader is not initialized.");
            }

            // Получение альбомов из MongoDB
            var albums = _mongoDBDataLoader.GetAlbumsFromMongoDb() ?? new List<Album>();
            // Обновление коллекции альбомов
            UpdateAlbums(albums);
        }

        // Метод загрузки данных из PostgreSQL
        public void LoadDataFromPostgres()
        {
            using (PracticeContext context = new PracticeContext())
            {
                // Получение альбомов из PostgreSQL
                var albums = context.Albums.ToList() ?? new List<Album>();
                // Обновление коллекции альбомов
                UpdateAlbums(albums);
            }
        }

        // Метод обновления коллекции альбомов
        private void UpdateAlbums(List<Album> albums)
        {
            // Очистка коллекции альбомов
            Albums.Clear();
            // Добавление альбомов в коллекцию
            foreach (var album in albums)
            {
                Albums.Add(album);
            }
        }


        // Метод загрузки фотографий для выбранного альбома
        public void LoadPhotosForAlbum()
        {
            if (SelectedAlbum != null)
            {
                // Создание контекста базы данных
                using (PracticeContext context = new PracticeContext())
                {
                    // Получение фотографий для выбранного альбома
                    var photos = context.Photos.Where(p => p.AlbumId == SelectedAlbum.Id).ToList();
                    // Очистка коллекции фотографий
                    Photos.Clear();
                    // Добавление фотографий в коллекцию
                    foreach (var photo in photos)
                    {
                        Photos.Add(photo);
                    }
                }
            }
        }
    }
}