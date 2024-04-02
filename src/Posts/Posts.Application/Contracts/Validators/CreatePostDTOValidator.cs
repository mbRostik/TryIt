using FluentValidation;
using Posts.Application.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.Contracts.Validators
{
    public class CreatePostDTOValidator : AbstractValidator<CreatePostDTO>
    {
        public CreatePostDTOValidator()
        {
            RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Title) || !string.IsNullOrWhiteSpace(x.Content))
            .WithMessage("Either Title or Content must be provided.");

            RuleFor(x => x.Content)
                .MaximumLength(500)
                .WithMessage("The max Length is 500");

            RuleFor(x => x.Title)
                .MaximumLength(50)
                .WithMessage("The max Length is 50");

            RuleFor(x => x.Files)
             .Must(files => files.Count <= 8)
             .WithMessage("A maximum of 8 files is allowed.");

        }
    }
}