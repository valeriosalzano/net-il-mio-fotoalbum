using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using net_il_mio_fotoalbum.Utility;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private IRepository<Message> _messageManager;

        public MessagesController(IRepository<Message> messageManager)
        {
            _messageManager = messageManager;
        }

        [HttpPost]
        public IActionResult Send([FromBody] Message newMessage)
        {
            try
            {
                _messageManager.Add(newMessage);
                return Ok("Message received!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
