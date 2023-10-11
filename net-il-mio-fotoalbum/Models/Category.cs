using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using net_il_mio_fotoalbum.ValidationAttributes;

namespace net_il_mio_fotoalbum.Models
{
    [Table(name: "categories")]
    public class Category
    {
        [Key, Column(name: "id")]
        public int Id { get; set; }

        [Column(name: "name"), Required(AllowEmptyStrings = false)]
        public string? Name { get; set; }

        [Column(name: "description", TypeName = "VARCHAR(1000)"), WordsCount(Min = 5, ErrorMessage = "The description must have 5 words at least."), Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        public List<Photo>? Photos { get; set; }
    }
}
