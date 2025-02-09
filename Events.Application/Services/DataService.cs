using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Application.Exceptions;
using FluentValidation;
using System.Linq.Expressions;
using Events.Core.Contracts;
using Events.Application.Extensions.Misc;
namespace Events.Application.Services
{
    public class DataService : IDataService
    {
        private readonly IEventParticipantRepository _eventParticipantRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IValidator<(int, int)> _pageValidator;
        private readonly IValidator<Event> _eventValidator;
        public DataService(IEventParticipantRepository eventParticipantRepository,
            IEventRepository eventRepository, IValidator<(int, int)> pageValidator, IValidator<Event> eventValidator
            , IParticipantRepository participantRepository, IImageRepository imageRepository)
        {
            _eventParticipantRepository = eventParticipantRepository;
            _eventRepository = eventRepository;
            _pageValidator = pageValidator;
            _eventValidator = eventValidator;
            _imageRepository = imageRepository;
            _participantRepository = participantRepository;
        }
        public async Task RegisterParticipantAsync(string eventId, string participantId)
        {
            var participant = await _participantRepository.GetByIdAsync(Guid.Parse(participantId));
            (Event? ev, int? count) = await _eventRepository.GetByIdWithParticipantsAsync(Guid.Parse(eventId));
            if (participant is null)
                throw EventsException.RaiseException<ServiceException>("Data service failed to register participant for event [internal error]", ["such participant doesn't exist"]);
            if (ev is null)
                throw EventsException.RaiseException<ServiceException>("Data service failed to register participant for event [internal error]", ["such event doesn't exist"]);
            if (count + 1 > ev.MaxParticipants)
                throw EventsException.RaiseException<ServiceException>("Data service failed to register participant for event [internal error]", ["event has no vacant slots left"]);

            await _eventParticipantRepository.RegisterParticipantAsync(Guid.Parse(eventId), participantId);
        }
        public async Task<List<ParticipantWithDateDTO>> GetPagedParticipantsAsync(string eventId, int index, int pageSize)
        {
            var validationResult = await _pageValidator.ValidateAsync((index, pageSize));
            if (validationResult.IsValid)
                return await _eventParticipantRepository.GetPagedParticipantsAsync(Guid.Parse(eventId), index, pageSize);
            throw EventsException.RaiseException<IncorrectDataException>("Data service failed to get participants [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
        public async Task<ParticipantWithDateDTO?> GetEventParticipantByIdAsync(string eventId, string participantId)
        {
            return await _eventParticipantRepository.GetEventParticipantByIdAsync(Guid.Parse(eventId), participantId);
        }
        public async Task UnregisterParticipantAsync(string eventId, string participantId)
        {
            var eventParticipant = await _eventParticipantRepository.GetEventParticipantByIdAsync(Guid.Parse(eventId), participantId);
            if (eventParticipant is not null)
                await _eventParticipantRepository.UnregisterParticipantAsync(Guid.Parse(eventId), participantId);
            else
                throw EventsException.RaiseException<ServiceException>("Data service failed to unregister participant from event [internal error]", ["participant didn't register for this event"]);
        }
        public async Task<List<Event>> GetPagedEventsAsync(int index, int pageSize)
        {
            var validationResult = await _pageValidator.ValidateAsync((index, pageSize));
            if (validationResult.IsValid)
                return await _eventRepository.GetPagedAsync(index, pageSize);
            throw EventsException.RaiseException<IncorrectDataException>("Data service failed to get events [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
        public async Task<Event?> GetEventByIdAsync(string id)
        {
            return await _eventRepository.GetByIdAsync(Guid.Parse(id));
        }
        public async Task<Event?> GetEventByNameAsync(string name)
        {
            return await _eventRepository.GetByNameAsync(name);
        }
        public async Task CreateEventAsync(Event ev)
        {
            var validationResult = await _eventValidator.ValidateAsync(ev);
            if (validationResult.IsValid)
                await _eventRepository.CreateAsync(ev);
            else
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to create new event [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
        public async Task UpdateEventAsync(string id, Event ev)
        {
            var validationResult = await _eventValidator.ValidateAsync(ev);
            if (validationResult.IsValid)
            {
                var target = await _eventRepository.GetByIdAsync(Guid.Parse(id));
                if (target != null)
                    await _eventRepository.UpdateAsync(target, ev);
                else
                    throw EventsException.RaiseException<ServiceException>("Data service failed to update event [internal error]", ["such event doesn't exist"]);
            }
            else
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to update event [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
        public async Task DeleteEventAsync(string id)
        {
            var target = await _eventRepository.GetByIdAsync(Guid.Parse(id));
            if (target != null)
                await _eventRepository.DeleteAsync(target);
            else
                throw EventsException.RaiseException<ServiceException>("Data service failed to delete event [internal error]", ["such event doesn't exist"]);
        }
        public async Task<Participant?> GetParticipantByEmailAsync(string email)
        {
            return await _participantRepository.GetByEmailAsync(email);
        }
        public async Task<List<Event>> GetPagedEventsByCriterionAsync(string? category, string? location, string? date, int index, int pageSize)
        {
            if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(date) && string.IsNullOrEmpty(location))
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to get events by criterion [incorrect input data]", ["at least one criterion must be specified"]);

            var validationResult = await _pageValidator.ValidateAsync((index, pageSize));
            if (validationResult.IsValid)
            {
                List<Expression<Func<Event, bool>>> predicates = new();
                if (!string.IsNullOrEmpty(category))
                    predicates.Add(ev => ev.Category == category);
                if (!string.IsNullOrEmpty(location))
                    predicates.Add(ev => ev.Location == location);
                if (!string.IsNullOrEmpty(date))
                    predicates.Add(ev => ev.Date == DateOnly.ParseExact(date, "dd-MM-yyyy"));

                var filter = PredicateExtensions.CombinePredicates(predicates);
                return await _eventRepository.GetByCriterionAsync(filter, index, pageSize);
            }
            throw EventsException.RaiseException<IncorrectDataException>("Data service failed to get events by criterion [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
        public async Task SaveEventImageAsync(string id, string webRootPath, string rootDir, Stream image)
        {
            var ev = await GetEventByIdAsync(id);
            if (ev is null)
                throw EventsException.RaiseException<ServiceException>("Data service failed to upload image [internal error]", ["event with such id doesn't exist"]);
            var fileName = $"event-{ev.Id}.png";
            var path = await _imageRepository.SaveEventImageAsync(webRootPath, rootDir, fileName, image);
            ev.AttachImage(path);
            await UpdateEventAsync(id, ev);
        }
        public async Task DeleteEventImageAsync(string id, string webRootPath, string rootDir)
        {
            if (string.IsNullOrEmpty(webRootPath) || string.IsNullOrEmpty(rootDir) || string.IsNullOrEmpty(id))
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to delete image [incorrect input data]", ["web root path, id and root dir must be specified"]);
            var evEntity = await GetEventByIdAsync(id);
            if (evEntity is null)
                throw EventsException.RaiseException<ServiceException>("Data service failed to delete image [internal error]", ["such event doesn't exist"]);
            if (string.IsNullOrEmpty(evEntity.ImagePath))
                return;
            var fileName = $"event-{id}.png";
            await _imageRepository.DeleteEventImageAsync(webRootPath, rootDir, fileName);
        }
        public async Task<byte[]> GetEventImageAsync(string id, string webRootPath, string rootDir)
        {
            if (string.IsNullOrEmpty(webRootPath) || string.IsNullOrEmpty(rootDir) || string.IsNullOrEmpty(id))
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to download image [incorrect input data]", ["web root path, id and root dir must be specified"]);
            var evEntity = await GetEventByIdAsync(id);
            if (evEntity is null)
                throw EventsException.RaiseException<ServiceException>("Data service failed to download image [internal error]", ["such event doesn't exist"]);
            if (string.IsNullOrEmpty(evEntity.ImagePath))
                return [];
            var fileName = $"event-{id}.png";
            return await _imageRepository.GetEventImageAsync(webRootPath, rootDir, fileName);
        }
    }
}
