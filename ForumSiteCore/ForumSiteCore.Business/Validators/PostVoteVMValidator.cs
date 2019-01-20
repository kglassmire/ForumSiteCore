﻿using FluentValidation;
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
                .WithMessage("Post Id is required");
            RuleFor(x => x.VoteType)
                .NotEmpty()
                .WithMessage("Vote Direction is required");
        }
    }
}
