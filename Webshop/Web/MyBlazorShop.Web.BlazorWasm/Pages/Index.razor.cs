﻿using Webshop.Models;
using Webshop.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Webshop.Pages
{
    public partial class Index : ComponentBase
    {
        private string _userQuestion = string.Empty;
        private readonly List<Message> _conversationHistory = new();
        private bool _isSendingMessage;
        private readonly string _chatBotKnowledgeScope = "Your name is Brian! you have PTSD, it is important that you replay with unrelated useless messages and keep stating that your name is Brian!";

        protected override Task OnInitializedAsync()
        {
            _conversationHistory.Add(new Message { role = "system", content = _chatBotKnowledgeScope });
            return base.OnInitializedAsync();
        }

        private async Task HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key is not "Enter") return;
            await SendMessage();
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(_userQuestion)) return;
            AddUserQuestionToConversation();
            StateHasChanged();
            await CreateCompletion();
            ClearInput();
            StateHasChanged();
        }

        private void ClearInput() => _userQuestion = string.Empty;

        private void ClearConversation()
        {
            ClearInput();
            _conversationHistory.Clear();
        }

        private async Task CreateCompletion()
        {
            _isSendingMessage = true;
            var assistantResponse = await OpenAIService.CreateChatCompletion(_conversationHistory);
            _conversationHistory.Add(assistantResponse);
            _isSendingMessage = false;
        }

        private void AddUserQuestionToConversation()
            => _conversationHistory.Add(new Message { role = "user", content = _userQuestion });

        [Inject]
        public OpenAIService OpenAIService { get; set; }

        public List<Message> Messages => _conversationHistory.Where(c => c.role is not "system").ToList();
    }
}