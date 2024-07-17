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

namespace WpfNeolant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаешь конфигуратор

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            HttpClient httpClient = new HttpClient();
            MongoClient mongoClient = new MongoClient(config["MongoDb:ConnectionString"]);


            // Регистрируешь зависимости

            ServiceCollection services = new ServiceCollection();


            services.AddSingleton(config)
                .AddSingleton(httpClient)
                .AddSingleton<IMongoClient>(mongoClient)
                .AddTransient<IMongoDbDataLoader, MongoDBDataLoader>()
                .AddTransient<IPostgresDataLoader, PostgresDataLoader>()
                .AddSingleton<IMainWindowViewModel, MainWindowViewModel>()
                .AddTransient<MainWindow>();

            App.Current.MainWindow = services.BuildServiceProvider().GetService<MainWindow>();
            App.Current.MainWindow.Show();


            // Разрешаешь зависимость                                               
            // App.Current.MainWindow = Resolve<MainWindow>(); - Resolve один раз   
            // App.Current.MainWindow.Show();                                       


        }
    }
}