using FluentValidation;
using ForumSiteCore.Business.Enums;
using ForumSiteCore.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Validators
{
    public class PostVoteVMValidator : AbstractValidator<PostVoteVM>
    {
        public PostVoteVMValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Post Id is required");
            RuleFor(x => x.VoteType)
                .IsInEnum()
                .WithMessage("Vote Direction is required");
        }
    }
}
