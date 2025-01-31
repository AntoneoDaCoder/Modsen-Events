using AutoMapper;
using Events.Core.Contracts;
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
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
             .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
             .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));

            CreateMap<Event, EventEntity>()
                .ForMember(dest => dest.EventParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Condition((src, dest, member) => member == Guid.Empty));

            CreateMap<ParticipantEntity, Participant>()
                .ConstructUsing(p => Participant.CreateParticipant(Guid.Parse(p.Id), p.Name, p.Surname, p.BirthDate, p.Email!, p.RefreshToken, p.RefreshTokenExpiryTime));
            CreateMap<Participant, ParticipantEntity>()
                .ForMember(dest => dest.EventParticipants, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Condition((src, dest, member) => member == string.Empty))
                .ForMember(dest => dest.UserName, opt =>
                    opt.Condition((src, dest) => string.IsNullOrWhiteSpace(dest.UserName)))
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));

            CreateMap<EventParticipantEntity, ParticipantWithDateDTO>()
                .ForMember(dest => dest.Participant,
                opt => opt.MapFrom(
                    src =>
                    Participant.CreateParticipant(Guid.Parse(src.Participant.Id), src.Participant.Name, src.Participant.Surname, src.Participant.BirthDate, src.Participant.Email!)));

        }
    }
}
