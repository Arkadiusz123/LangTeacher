using Microsoft.AspNetCore.Mvc;

namespace LangTeacher.Server.Conversations
{
    [ApiController, Route("api/conversations")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost]
        public async Task<ActionResult> GetResponse([FromBody]GetResponseRequest request)
        {
            var result = await _conversationService.GetResponseAsync(request);

            return Ok(result);
        }
    }
}
