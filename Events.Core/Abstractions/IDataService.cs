using Events.Core.Contracts;
using Events.Core.Models;
using System;
using System.Linq.Expressions;

namespace Events.Core.Abstractions
{
    public interface IDataService
    {
        Task RegisterParticipantAsync(string eventId, string participantId);
        Task<List<ParticipantWithDateDTO>> GetPagedParticipantsAsync(string eventId, int index, int pageSize);
        Task<ParticipantWithDateDTO?> GetEventParticipantByIdAsync(string eventId, string participantId);
        Task UnregisterParticipantAsync(string eventId, string participantId);
        Task<List<Event>> GetPagedEventsAsync(int index, int pageSize);
        Task<Event?> GetEventByIdAsync(string id);
        Task<Event?> GetEventByNameAsync(string name);
        Task CreateEventAsync(Event ev);
        Task UpdateEventAsync(string id, Event ev);
        Task DeleteEventAsync(string id);
        Task<List<Event>> GetPagedEventsByCriterionAsync(Expression<Func<Event, bool>> filter, int index, int pageSize);
        Task SaveEventImageAsync(string id, string webRootPath, string rootDir, Stream image);
        Task DeleteEventImageAsync(string id, string webRootPath, string rootDir);
        Task<byte[]> GetEventImageAsync(string id, string webRootPath, string rootDir);
    }
}
