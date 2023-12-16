using ElLibrary.Domain.Entities;
using ElLibrary.Domain.Services;

namespace ElLibrary.Infrastructure
{
    public class BookReader :IBookReader
    {
        private readonly IRepository<Book> repository;
        private readonly IRepository<Category> categories;

        public BookReader (IRepository<Book> repository, IRepository<Category> categories)
        {
            this.repository = repository;
            this.categories = categories;
        }

        public async Task<Book?> FindBookAsync (int bookId) =>
            await repository.FindAsync(bookId);

        public async Task<List<Book>> FindBooksAsync (string searchString, int category) => (searchString, category) switch
        {
            ("" or null,0) => await repository.GetAllAsync(),
            (_,0)=> await repository.FindWhere(book => book.Title.Contains(searchString) || book.Author.Contains(searchString)),
            (_,_)=> await repository.FindWhere(book =>book.CategoryId == category &&
            (book.Title.Contains(searchString) || book.Author.Contains(searchString))),
        };
        public async Task<List<Book>> GetAllBooksAsync () => await repository.GetAllAsync();

        public async Task<List<Category>> GetCategoriesAsync () =>
            await categories.GetAllAsync();
    }
}
