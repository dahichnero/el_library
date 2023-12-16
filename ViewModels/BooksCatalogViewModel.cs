using ElLibrary.Domain.Entities;

namespace ElLibrary.ViewModels
{
    public class BooksCatalogViewModel
    {
        public List<Book> Books { get; set; }
        public List<Category> Categories { get; set; }
    }
}
