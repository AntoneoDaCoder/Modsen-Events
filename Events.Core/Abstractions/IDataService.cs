﻿using Events.Core.Contracts;
using Events.Core.Models;
namespace Events.Core.Abstractions
{
    public interface IDataService
    {
        Task RegisterParticipantAsync(string eventId, string participantId);
        Task<List<ParticipantWithDateDTO>> GetPagedParticipantsAsync(string eventId, int index, int pageSize);
        Task<ParticipantWithDateDTO?> GetEventParticipantByIdAsync(string eventId, string participantId);
        Task<Participant?> GetParticipantByEmailAsync(string email);
        Task UnregisterParticipantAsync(string eventId, string participantId);
        Task<List<Event>> GetPagedEventsAsync(int index, int pageSize);
        Task<Event?> GetEventByIdAsync(string id);
        Task<Event?> GetEventByNameAsync(string name);
        Task CreateEventAsync(Event ev);
        Task UpdateEventAsync(string id, Event ev);
        Task DeleteEventAsync(string id);
        Task<List<Event>> GetPagedEventsByCriterionAsync(string? category, string? location, string? date, int index, int pageSize);
        Task SaveEventImageAsync(string id, string webRootPath, string rootDir, Stream image);
        Task DeleteEventImageAsync(string id, string webRootPath, string rootDir);
        Task<byte[]> GetEventImageAsync(string id, string webRootPath, string rootDir);
    }
}
