using Microsoft.AspNetCore.Identity;

namespace net_il_mio_fotoalbum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Photo>? Photos { get; set; }
    }
}
