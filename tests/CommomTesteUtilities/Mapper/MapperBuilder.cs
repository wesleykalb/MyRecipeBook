using MyRecipeBook.Application.Services.AutoMapper;
using AutoMapper;

namespace CommomTesteUtilities.Mapper
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            return new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping());
            }).CreateMapper();
        }
    }
}
