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
            .ForMember(l => l.Id, 
                opt => opt.MapFrom(l => l.Id == default ? Guid.NewGuid() : l.Id));
        CreateMap<Loan, LoanDto>();
    }
}