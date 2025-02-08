using Microsoft.AspNetCore.Mvc;
using Events.Application.Contracts;
using Events.Core.Abstractions;
using FluentValidation;
using Events.Core.Models;
using AutoMapper;
using System.Security.Cryptography;
using System.Net.Http.Headers;
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
        private readonly IMapper _mapper;
        public DataController
            (IDataService dataService, IValidator<AddEventParticipantDTO> addParticipant,
             IValidator<CreateEventDTO> createEvent, IValidator<IFormFile> imgValidator, IMapper mapper)
        {
            _dataService = dataService;
            _addParticipantValidator = addParticipant;
            _createEventValidator = createEvent;
            _mapper = mapper;
            _imgValidator = imgValidator;
        }
        [HttpGet]
        [Route("{eventId:guid}/participants")]
        public async Task<IActionResult> GetPagedEventParticipantsAsync([FromQuery(Name = "pageSize")] int pageSize, [FromQuery(Name = "page")] int page, Guid eventId)
        {
            var participants = await _dataService.GetPagedParticipantsAsync(eventId.ToString(), page, pageSize);
            return StatusCode(200, participants);
        }
        [HttpGet]
        [Route("{eventId:guid}/participants/{participantId:guid}")]
        public async Task<IActionResult> GetEventParticipantByIdAsync(Guid eventId, Guid participantId)
        {
            var participant = await _dataService.GetEventParticipantByIdAsync(eventId.ToString(), participantId.ToString());
            return StatusCode(200, participant);
        }
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetPagedEventsAsync([FromQuery(Name = "page")] int page, [FromQuery(Name = "pageSize")] int pageSize)
        {
            var events = await _dataService.GetPagedEventsAsync(page, pageSize);
            return StatusCode(200, events);
        }
        [HttpGet]
        [Route("event/{eventId:guid}")]
        public async Task<IActionResult> GetEventByIdAsync(Guid eventId)
        {
            var @event = await _dataService.GetEventByIdAsync(eventId.ToString());
            return StatusCode(200, @event);
        }
        [HttpGet]
        [Route("event")]
        public async Task<IActionResult> GetEventByNameAsync([FromQuery(Name = "name")] string name)
        {
            var @event = await _dataService.GetEventByNameAsync(name);
            return StatusCode(200, @event);
        }
        [HttpGet]
        [Route("participant")]
        public async Task<IActionResult> GetParticipantByEmail([FromQuery(Name = "email")] string email)
        {
            var participant = await _dataService.GetParticipantByEmailAsync(email);
            return StatusCode(200, participant);
        }
        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> GetPagedEventsByCriterionAsync
            ([FromQuery] string? date, [FromQuery] string? location, [FromQuery] string? category, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var res = await _dataService.GetPagedEventsByCriterionAsync(category, location, date, page, pageSize);
            return StatusCode(200, res);
        }

        [HttpGet]
        [Route("{eventId:guid}/image")]
        public async Task<IActionResult> GetEventImageAsync(Guid eventId)
        {
            var rootDir = Environment.GetEnvironmentVariable("ROOT_DIR");
            var webRootPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");
            byte[] imgBytes = await _dataService.GetEventImageAsync(eventId.ToString(), webRootPath!, rootDir!);
            if (imgBytes.Length > 0)
            {
                string eTag = $"\"{Convert.ToBase64String(MD5.HashData(imgBytes))}\"";
                var clientETag = Request.Headers.IfNoneMatch.FirstOrDefault();
                if (clientETag != null && clientETag == eTag)
                    return StatusCode(304);
                Response.Headers.ETag = eTag;
                Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromMinutes(10)
                }.ToString();   
            }
            return File(imgBytes, "image/png");
        }
        [HttpPost]
        [Route("{eventId:guid}/participants")]
        public async Task<IActionResult> RegisterEventParticipantAsync(Guid eventId, [FromBody] AddEventParticipantDTO newParticipant)
        {
            var validationResult = _addParticipantValidator.Validate(newParticipant);
            if (validationResult.IsValid)
            {
                await _dataService.RegisterParticipantAsync(eventId.ToString(), newParticipant.ParticipantId);
                return StatusCode(201, "Participant has been registered for event");
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateEventAsync([FromBody] CreateEventDTO newEvent)
        {
            var validationResult = _createEventValidator.Validate(newEvent);
            if (validationResult.IsValid)
            {
                await _dataService.CreateEventAsync(_mapper.Map<Event>(newEvent));
                return StatusCode(201, "New event has been added.");
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpPost]
        [Route("{eventId:guid}")]
        public async Task<IActionResult> SaveEventImageAsync(Guid eventId, [FromForm(Name = "Image")] IFormFile image)
        {
            var validationResult = _imgValidator.Validate(image);
            if (validationResult.IsValid)
            {
                var rootDir = Environment.GetEnvironmentVariable("ROOT_DIR");
                var webRootPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");
                using (var stream = image.OpenReadStream())
                {
                    await _dataService.SaveEventImageAsync(eventId.ToString(), webRootPath!, rootDir!, stream);
                }
                return StatusCode(201, "Image has been uploaded.");
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpPut]
        [Route("{eventId:guid}")]
        public async Task<IActionResult> UpdateEventAsync(Guid eventId, [FromBody] CreateEventDTO newEvent)
        {
            var validationResult = _createEventValidator.Validate(newEvent);
            if (validationResult.IsValid)
            {
                await _dataService.UpdateEventAsync(eventId.ToString(), _mapper.Map<Event>(newEvent));
                return StatusCode(200, "Event successfully updated.");
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpDelete]
        [Route("{eventId:guid}/participants/{participantId:guid}")]
        public async Task<IActionResult> UnregisterParticipantAsync(Guid eventId, Guid participantId)
        {
            await _dataService.UnregisterParticipantAsync(eventId.ToString(), participantId.ToString());
            return StatusCode(200, "Successfully unregistered participant from event.");
        }
        [HttpDelete]
        [Route("{eventId:guid}")]
        public async Task<IActionResult> DeleteEventAsync(Guid eventId)
        {
            var rootDir = Environment.GetEnvironmentVariable("ROOT_DIR");
            var webRootPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");
            await _dataService.DeleteEventImageAsync(eventId.ToString(), webRootPath!, rootDir!);
            await _dataService.DeleteEventAsync(eventId.ToString());
            return StatusCode(200, "Event successfully deleted.");
        }

    }
}
