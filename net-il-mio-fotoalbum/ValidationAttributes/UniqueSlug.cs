using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.ValidationAttributes
{
    public class UniqueSlug : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            value ??= "";
            string parsedValue = (string)value;
            Photo? evaluatedPhoto = validationContext.ObjectInstance as Photo;

            using (PhotographerShowcaseContext db = new PhotographerShowcaseContext())
            {
                Photo? foundPhoto = db.Photos.Where(photo => photo.Slug == parsedValue).FirstOrDefault();

                if (foundPhoto is not null && foundPhoto.Id != evaluatedPhoto?.Id)
                    return new ValidationResult($"A slug for this name already exists, choose a different name.");
            }
            return ValidationResult.Success;
        }
    }
    
}
