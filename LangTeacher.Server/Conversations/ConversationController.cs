using LangTeacher.Server.Conversations.Responses;
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

        /// <summary>
        /// Send your massage and id of conversation, that you wolud like to continue. If empty then new conversation will start.
        /// As a response, you will get AI reposnse to your message and id of current conversation.
        /// </summary>
        [HttpPost("generate-response")]
        public async Task<ActionResult<GetResponseResp>> GetResponse([FromBody]GetResponseRequest request)
        {
            var result = await _conversationService.GetResponseAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Get list of your conversations
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ConversationResponse>>> ConversationsList()
        {
            var result = await _conversationService.GetConversationsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Delete conversation's history by id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteConversation(int id)
        {
            var deleteResult = await _conversationService.DeleteConversationAsync(id);

            if (!deleteResult.IsSuccess)
                return BadRequest(deleteResult.Error);

            return NoContent();
        }
    }
}
