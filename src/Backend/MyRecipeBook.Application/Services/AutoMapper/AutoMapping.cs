using AutoMapper;
using Microsoft.Data.SqlClient;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using Sqids;

namespace MyRecipeBook.Application.Services.AutoMapper
{
    public class AutoMapping : Profile
    {
        private readonly SqidsEncoder<long> _sqidsEncoder;
        public AutoMapping(SqidsEncoder<long> sqidsEncoder) 
        {
            _sqidsEncoder = sqidsEncoder;

            RequestToDomain();
            DomainToResponse();
        }
        private void RequestToDomain() 
        {
            CreateMap<RequestRegisterUserJson, Domain.Entities.User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<RequestRecipeJson, Domain.Entities.Recipe>()
                .ForMember(dest => dest.Instructions, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients.Distinct()))
                .ForMember(dest => dest.DishTypes, opt => opt.MapFrom(src => src.DishTypes.Distinct()));

            CreateMap<string, Domain.Entities.Ingredient>()
                .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src));
            
            CreateMap<Communication.Enums.DishType, Domain.Entities.DishType>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));
            
            CreateMap<RequestInstructionJson, Domain.Entities.Instruction>();
        }

        private void DomainToResponse()
        {
            CreateMap<Domain.Entities.User, ResponseUserProfileJson>();
            CreateMap<Domain.Entities.Recipe, ResponseRegisteredRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(src => _sqidsEncoder.Encode(src.id)));
            CreateMap<Domain.Entities.Recipe, ResponseShortRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(src => _sqidsEncoder.Encode(src.id)))
                .ForMember(dest => dest.AmountIngredients, config => config.MapFrom(src => src.Ingredients.Count));
            CreateMap<Domain.Entities.Recipe, ResponseRecipeJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(src => _sqidsEncoder.Encode(src.id)))
                .ForMember(dest => dest.DishTypes, config => config.MapFrom(src => src.DishTypes.Select(x => x.Type)));
            CreateMap<Domain.Entities.Ingredient, ResponseIngredientJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(src => _sqidsEncoder.Encode(src.id)));
            CreateMap<Domain.Entities.Instruction, ResponseInstructionJson>()
                .ForMember(dest => dest.Id, config => config.MapFrom(src => _sqidsEncoder.Encode(src.id)));
        }
    }
}
