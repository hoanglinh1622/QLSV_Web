namespace StudentManagement.Services
{
    public interface ICsvService
    {
        Task<List<T>> ReadAsync<T>(string fileName) where T : class, new();
        Task WriteAsync<T>(string fileName, List<T> data) where T : class;
    }
}