using System.ComponentModel.DataAnnotations;

namespace ElLibrary.Domain.Entities
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
