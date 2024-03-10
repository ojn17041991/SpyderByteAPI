using AutoMapper;

namespace SpyderByteAPI.Mappers
{
    // The job of this mapper is to handle mapping operations between the API and Services layer.

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap(typeof(SpyderByteResources.Responses.DataResponse<>), typeof(SpyderByteResources.Responses.DataResponse<>));

            CreateMap<SpyderByteServices.Models.Authentication.Login, SpyderByteAPI.Models.Authentication.Login>().ReverseMap();

            CreateMap<SpyderByteServices.Models.Games.Game, SpyderByteAPI.Models.Games.Game>()
                .ForMember(
                    d => d.LeaderboardId,
                    o => o.MapFrom(
                        s => s.LeaderboardGame!.LeaderboardId
                    )
                )
                .ForMember(
                    d => d.UserId,
                    o => o.MapFrom(
                        s => s.UserGame!.UserId
                    )
                );
            CreateMap<SpyderByteAPI.Models.Games.PostGame, SpyderByteServices.Models.Games.PostGame>();
            CreateMap<SpyderByteAPI.Models.Games.PatchGame, SpyderByteServices.Models.Games.PatchGame>();

            CreateMap<SpyderByteServices.Models.Users.User, SpyderByteAPI.Models.Users.User>()
                .ForMember(
                    d => d.GameId,
                    o => o.MapFrom(
                        s => s.UserGame!.GameId
                    )
                );
            CreateMap<SpyderByteAPI.Models.Users.PostUser, SpyderByteServices.Models.Users.PostUser>();
            CreateMap<SpyderByteAPI.Models.Users.PatchUser, SpyderByteServices.Models.Users.PatchUser>();
        }
    }
}
