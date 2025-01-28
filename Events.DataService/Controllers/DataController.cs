using Microsoft.AspNetCore.Mvc;
using Events.Application.Contracts;
using Events.Core.Abstractions;
using FluentValidation;
using Events.Core.Models;
using AutoMapper;
using Events.Application.Extensions.Misc;
using System.Linq.Expressions;
namespace Events.DataService.Controllers
{
    [ApiController]
    [Route("api")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IValidator<AddEventParticipantDTO> _addParticipantValidator;
        private readonly IValidator<CreateEventDTO> _createEventValidator;
        private readonly IValidator<IFormFile> _imgValidator;
        private readonly IWebHostEnvironment _webEnvironment;
        private readonly IMapper _mapper;
        public DataController
            (IDataService dataService, IValidator<AddEventParticipantDTO> addParticipant,
             IValidator<CreateEventDTO> createEvent, IValidator<IFormFile> imgValidator, IWebHostEnvironment web, IMapper mapper)
        {
            _dataService = dataService;
            _addParticipantValidator = addParticipant;
            _createEventValidator = createEvent;
            _mapper = mapper;
            _imgValidator = imgValidator;
            _webEnvironment = web;
        }

        [HttpPost]
        [Route("api/events/{eventId}/participants")]
        public async Task<IActionResult> RegisterEventParticipantAsync(string eventId, [FromBody] AddEventParticipantDTO newParticipant)
        {
            var validationResult = _addParticipantValidator.Validate(newParticipant);
            if (validationResult.IsValid)
            {
                await _dataService.RegisterParticipantAsync(eventId, newParticipant.ParticipantId);
                return StatusCode(201);
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpGet]
        [Route("api/events/{eventId}/participants")]
        public async Task<IActionResult> GetPagedEventParticipantsAsync([FromQuery] int pageSize, [FromQuery] int page, string eventId)
        {
            var participants = await _dataService.GetPagedParticipantsAsync(eventId, page, pageSize);
            if (participants.Count > 0)
                return StatusCode(200, participants);
            return StatusCode(200, "Couldn't find any participants.");
        }
        [HttpGet]
        [Route("api/events/{eventId}/participants/{participantId}")]
        public async Task<IActionResult> GetEventParticipantByIdAsync(string eventId, string participantId)
        {
            var participant = await _dataService.GetEventParticipantByIdAsync(eventId, participantId);
            if (participant is null)
                return StatusCode(200, "Event doesn't exist or participant isn't registered for this event.");
            return StatusCode(200, participant);
        }
        [HttpDelete]
        [Route("api/events/{eventId}/participants/{participantId}")]
        public async Task<IActionResult> UnregisterParticipantAsync(string eventId, string participantId)
        {
            await _dataService.UnregisterParticipantAsync(eventId, participantId);
            return StatusCode(200);
        }
        [HttpGet]
        [Route("api/events")]
        public async Task<IActionResult> GetPagedEventsAsync([FromQuery] int page, [FromQuery] int pageSize)
        {
            var events = await _dataService.GetPagedEventsAsync(page, pageSize);
            if (events.Count > 0)
                return StatusCode(200, events);
            return StatusCode(200, "There are no events.");
        }
        [HttpGet]
        [Route("api/events/{eventId}")]
        public async Task<IActionResult> GetEventByIdAsync(string eventId)
        {
            var @event = await _dataService.GetEventByIdAsync(eventId);
            if (@event is null)
                return StatusCode(200, "Such event doesn't exist.");
            return StatusCode(200, @event);
        }
        [HttpGet]
        [Route("api/events/")]
        public async Task<IActionResult> GetEventByNameAsync([FromQuery] string name)
        {
            var @event = await _dataService.GetEventByNameAsync(name);
            if (@event is null)
                return StatusCode(200, "Such event doesn't exist.");
            return StatusCode(200, @event);
        }
        [HttpPost]
        [Route("api/events")]
        public async Task<IActionResult> CreateEventAsync([FromBody] CreateEventDTO newEvent)
        {
            var validationResult = _createEventValidator.Validate(newEvent);
            if (validationResult.IsValid)
            {

                await _dataService.CreateEventAsync(_mapper.Map<Event>(newEvent));
                return StatusCode(201);
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpPut]
        [Route("api/events/{eventId}")]
        public async Task<IActionResult> UpdateEventAsync(string eventId, [FromBody] CreateEventDTO newEvent)
        {
            var validationResult = _createEventValidator.Validate(newEvent);
            if (validationResult.IsValid)
            {
                await _dataService.UpdateEventAsync(eventId, _mapper.Map<Event>(newEvent));
                return StatusCode(200);
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpDelete]
        [Route("api/events/{eventId}")]
        public async Task<IActionResult> DeleteEventAsync(string eventId)
        {
            await _dataService.DeleteEventAsync(eventId);
            return StatusCode(200);
        }
        [HttpGet]
        [Route("api/events/filter")]
        public async Task<IActionResult> GetPagedEventsByCriterionAsync
            ([FromQuery] DateOnly? date, [FromQuery] string? location, [FromQuery] string? category, [FromQuery] int page, [FromQuery] int pageSize)
        {
            if (string.IsNullOrEmpty(category) && !date.HasValue && string.IsNullOrEmpty(location))
                return BadRequest("You must specify at least 1 filter criterion.");

            List<Expression<Func<Event, bool>>> predicates = new();
            if (!string.IsNullOrEmpty(category))
                predicates.Add(ev => ev.Category == category);
            if (!string.IsNullOrEmpty(location))
                predicates.Add(ev => ev.Location == location);
            if (date.HasValue)
                predicates.Add(ev => ev.Date == date.Value);

            var res = await _dataService.GetPagedEventsByCriterionAsync(PredicateExtensions.CombinePredicates(predicates), page, pageSize);
            if (res.Count > 0)
                return StatusCode(200, res);
            return StatusCode(200, "No events meet specified criteria.");
        }
        [HttpPost]
        [Route("api/events/{eventId}")]
        public async Task<IActionResult> SaveEventImageAsync(string eventId, [FromBody] IFormFile img)
        {
            var validationResult = _imgValidator.Validate(img);
            if (validationResult.IsValid)
            {
                var rootDir = Environment.GetEnvironmentVariable("ROOT_DIR");
                using (var stream = img.OpenReadStream())
                {
                    await _dataService.SaveEventImageAsync(eventId, _webEnvironment.WebRootPath, rootDir!, stream);
                }
                return StatusCode(201);
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}
