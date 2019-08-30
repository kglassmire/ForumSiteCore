using FluentValidation;
using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Validators
{
    public class ForumSaveVMValidator : AbstractValidator<ForumSaveVM>
    {
        public ForumSaveVMValidator()
        {
            RuleFor(x => x.ForumId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("ForumId is required");                

            RuleFor(x => x.Saved)
                .NotEmpty()                
                .WithMessage("Saved is required");
        }
    }
}
