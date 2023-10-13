using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.API
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class PhotosController : ControllerBase
	{
		private PhotoManager _photoManager;

		public PhotosController (PhotoManager photoManager)
		{
			_photoManager = photoManager;
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Photo> photoList = (List<Photo>)_photoManager.GetAllFiltered(pizza => pizza.Visibility, true);
			return Ok(photoList);
		}

		[HttpGet]
		public IActionResult GetAllContaining(string? name)
		{
			List<Photo> foundPhotos = _photoManager.GetAllContaining(name, true).Where(photo => photo.Visibility == true).ToList();

			return Ok(foundPhotos);
		}
	}
}
