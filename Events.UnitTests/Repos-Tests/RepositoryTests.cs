using AutoMapper;
using Events.Core.Models;
using Events.Infrastructure.DbEntities;
using Events.Infrastructure.Repositories;
using Events.Infrastructure.MappingProfiles;
using Events.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;

namespace Events.Tests.Repositories
{
    public class ParticipantRepositoryTests
    {
        private readonly ParticipantRepository _repository;
        private readonly UserManager<ParticipantEntity> _userManager;
        private readonly IMapper _mapper;
        private readonly DbContextOptions<EventsDbContext> _dbOptions;

        public ParticipantRepositoryTests()
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

            _repository = new ParticipantRepository(_userManager, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsParticipant_WhenParticipantExists()
        {

            var participantId = Guid.NewGuid();
            var participantEntity = new ParticipantEntity
            {
                Id = participantId.ToString(),
                Email = "test@example.com",
                UserName = Guid.NewGuid().ToString(),
                Name = "a",
                Surname = "b",
                BirthDate = DateOnly.ParseExact("01/01/1990", "dd/MM/yyyy")
            };
            await _userManager.CreateAsync(participantEntity, "Password123!");

            var result = await _repository.GetByIdAsync(participantId);

            Assert.NotNull(result);
            Assert.Equal(participantId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenParticipantDoesNotExist()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsSuccess_WhenCreationIsSuccessful()
        {
            var participant = Participant.CreateParticipant(Guid.NewGuid(), "name", "surname", DateOnly.ParseExact("01/01/1990", "dd/MM/yyyy"), "xdd@gmail.com");
            var (isSuccess, errors) = await _repository.CreateAsync(participant, "Password123!");

            Assert.True(isSuccess);
            Assert.Empty(errors);
        }

        [Fact]
        public async Task CreateAsync_ReturnsFailure_WhenCreationFails()
        {
            var participant = Participant.CreateParticipant(Guid.NewGuid(), "name", "surname", DateOnly.ParseExact("01/01/1990", "dd/MM/yyyy"), "invalid email");

            var (isSuccess, errors) = await _repository.CreateAsync(participant, "Password123!");

            Assert.False(isSuccess);
            Assert.NotEmpty(errors);
        }
    }
}
