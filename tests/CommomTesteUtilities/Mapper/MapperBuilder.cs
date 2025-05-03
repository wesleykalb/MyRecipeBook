using MyRecipeBook.Application.Services.AutoMapper;
using AutoMapper;
using CommomTesteUtilities.IdEncryption;

namespace CommomTesteUtilities.Mapper
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            var idEncrypter = IdEncrypterBuilder.Build();

            return new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping(idEncrypter));
            }).CreateMapper();
        }
    }
}
