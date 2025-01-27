using AutoMapper;
using Events.Core.Models;
using Events.Application.Contracts;
namespace Events.Application.MappingProfiles
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile() 
        {
            CreateMap<RegisterParticipantDTO, Participant>()
                .ConstructUsing(p => Participant.CreateParticipant(Guid.NewGuid(), p.Name, p.Surname, DateOnly.ParseExact(p.BirthDate,"dd-MM-yyyy"), p.Email));
        }
    }
}
