using Chats.Application.Contracts.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.Validators
{
    public class GetChatMessagesDTOValidator : AbstractValidator<GetChatMessagesDTO>
    {
        public GetChatMessagesDTOValidator()
        {
            RuleFor(message => message.ChatId)
                .NotEmpty().WithMessage("ChatId is required.");
        }
    }
}