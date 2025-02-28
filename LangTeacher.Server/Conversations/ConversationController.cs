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

        [HttpPost("generate-response")]
        public async Task<ActionResult> GetResponse([FromBody]GetResponseRequest request)
        {
            var result = await _conversationService.GetResponseAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("list")]
        public async Task<ActionResult> ConversationsList()
        {
            var result = await _conversationService.GetConversations();
            return Ok(result);
        }
    }
}
