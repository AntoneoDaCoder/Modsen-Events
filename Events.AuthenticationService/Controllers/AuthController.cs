using Events.Core.Abstractions;
using Events.Core.Models;
using Events.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using AutoMapper;
using System.Text;

namespace Events.AuthenticationService.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginParticipantDTO> _loginValidator;
        private readonly IValidator<RegisterParticipantDTO> _registerValidator;
        private readonly IMapper _mapper;
        public AuthController(IAuthService authService, IValidator<LoginParticipantDTO> loginValidator,
            IValidator<RegisterParticipantDTO> registerValidator, IMapper mapper)
        {
            _authService = authService;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginParticipantAsync([FromBody] LoginParticipantDTO loginDto)
        {
            var validationResult = _loginValidator.Validate(loginDto);
            if (validationResult.IsValid)
            {
                (bool success, Participant? res) = await _authService.ValidateParticipantAsync(loginDto.Email, loginDto.Password);
                if (success)
                {
                    (string access, string refresh) = await _authService.CreateToken(res!, refresh: true);
                    return StatusCode(201, new { access, refresh });
                }
                return BadRequest("Participant doesn't exist or password is incorrect.");
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokensAsync([FromBody] RefreshTokenDTO refreshDto)
        {
            //if (!await _authService.ValidateUserAsync(loginDto))
            //    return Unauthorized();

            //return Ok(new { Token = _authService.GetUserToken() });
            return Ok();
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterParticipantDTO registerDto)
        {
            var validationResult = _registerValidator.Validate(registerDto);
            if (validationResult.IsValid)
            {
                var p = _mapper.Map<Participant>(registerDto);
                (bool success, IEnumerable<string> errors) = await _authService.RegisterParticipantAsync(p, registerDto.Password);
                if (success)
                    return StatusCode(201);
                StringBuilder sb = new StringBuilder();
                foreach (var error in errors)
                    sb.Append(error + "\n");
                return BadRequest(errors);
            }
            foreach (var error in validationResult.Errors)
                ModelState.TryAddModelError(error.PropertyName, error.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}
