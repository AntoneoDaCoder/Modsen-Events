using Moq;
using Xunit;
using Events.Core.Abstractions;
using Events.Application.Exceptions;
using Events.Application.Services;
using Events.Core.Contracts;
using Events.Core.Models;
using FluentValidation;
using FluentValidation.Results;
using Events.Infrastructure.DbEntities;
using Events.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Events.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Events.Infrastructure.MappingProfiles;

public class EventParticipantServiceTests
{
    private readonly Mock<IValidator<(int, int)>> _pageValidatorMock;
    private readonly Mock<IValidator<Event>> _eventValidatorMock;
    private readonly IMapper _mapper;

    private readonly EventRepository _eventRepository;
    private readonly ParticipantRepository _participantRepository;
    private readonly EventParticipantRepository _eventParticipantRepository;
    private readonly UserManager<ParticipantEntity> _userManager;
    private readonly DataService _service;
    private readonly DbContextOptions<EventsDbContext> _dbOptions;

    public EventParticipantServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<EventsDbContext>()
              .UseInMemoryDatabase(Guid.NewGuid().ToString())
              .Options;
        var dbContext = new EventsDbContext(_dbOptions);
        dbContext.Database.EnsureCreated();

        var store = new UserStore<ParticipantEntity>(dbContext);
        var hasher = new PasswordHasher<ParticipantEntity>();
        var validators = new List<IUserValidator<ParticipantEntity>> { new UserValidator<ParticipantEntity>() };
        var passwordValidators = new List<IPasswordValidator<ParticipantEntity>> { new PasswordValidator<ParticipantEntity>() };

        var iOptions = new IdentityOptions();

        iOptions.Password.RequireDigit = true;
        iOptions.Password.RequiredLength = 8;
        iOptions.Password.RequireNonAlphanumeric = false;
        iOptions.User.RequireUniqueEmail = true;

        var options = Options.Create(iOptions);

        _userManager = new UserManager<ParticipantEntity>(
                store,
                options,
                hasher,
                validators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                new NullLogger<UserManager<ParticipantEntity>>()
            );

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InfrastructureMappingProfile>());
        _mapper = config.CreateMapper();

        _participantRepository = new ParticipantRepository(_userManager, _mapper);
        _eventParticipantRepository = new EventParticipantRepository(dbContext, _mapper);
        _eventRepository = new EventRepository(dbContext, _mapper);
        _pageValidatorMock = new();
        _eventValidatorMock = new();

        _service = new DataService
            (_eventParticipantRepository,
            _eventRepository,
            _pageValidatorMock.Object,
            _eventValidatorMock.Object,
            _participantRepository,
            It.IsAny<IImageRepository>()
            );
    }
    private async Task<string> CreateValidParticipantAsync()
    {
        var participant = Participant.CreateParticipant(Guid.Empty, "a", "b", DateOnly.ParseExact("01/01/1990", "dd/MM/yyyy"), "test@example.com");
        (string id, IEnumerable<string> errors) = await _participantRepository.CreateAsync(participant, "Password123!");
        return id;
    }
    private async Task<string> CreateValidEventAsync()
    {
        var eventId = Guid.NewGuid();
        var @event = Event.CreateEvent(eventId, "test", "test", DateOnly.MinValue, TimeOnly.MaxValue, "test", "test", 3);
        await _eventRepository.CreateAsync(@event);
        return eventId.ToString();
    }
    [Fact]
    public async Task RegisterParticipantAsync_ShouldThrowException_WhenEventDoesNotExist()
    {
        var id = await CreateValidParticipantAsync();
        await Assert.ThrowsAsync<ServiceException>(() => _service.RegisterParticipantAsync(Guid.NewGuid().ToString(), id));
    }
    [Fact]
    public async Task RegisterParticipantAsync_ShouldThrowException_WhenParticipantDoesNotExist()
    {
        var eventId = await CreateValidEventAsync();
        await Assert.ThrowsAsync<ServiceException>(() => _service.RegisterParticipantAsync(eventId, Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task RegisterParticipantAsync_ShouldNotThrow_WhenRegistrationSucceeds()
    {
        var pId = await CreateValidParticipantAsync();
        var eventId = await CreateValidEventAsync();
        try
        {
            await _service.RegisterParticipantAsync(eventId, pId);
            Assert.True(true);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exceptions, but got: " + ex.Message);
        }
    }

    [Fact]
    public async Task GetPagedParticipantsAsync_ShouldReturnParticipants_WhenValidationPasses()
    {
        var pId = await CreateValidParticipantAsync();
        var eventId = await CreateValidEventAsync();
        await _service.RegisterParticipantAsync(eventId, pId);

        _pageValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<(int, int)>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var result = await _service.GetPagedParticipantsAsync(eventId, 1, 10);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetPagedParticipantsAsync_ShouldThrowException_WhenValidationFails()
    {
        _pageValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<(int, int)>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<ValidationFailure> { new ValidationFailure("index", "Error") }));

        await Assert.ThrowsAsync<IncorrectDataException>(() => _service.GetPagedParticipantsAsync(Guid.NewGuid().ToString(), 1, 10));
    }

    [Fact]
    public async Task GetEventParticipantByIdAsync_ShouldReturnParticipant_WhenFound()
    {
        var pId = await CreateValidParticipantAsync();
        var eventId = await CreateValidEventAsync();
        await _service.RegisterParticipantAsync(eventId, pId);

        var result = await _service.GetEventParticipantByIdAsync(eventId, pId);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetEventParticipantByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _service.GetEventParticipantByIdAsync(Guid.NewGuid().ToString(), "participant-id");
        Assert.Null(result);
    }

    [Fact]
    public async Task UnregisterParticipantAsync_ShouldThrowException_WhenUnregistrationFails()
    {
        await Assert.ThrowsAsync<ServiceException>(() => _service.UnregisterParticipantAsync(Guid.NewGuid().ToString(), "participant-id"));
    }

    [Fact]
    public async Task UnregisterParticipantAsync_ShouldNotThrow_WhenUnregistrationSucceeds()
    {
        var pId = await CreateValidParticipantAsync();
        var eventId = await CreateValidEventAsync();
        await _service.RegisterParticipantAsync(eventId, pId);
        try
        {
            await _service.UnregisterParticipantAsync(eventId, pId);
            Assert.True(true);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exceptions, but got: " + ex.Message);
        }
    }
}
