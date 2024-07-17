using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WpfNeolant.Data.Interfaces;
using WpfNeolant.Model;
using WpfNeolant.Utils;
using System.Net.Http;
using System.Net.Http.Json;

namespace WpfNeolant.Data
{
    internal sealed class MongoDBDataLoader : IMongoDbDataLoader
    {
        private readonly HttpClient _httpClient;
        private readonly IMongoClient _mongoClient;

        private readonly IConfiguration _config;
        private readonly string? _databaseName;
        private readonly string? _collectionName;

        public MongoDBDataLoader(IMongoClient mongodb, IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _mongoClient = mongodb;
            _databaseName = _config["MongoDb:DatabaseName"];
            _collectionName = _config["MongoDb:CollectionName"];
        }

        public async Task LoadDbAsync()
        {
            string? albumsUrl = _config["DownloadStrings:Albums"];
            string? photosUrl = _config["DownloadStrings:Photos"];

            List<Album>? albums = await _httpClient.GetFromJsonAsync<List<Album>>(albumsUrl);
            List<Photo>? photos = await _httpClient.GetFromJsonAsync<List<Photo>>(photosUrl);

            if (albums == null || photos == null) return;

            foreach (Album album in albums)
                album.Photos = photos.Where(x => x.AlbumId == album.Id).ToList();

            IMongoDatabase db = _mongoClient.GetDatabase(_databaseName);

            // Удаление коллекции перед вставкой новых данных
            await db.DropCollectionAsync(_collectionName);

            // Создание новой коллекции
            IMongoCollection<Album> albumsCollection = db.GetCollection<Album>(_collectionName);

            // Вставка данных в новую коллекцию
            await albumsCollection.InsertManyAsync(albums);
        }

        public List<Album> GetAlbumsFromMongoDb()
        {
            IMongoDatabase db = _mongoClient.GetDatabase(_databaseName);
            IMongoCollection<Album> albumsCollection = db.GetCollection<Album>(_collectionName);

            return albumsCollection.Find(new BsonDocument()).ToList();
        }

        public async Task<List<Album>> GetAlbumsFromMongoDbAsync()
        {
            IMongoDatabase db = _mongoClient.GetDatabase(_databaseName);
            IMongoCollection<Album> albumsCollection = db.GetCollection<Album>(_collectionName);

            using (IAsyncCursor<Album> cursor = await albumsCollection.FindAsync(new BsonDocument()))
            {
                return cursor.ToList();
            };
        }
    }
}
