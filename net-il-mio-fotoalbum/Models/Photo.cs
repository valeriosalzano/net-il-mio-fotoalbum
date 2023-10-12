using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Utility;
using net_il_mio_fotoalbum.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_il_mio_fotoalbum.Models
{
    [Table("photos"), Index(nameof(Slug), IsUnique = true)]
    public class Photo
    {
        #region *** Table Columns ***

        [Column(name:"id"), Key]
        public int Id { get; set; }

        [Column(name: "title", TypeName = "VARCHAR(100)"), Required(AllowEmptyStrings = false, ErrorMessage = "The title of the photo is required.")]
        public string? Title { get; set; }

        [Column(name: "slug", TypeName = "VARCHAR(100)"), Required(AllowEmptyStrings = true), UniqueSlug]
        public string? Slug { get; set; }

        [Column(name: "description", TypeName = "VARCHAR(1000)"), WordsCount(Min = 5, ErrorMessage = "The description must have 5 words at least."), Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [Column(name: "img_path", TypeName = "VARCHAR(1000)"), Required(AllowEmptyStrings = true)]
        public string? ImgPath { get; set; }

        [Column(name:"visibility")]
        public bool Visibility { get; set; }

        [Column(name: "user_id", TypeName = "NVARCHAR"), Required ]
        public string? UserId { get; set; }
        #endregion

        #region *** Relations ***
        public List<Category>? Categories { get; set; }

        #endregion

        #region *** Constructors ***
        public Photo () { }

        public Photo(string title, string description, string imgPath, bool visibility = false)
        {
            Title = title;
            Slug = Helper.GetSlugFromString(title);
            Description = description;
            ImgPath = imgPath;
            Visibility = visibility;
        }

        #endregion
    }
}
