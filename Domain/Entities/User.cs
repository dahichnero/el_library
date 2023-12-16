using System.ComponentModel.DataAnnotations;

namespace ElLibrary.Domain.Entities
{
    public class User :Entity
    {
        [StringLength(100)]
        public string Fullname { get; set; }
        [StringLength(100)]
        public string Login { get; set; }
        [StringLength(250)]
        public string Password { get; set; }
        [StringLength(100)]
        public string Salt { get; set; }
        [StringLength(250)]
        public string? Photo { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
