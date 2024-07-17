using MongoDB.Driver;
using WpfNeolant.Model;
using System.Net.Http;

namespace WpfNeolant.Data.Interfaces
{
    public interface IMongoDbDataLoader
    {
        Task LoadDbAsync();
        List<Album> GetAlbumsFromMongoDb();
        Task<List<Album>> GetAlbumsFromMongoDbAsync();
    }
}
