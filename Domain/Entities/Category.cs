using System.ComponentModel.DataAnnotations;

namespace ElLibrary.Domain.Entities
{
    public class Category :Entity
    {
        [StringLength(150)]
        public string Name { get; set; } = null!;
        public List<Book> Books { get; set; }
    }
}
