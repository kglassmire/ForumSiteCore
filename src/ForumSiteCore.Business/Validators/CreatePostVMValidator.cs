using FluentValidation;
using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Validators
{
    public class CreatePostVMValidator : AbstractValidator<CreatePostVM>
    {
        public CreatePostVMValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(500);
            RuleFor(m => m.Url)
                .Empty()
                .When(x => !string.IsNullOrEmpty(x.Description));
            RuleFor(m => m.Description)
                .Empty()
                .When(m => !string.IsNullOrEmpty(m.Url));
        }
    }
}
