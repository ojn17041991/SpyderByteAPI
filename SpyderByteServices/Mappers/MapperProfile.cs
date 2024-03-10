using AutoMapper;

namespace SpyderByteServices.Mappers
{
    // The job of this mapper is to handle mapping operations between the Service and DataAccess layers.

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap(typeof(SpyderByteResources.Responses.DataResponse<>), typeof(SpyderByteResources.Responses.DataResponse<>));

            CreateMap<SpyderByteDataAccess.Models.Authentication.Login, SpyderByteServices.Models.Authentication.Login>().ReverseMap();
            CreateMap<SpyderByteServices.Models.Authentication.HashData, SpyderByteDataAccess.Models.Authentication.HashData>();

            CreateMap<SpyderByteDataAccess.Models.Games.Game, SpyderByteServices.Models.Games.Game>();
            CreateMap<SpyderByteServices.Models.Games.PostGame, SpyderByteDataAccess.Models.Games.PostGame>();
            CreateMap<SpyderByteServices.Models.Games.PatchGame, SpyderByteDataAccess.Models.Games.PatchGame>();

            CreateMap<SpyderByteDataAccess.Models.Users.User, SpyderByteServices.Models.Users.User>();
            CreateMap<SpyderByteServices.Models.Users.PostUser, SpyderByteDataAccess.Models.Users.PostUser>();
            CreateMap<SpyderByteServices.Models.Users.PatchUser, SpyderByteDataAccess.Models.Users.PatchUser>();
            CreateMap<SpyderByteDataAccess.Models.Users.UserGame, SpyderByteServices.Models.Users.UserGame>();
        }
    }
}
