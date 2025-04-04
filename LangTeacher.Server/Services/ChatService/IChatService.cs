﻿using LangTeacher.Server.Conversations;

namespace LangTeacher.Server.Services.ChatService
{
    public interface IChatService
    {
        void SetChatHistory(IEnumerable<AppMessage> messages);
        Task<string> GetResponseAsync(string prompt);
    }
}
