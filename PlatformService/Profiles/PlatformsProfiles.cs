namespace PlatformService.Profiles
{
    using AutoMapper;
    using PlatformService.Dtos;
    using PlatformService.Models;

    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            // source => target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();
            CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(desc => desc.PlatformId, memberOption => memberOption.MapFrom(src => src.Id));
        }
    }
}