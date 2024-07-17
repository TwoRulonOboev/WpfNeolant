using WpfNeolant.Data.Interfaces;
using WpfNeolant.Model;
using WpfNeolant.Utils;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace WpfNeolant.Data
{
    public class PostgresDataLoader : IPostgresDataLoader
    {
        private readonly HttpClient _client;

        public PostgresDataLoader(HttpClient client)
        {
            _client = client;
        }

        public async Task LoadDataToPostgreAsync()
        {
            try
            {
                // Удаление базы данных
                using (PracticeContext context = new PracticeContext())
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                }

                // Загрузка и вставка данных
                await LoadAlbumsPostgresAsync();
                await LoadPhotoPostgresAsync();
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

        public async Task LoadPhotoPostgresAsync()
        {
            string? photosUrl = ConfigUtility.Config["DownloadStrings:Photos"];
            List<Photo>? photos = await _client.GetFromJsonAsync<List<Photo>>(photosUrl);

            if (photos == null) return;

            using (PracticeContext context = new PracticeContext())
            {
                // Вставка данных в новую базу данных
                await context.Photos.AddRangeAsync(photos);
                await context.SaveChangesAsync();
            }
        }

        public async Task LoadAlbumsPostgresAsync()
        {
            string? albumsUrl = ConfigUtility.Config["DownloadStrings:Albums"];
            List<Album>? albums = await _client.GetFromJsonAsync<List<Album>>(albumsUrl);

            if (albums == null) return;

            using (PracticeContext context = new PracticeContext())
            {
                // Вставка данных в новую базу данных
                await context.Albums.AddRangeAsync(albums);
                await context.SaveChangesAsync();
            }
        }
    }
}
