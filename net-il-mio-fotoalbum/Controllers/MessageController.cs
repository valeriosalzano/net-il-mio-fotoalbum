using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
	public class MessageController : Controller
	{
		private IRepository<Message> _messageManager;

		public MessageController(IRepository<Message> messageManager)
		{
			_messageManager = messageManager;
		}
		public IActionResult Index()
		{
			List<Message> messages = (List<Message>)_messageManager.GetAll();
			return View("Index",messages);
		}
	}
}
