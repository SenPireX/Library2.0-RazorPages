using AutoMapper;
using Library.Application.Model;

namespace Library.Application.Dto;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LibraryDto, Model.Library>();
        CreateMap<Model.Library, LibraryDto>();
        CreateMap<LoanDto, Loan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Loan, LoanDto>();
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Salt, opt => opt.Ignore())
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.User));
    }
}