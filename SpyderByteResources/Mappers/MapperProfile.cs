using AutoMapper;
using SpyderByteResources.Mappers.Converters;
using SpyderByteResources.Models.Paging.Abstract;
using SpyderByteResources.Models.Responses;

namespace SpyderByteResources.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap(typeof(DataResponse<>), typeof(DataResponse<>));

            CreateMap(typeof(IPagedList<>), typeof(IPagedList<>))
                .ConvertUsing(typeof(PagedListConverter<,>));
        }
    }
}
