using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ForumSiteCore.Business.Models;
using ForumSiteCore.DAL.Models;

namespace ForumSiteCore.Business
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()
        {
            CreateMap<Forum, ForumDto>()
                .ForMember(dest => dest.UserSaved, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserDto>();

            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserVote, opt => opt.Ignore())
                .ForMember(dest => dest.UserCreated, opt => opt.Ignore())
                .ForMember(dest => dest.UserSaved, opt => opt.Ignore())
                .ForMember(dest => dest.ForumName, opt => opt.MapFrom(s => s.Forum != null ? s.Forum.Name : string.Empty))
                .ForMember(dest => dest.ShowForumName, opt => opt.Ignore());


            //.ForMember(dest => dest.ForumName, opt => opt.MapFrom(src => src.Forum.Name));                
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserVote, opt => opt.Ignore())
                .ForMember(dest => dest.UserCreated, opt => opt.Ignore())
                .ForMember(dest => dest.UserSaved, opt => opt.Ignore())
                .ForMember(dest => dest.Path, opt => opt.Ignore())
                .ForMember(dest => dest.Level, opt => opt.Ignore());



            CreateMap<CommentTree, CommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserVote, opt => opt.Ignore())
                .ForMember(dest => dest.UserCreated, opt => opt.Ignore())
                .ForMember(dest => dest.UserSaved, opt => opt.Ignore())
                .ForMember(dest => dest.Children, opt => opt.Ignore());
        }
    }
}
