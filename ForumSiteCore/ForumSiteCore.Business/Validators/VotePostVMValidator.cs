using FluentValidation;
using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Validators
{
    public class VotePostVMValidator : AbstractValidator<VotePostVM>
    {
        public VotePostVMValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Post Id is required");
            RuleFor(x => x.Direction)
                .NotEmpty()
                .WithMessage("Vote Direction is required");
        }
    }
}
