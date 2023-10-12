using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoFormModel
    {
        public required Photo Photo { get; set; }
        public string? UserName { get; set; }

        public IFormFile? ImgFile { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<string>? SelectedCategoriesId { get; set; }
    }
}
