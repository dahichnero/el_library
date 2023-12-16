using ElLibrary.Domain.Entities;

namespace ElLibrary.Domain.Services
{
    public interface IBookReader
    {
        Task<List<Book>> GetAllBooksAsync ();
        Task<List<Book>> FindBooksAsync (string searchString, int category);
        Task<Book?> FindBookAsync (int bookId);
        Task<List<Category>> GetCategoriesAsync ();
    }
}
