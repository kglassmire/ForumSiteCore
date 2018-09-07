using AutoMapper;
using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business
{
    public static class AutoMapperConfiguration
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Forum, ForumDto>()
                    .ForMember(dest => dest.UserSaved, opt => opt.Ignore());

                cfg.CreateMap<ApplicationUser, UserDto>();

                cfg.CreateMap<Post, PostDto>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                    .ForMember(dest => dest.UserVote, opt => opt.Ignore())
                    .ForMember(dest => dest.UserCreated, opt => opt.Ignore())
                    .ForMember(dest => dest.UserSaved, opt => opt.Ignore());

                //.ForMember(dest => dest.ForumName, opt => opt.MapFrom(src => src.Forum.Name));                
                cfg.CreateMap<Comment, CommentDto>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                    .ForMember(dest => dest.UserVote, opt => opt.Ignore())
                    .ForMember(dest => dest.UserCreated, opt => opt.Ignore())
                    .ForMember(dest => dest.UserSaved, opt => opt.Ignore());
            });

            Mapper.AssertConfigurationIsValid();
        }
    }
}
