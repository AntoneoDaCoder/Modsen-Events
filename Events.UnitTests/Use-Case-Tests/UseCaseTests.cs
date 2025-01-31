using Moq;
using Xunit;
using Events.Core.Abstractions;
using Events.Application.Exceptions;
using Events.Application.Services;
using Events.Core.Contracts;
using Events.Core.Models;
using FluentValidation;
using FluentValidation.Results;

public class EventParticipantServiceTests
{
    private readonly Mock<IEventParticipantRepository> _eventParticipantMock;
    private readonly Mock<IValidator<(int, int)>> _pageValidatorMock;
    private readonly DataService _service;

    public EventParticipantServiceTests()
    {
        _eventParticipantMock = new();
        _pageValidatorMock = new();
        _service = new DataService
            (_eventParticipantMock.Object,
            It.IsAny<IEventRepository>(),
            _pageValidatorMock.Object,
            It.IsAny<IValidator<Event>>(),
            It.IsAny<IParticipantRepository>(),
            It.IsAny<IImageRepository>()
            );
    }

    [Fact]
    public async Task RegisterParticipantAsync_ShouldThrowException_WhenRegistrationFails()
    {
        _eventParticipantMock.Setup(r => r.RegisterParticipantAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((false, new List<string> { "Error" }));

        await Assert.ThrowsAsync<ServiceException>(() => _service.RegisterParticipantAsync(Guid.NewGuid().ToString(), "participant-id"));
    }

    [Fact]
    public async Task RegisterParticipantAsync_ShouldNotThrow_WhenRegistrationSucceeds()
    {
        _eventParticipantMock.Setup(r => r.RegisterParticipantAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((true, new List<string>()));

        await _service.RegisterParticipantAsync(Guid.NewGuid().ToString(), "participant-id");
    }

    [Fact]
    public async Task GetPagedParticipantsAsync_ShouldReturnParticipants_WhenValidationPasses()
    {
        var participants = new List<ParticipantWithDateDTO> { new ParticipantWithDateDTO() };
        _pageValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<(int, int)>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _eventParticipantMock.Setup(r => r.GetPagedParticipantsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(participants);

        var result = await _service.GetPagedParticipantsAsync(Guid.NewGuid().ToString(), 1, 10);

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
        var participant = new ParticipantWithDateDTO();
        _eventParticipantMock.Setup(r => r.GetEventParticipantByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(participant);

        var result = await _service.GetEventParticipantByIdAsync(Guid.NewGuid().ToString(), "participant-id");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetEventParticipantByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _eventParticipantMock.Setup(r => r.GetEventParticipantByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((ParticipantWithDateDTO?)null);

        var result = await _service.GetEventParticipantByIdAsync(Guid.NewGuid().ToString(), "participant-id");

        Assert.Null(result);
    }

    [Fact]
    public async Task UnregisterParticipantAsync_ShouldThrowException_WhenUnregistrationFails()
    {
        _eventParticipantMock.Setup(r => r.UnregisterParticipantAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((false, new List<string> { "Error" }));

        await Assert.ThrowsAsync<ServiceException>(() => _service.UnregisterParticipantAsync(Guid.NewGuid().ToString(), "participant-id"));
    }

    [Fact]
    public async Task UnregisterParticipantAsync_ShouldNotThrow_WhenUnregistrationSucceeds()
    {
        _eventParticipantMock.Setup(r => r.UnregisterParticipantAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync((true, new List<string>()));

        await _service.UnregisterParticipantAsync(Guid.NewGuid().ToString(), "participant-id");
    }
}
