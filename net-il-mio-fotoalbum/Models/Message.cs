using net_il_mio_fotoalbum.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_il_mio_fotoalbum.Models
{
	[Table("messages")]
	public class Message
	{
		[Key, Column(name: "id")]
		public long Id { get; set; }

		[Column(name: "email"), Required(AllowEmptyStrings = false), EmailAddress]
		public string? Email { get; set; }

		[Column(name: "body", TypeName = "VARCHAR(2000)"), WordsCount(Min = 10, ErrorMessage = "The message must have 10 words at least."), Required(ErrorMessage = "A message is required.")]
		public string? Body { get; set; }
	}
}
