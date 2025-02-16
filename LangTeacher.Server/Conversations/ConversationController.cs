using Microsoft.AspNetCore.Mvc;

namespace LangTeacher.Server.Conversations
{
    [ApiController, Route("api/conversations")]
    public class ConversationController : ControllerBase
    {
        public ActionResult GetResponse([FromBody]GetResponseRequest request)
        {
            return null;
        }
    }
}
