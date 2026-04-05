using AutoMapper;

namespace SpyderByteAPI.Mappers
{
    // The job of this mapper is to handle mapping operations between the API and Services layer.

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
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
            CreateMap<SpyderByteAPI.Models.Games.V1.PatchGame, SpyderByteServices.Models.Games.PatchGame>();
            CreateMap<SpyderByteAPI.Models.Games.V1_4.PatchGame, SpyderByteServices.Models.Games.PatchGame>();

            CreateMap<SpyderByteServices.Models.Users.User, SpyderByteAPI.Models.Users.User>()
                .ForMember(
                    d => d.GameId,
                    o => o.MapFrom(
                        s => s.UserGame!.GameId
                    )
                );
            CreateMap<SpyderByteAPI.Models.Users.PostUser, SpyderByteServices.Models.Users.PostUser>();
            CreateMap<SpyderByteAPI.Models.Users.V1.PatchUser, SpyderByteServices.Models.Users.PatchUser>();
            CreateMap<SpyderByteAPI.Models.Users.V1_4.PatchUser, SpyderByteServices.Models.Users.PatchUser>();

            CreateMap<SpyderByteServices.Models.Leaderboards.Leaderboard, SpyderByteAPI.Models.Leaderboards.Leaderboard>()
                .ForMember(
                    d => d.GameId,
                    o => o.MapFrom(
                        s => s.LeaderboardGame!.GameId
                    )
                );
            CreateMap<SpyderByteServices.Models.Leaderboards.LeaderboardRecord, SpyderByteAPI.Models.Leaderboards.LeaderboardRecord>();
            CreateMap<SpyderByteAPI.Models.Leaderboards.PostLeaderboard, SpyderByteServices.Models.Leaderboards.PostLeaderboard>();
            CreateMap<SpyderByteAPI.Models.Leaderboards.PostLeaderboardRecord, SpyderByteServices.Models.Leaderboards.PostLeaderboardRecord>();
            CreateMap<SpyderByteAPI.Models.Leaderboards.V1.PatchLeaderboard, SpyderByteServices.Models.Leaderboards.PatchLeaderboard>();
            CreateMap<SpyderByteAPI.Models.Leaderboards.V1_4.PatchLeaderboard, SpyderByteServices.Models.Leaderboards.PatchLeaderboard>();
        }
    }
}
