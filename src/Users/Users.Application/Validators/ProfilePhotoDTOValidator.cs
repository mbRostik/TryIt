using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;

namespace Users.Application.Validators
{
    public class ProfilePhotoDTOValidator : AbstractValidator<ProfilePhotoDTO>
    {
        public ProfilePhotoDTOValidator()
        {
            RuleFor(message => message.Avatar)
                .NotEmpty().WithMessage("Avatar is required.");
        }
    }
}