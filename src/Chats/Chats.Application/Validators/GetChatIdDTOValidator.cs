using Chats.Application.Contracts.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.Validators
{
    public class GetChatIdDTOValidator : AbstractValidator<GetChatIdDTO>
    {
        public GetChatIdDTOValidator()
        {
            RuleFor(message => message.ProfileId)
                .NotEmpty().WithMessage("Profile ID is required.");
        }
    }
}