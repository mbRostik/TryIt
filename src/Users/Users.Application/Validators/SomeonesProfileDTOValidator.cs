using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;

namespace Users.Application.Validators
{
    public class SomeonesProfileDTOValidator : AbstractValidator<SomeonesProfileDTO>
    {
        public SomeonesProfileDTOValidator()
        {
            RuleFor(message => message.ProfileId)
                .NotEmpty().WithMessage("Profile ID is required.");

        }
    }
}