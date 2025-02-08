using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using AutoMapper;

namespace Events.AuthenticationService.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginParticipantDTO> _loginValidator;
        private readonly IValidator<RegisterParticipantDTO> _registerValidator;
        private readonly IValidator<RefreshTokenDTO> _refreshValidator;
        private readonly IMapper _mapper;
        public AuthController(IAuthService authService, IValidator<LoginParticipantDTO> loginValidator,
            IValidator<RegisterParticipantDTO> registerValidator, IValidator<RefreshTokenDTO> refreshValidator, IMapper mapper)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _refreshValidator = refreshValidator;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginParticipantAsync([FromBody] LoginParticipantDTO loginDto)
        {
            var validationResult = _loginValidator.Validate(loginDto);
            if (validationResult.IsValid)
            {
                var res = await _authService.ValidateParticipantAsync(loginDto.Email, loginDto.Password);
                (string access, string refresh) = await _authService.CreateToken(res!, refresh: true);
                return StatusCode(200, new { access, refresh });
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokensAsync([FromBody] RefreshTokenDTO refreshDto)
        {
            var validationResult = _refreshValidator.Validate(refreshDto);
            if (validationResult.IsValid)
            {
                (string access, string refresh) = await _authService.UpdateToken(refreshDto.AccessToken, refreshDto.RefreshToken);
                return Ok(new { access, refresh });
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterParticipantDTO registerDto)
        {
            var validationResult = _registerValidator.Validate(registerDto);
            if (validationResult.IsValid)
            {
                var p = _mapper.Map<Participant>(registerDto);
                await _authService.RegisterParticipantAsync(p, registerDto.Password);
                return StatusCode(201, "New participant has been successfully registered.");
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}
