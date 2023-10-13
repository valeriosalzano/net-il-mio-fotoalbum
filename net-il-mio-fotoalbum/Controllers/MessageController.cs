using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    [Authorize(Roles = "SUPERADMIN")]
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
			return View(messages);
		}

        public IActionResult Details(int? id)
        {
			if (id is null)
				return NotFound();
			Message? message = _messageManager.GetById((int)id);
			if (message is null)
				return NotFound();
            return View(message);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id)
        {
            if (id is null)
                return NotFound();

            Message? message = _messageManager.GetById((int)id);

            if (message is null)
                return NotFound();

            try
            {
                _messageManager.Delete(message);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Problem("Something went wrong.");
            }

        }
    }
}
