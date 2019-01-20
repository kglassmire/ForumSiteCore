using FluentValidation;
using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Validators
{
    public class VotePostVMValidator : AbstractValidator<PostVoteVM>
    {
        public VotePostVMValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty()
                .WithMessage("Post Id is required");
            RuleFor(x => x.VoteType)
                .NotEmpty()
                .WithMessage("Vote Direction is required");
        }
    }
}
