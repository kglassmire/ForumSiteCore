using FluentValidation;
using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Validators
{
    public class PostSaveVMValidator : AbstractValidator<PostSaveVM>
    {
        public PostSaveVMValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Post Id is required");

            RuleFor(x => x.Saved)
                .NotEmpty()
                .WithMessage("Saved is required");
        }
    }
}
