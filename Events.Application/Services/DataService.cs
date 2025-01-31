using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Application.Exceptions;
using FluentValidation;
using System.Linq.Expressions;
using Events.Core.Contracts;
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
            var (res, errors) = await _eventParticipantRepository.RegisterParticipantAsync(Guid.Parse(eventId), participantId);
            if (!res && errors.Any())
                throw EventsException.RaiseException<ServiceException>("Data service failed to register participant for event [internal error]", errors);
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
            var (res, errors) = await _eventParticipantRepository.UnregisterParticipantAsync(Guid.Parse(eventId), participantId);
            if (!res && errors.Any())
                throw EventsException.RaiseException<ServiceException>("Data service failed to unregister participant from event [internal error]", errors);
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
            {
                var (res, errors) = await _eventRepository.CreateAsync(ev);
                if (!res && errors.Any())
                    throw EventsException.RaiseException<ServiceException>("Data service failed to create new event [internal error]", errors);
            }
            else
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to create new event [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
        public async Task UpdateEventAsync(string id, Event ev)
        {
            var validationResult = await _eventValidator.ValidateAsync(ev);
            if (validationResult.IsValid)
            {
                var (res, errors) = await _eventRepository.UpdateAsync(Guid.Parse(id), ev);
                if (!res && errors.Any())
                    throw EventsException.RaiseException<ServiceException>("Data service failed to update event [internal error]", errors);
            }
            else
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to update event [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));
        }
        public async Task DeleteEventAsync(string id)
        {
            var (res, errors) = await _eventRepository.DeleteAsync(Guid.Parse(id));
            if (!res && errors.Any())
                throw EventsException.RaiseException<ServiceException>("Data service failed to delete event [internal error]", errors);
        }
        public async Task<Participant?> GetParticipantByEmailAsync(string email)
        {
            return await _participantRepository.GetByEmailAsync(email);
        }
        public async Task<List<Event>> GetPagedEventsByCriterionAsync(Expression<Func<Event, bool>> filter, int index, int pageSize)
        {
            var validationResult = await _pageValidator.ValidateAsync((index, pageSize));
            if (validationResult.IsValid)
            {
                if (filter is not null)
                    return await _eventRepository.GetByCriterionAsync(filter, index, pageSize);
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to get events by criterion [incorrect input data]", ["criterion not specified"]);
            }
            throw EventsException.RaiseException<IncorrectDataException>("Data service failed to get events by criterion [incorrect input data]", validationResult.Errors.Select(e => e.ErrorMessage));

        }
        public async Task SaveEventImageAsync(string id, string webRootPath, string rootDir, Stream image)
        {
            var ev = await GetEventByIdAsync(id);
            if (ev is null)
            {
                ev = await GetEventByNameAsync(id);
                if (ev is null)
                    throw EventsException.RaiseException<ServiceException>("Data service failed to upload image [internal error]", ["event with such id doesn't exist"]);
            }
            var fileName = $"event-{ev.Id}.png";
            var (path, uploadErrors) = await _imageRepository.SaveEventImageAsync(webRootPath, rootDir, fileName, image);
            if (!string.IsNullOrEmpty(path))
            {
                ev.AttachImage(path);
                await UpdateEventAsync(id, ev);
            }
            else
                throw EventsException.RaiseException<ServiceException>("Data service failed to upload image [internal error]", uploadErrors);
        }
        public async Task DeleteEventImageAsync(string id, string webRootPath, string rootDir)
        {
            if (string.IsNullOrEmpty(webRootPath) || string.IsNullOrEmpty(rootDir) || string.IsNullOrEmpty(id))
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to delete image [incorrect input data]", ["web root path, id and root dir must be specified"]);
            var evEntity = await GetEventByIdAsync(id);
            if (evEntity is null)
                throw EventsException.RaiseException<ServiceException>("Data service failed to delete image [internal error]", ["such event doesn't exist"]);
            if (string.IsNullOrEmpty(evEntity!.ImagePath))
                return;
            var fileName = $"event-{id}.png";
            var (res, errors) = await _imageRepository.DeleteEventImageAsync(webRootPath, rootDir, fileName);
            if (errors.Any() && !res)
                throw EventsException.RaiseException<ServiceException>("Data service failed to delete image [internal error]", errors);
        }
        public async Task<byte[]> GetEventImageAsync(string id, string webRootPath, string rootDir)
        {
            if (string.IsNullOrEmpty(webRootPath) || string.IsNullOrEmpty(rootDir) || string.IsNullOrEmpty(id))
                throw EventsException.RaiseException<IncorrectDataException>("Data service failed to download image [incorrect input data]", ["web root path, id and root dir must be specified"]);

            var evEntity = await GetEventByIdAsync(id);
            if (evEntity is null)
                throw EventsException.RaiseException<ServiceException>("Data service failed to download image [internal error]", ["such event doesn't exist"]);

            if (string.IsNullOrEmpty(evEntity!.ImagePath))
                return [];

            var fileName = $"event-{id}.png";
            var (img, errors) = await _imageRepository.GetEventImageAsync(webRootPath, rootDir, fileName);
            if (errors.Any())
                throw EventsException.RaiseException<ServiceException>("Data service failed to download image [internal error]", errors);
            return img;
        }
    }
}
