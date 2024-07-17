namespace WpfNeolant.Data.Interfaces
{
    public interface IPostgresDataLoader
    {
        Task LoadPhotoPostgresAsync();
        Task LoadAlbumsPostgresAsync();
    }
}