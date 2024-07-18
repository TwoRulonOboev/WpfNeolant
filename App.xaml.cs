using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using WpfNeolant.Data;
using WpfNeolant.Data.Interfaces;
using WpfNeolant.ViewModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using WpfNeolant;

namespace WpfNeolant
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем конфигуратор, который будет читать настройки из файла config.json
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            // Регистрируем зависимости для нашего приложения
            ServiceCollection services = new ServiceCollection();

            // Регистрируем конфигуратор как singleton
            services.AddSingleton(config)
                // Регистрируем HttpClient как singleton
                .AddSingleton<HttpClient>()
                // Регистрируем MongoClient как транзакционный объект, который будет создаваться каждый раз, когда он будет запрошен
                .AddTransient<IMongoClient>(provider =>
                {
                    // Получаем конфигуратор из провайдера сервисов
                    var cfg = provider.GetService<IConfiguration>();
                    // Создаем MongoClient с помощью строки подключения из конфигурации
                    return new MongoClient(cfg["MongoDb:ConnectionString"]);
                })
                // Регистрируем MongoDBDataLoader как транзакционный объект, который будет создаваться каждый раз, когда он будет запрошен
                .AddTransient<IMongoDbDataLoader, MongoDBDataLoader>()
                // Регистрируем PostgresDataLoader как транзакционный объект, который будет создаваться каждый раз, когда он будет запрошен
                .AddTransient<IPostgresDataLoader, PostgresDataLoader>()
                // Регистрируем MainWindowViewModel как транзакционный объект, который будет создаваться каждый раз, когда он будет запрошен
                .AddTransient<IMainWindowViewModel, MainWindowViewModel>()
                // Регистрируем MainWindow как транзакционный объект, который будет создаваться каждый раз, когда он будет запрошен
                .AddTransient<MainWindow>();

            // Создаем провайдер сервисов на основе зарегистрированных зависимостей
            var serviceProvider = services.BuildServiceProvider();

            // Получаем главное окно из провайдера сервисов
            App.Current.MainWindow = serviceProvider.GetService<MainWindow>();
            // Показываем главное окно
            App.Current.MainWindow!.Show();
        }
    }
}