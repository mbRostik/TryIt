using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;

namespace Users.Application.Validators
{
    public class ChangeProfileInformationDTOValidator : AbstractValidator<ChangeProfileInformationDTO>
    {
        public ChangeProfileInformationDTOValidator()
        {
            RuleFor(message => message.Email)
                .NotEmpty().WithMessage("Email is required.")
                .Must(email => email.EndsWith("@gmail.com")).WithMessage("Email must be a Gmail account.");

            RuleFor(message => message.Bio)
                .MaximumLength(5).WithMessage("Bio size - 500");
        }
    }
}