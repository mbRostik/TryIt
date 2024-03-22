﻿using Aggregator.Application.Contracts.DTOs;
using Aggregator.Application.Contracts.Interfaces;
using Aggregator.WebApi.Services.ProtoServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Infrastructure.Services
{
    public class ChatService:IChatService
    {
        private grpcGetUserForChatService UsergrpcService { get; set; }
        private grpcGetUserChatsService ChatgrpcService { get; set; }

        public ChatService(grpcGetUserChatsService ChatgrpcService, grpcGetUserForChatService UsergrpcService)
        {
            this.ChatgrpcService = ChatgrpcService;
            this.UsergrpcService = UsergrpcService;
        }


        public async Task<List<GiveUserChatsDTO>> GetUserChats(string userId, string accessToken)
        {
            try
            {
                var chats = await ChatgrpcService.GetUserChatsAsync(userId, accessToken);

                if (chats.Chats.Count == 1 && chats.Chats[0].Chatid == 0)
                {
                    return null;
                }

                List<string> chatsIds = new List<string>();
                foreach (var item in chats.Chats)
                {
                    chatsIds.Add(item.ContactId);
                }

                var contacts = await UsergrpcService.GetUserChatsAsync(chatsIds, accessToken);


                if (contacts.Users.Count() != chats.Chats.Count())
                {
                    return null;
                }

                List<GiveUserChatsDTO> result = new List<GiveUserChatsDTO>();

                for (int i = 0; i != chats.Chats.Count(); i++)
                {
                    GiveUserChatsDTO temp = new GiveUserChatsDTO();
                    temp.ChatId = chats.Chats[i].Chatid;
                    temp.lastActivity = null;
                    if (chats.Chats[i].LastActivity != null)
                    {
                        temp.lastActivity = chats.Chats[i].LastActivity.ToDateTime();
                    }
                    temp.lastMessage = chats.Chats[i].LastMessage;
                    temp.lastMessageSender = chats.Chats[i].LastMessageSenderId;
                    temp.ContactId = contacts.Users[i].UserId;
                    temp.ContactNickName = contacts.Users[i].NickName;
                    temp.ContactPhoto = contacts.Users[i].Photo.ToByteArray();

                    result.Add(temp);
                }
                return result;
            }
           catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}