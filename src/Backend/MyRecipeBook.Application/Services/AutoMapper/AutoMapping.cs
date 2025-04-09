using AutoMapper;
using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping() 
        {
            RequestToDomain();

        }

        private void RequestToDomain() 
        {
            CreateMap<RequestRegisterUserJson, Domain.Entities.User>()
                .ForMember(src => src.Password, opt => opt.Ignore());
        }
    }
}
