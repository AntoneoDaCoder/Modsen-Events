using AutoMapper;
using Events.Core.Models;
using Events.Infrastructure.DbEntities;
namespace Events.Infrastructure.MappingProfiles
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<EventEntity, Event>()
                .ConstructUsing(e => Event.CreateEvent(e.Id, e.Name, e.Description, e.Date, e.Time, e.Location, e.Category, e.MaxParticipants))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath));
            CreateMap<Event, EventEntity>()
                .ForMember(dest => dest.EventParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Condition((src, dest, member) => member != Guid.Empty));

            CreateMap<ParticipantEntity, Participant>()
                .ConstructUsing(p => Participant.CreateParticipant(Guid.Parse(p.Id), p.UserName!, p.Surname, p.BirthDate, p.Email!));
            CreateMap<Participant, ParticipantEntity>()
                .ForMember(dest => dest.EventParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
                 .ForMember(dest => dest.Id, opt => opt.Condition((src, dest, member) => member != string.Empty));
        }
    }
}
