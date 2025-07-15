using Microsoft.AspNetCore.Mvc;
using SingularExpress.Interfaces;
using SingularExpress.Models.Models;
using SingularExpress.Dto;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using SingularExpress.Models;
using Microsoft.EntityFrameworkCore; 






namespace SingularExpress.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserController> _logger;
        private readonly ModelDbContext _context;
        private readonly IEmailService _emailService;

        public UserController(
            IUserRepository userRepository,
            ILogger<UserController> logger,
            ModelDbContext context,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        private bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   email.EndsWith("@singular.co.za", StringComparison.OrdinalIgnoreCase) &&
                   email.IndexOf('@') > 0 &&
                   email.Substring(0, email.IndexOf('@')).Any(char.IsLetter);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();

            var dtos = users.Select(u => new UserDto
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                Password = string.Empty,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CreatedOn = u.CreatedOn,
                ModifiedOn = u.ModifiedOn
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(Guid id)
        {
            var user = _userRepository.GetUser(id);
            if (user == null) return NotFound();

            var dto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Password = string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedOn = user.CreatedOn,
                ModifiedOn = user.ModifiedOn
            };

            return Ok(dto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult CreateUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!IsValidEmail(userDto.Email))
                return BadRequest("Email must be a valid '@singular.co.za' address.");

            if (!IsValidPassword(userDto.Password))
                return BadRequest("Password does not meet complexity requirements.");

            if (_userRepository.GetUsers()
                .Any(u => u.Email.Equals(userDto.Email, StringComparison.OrdinalIgnoreCase)))
                return Conflict("Email is already registered.");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = userDto.UserName,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                CreatedOn = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

            var created = _userRepository.CreateUser(user);
            if (!created)
            {
                _logger.LogError("An error occurred while saving the user.");
                return StatusCode(500, "An error occurred while saving the user.");
            }

            userDto.UserId = user.UserId;
            userDto.CreatedOn = user.CreatedOn;
            userDto.ModifiedOn = user.ModifiedOn;
            userDto.Password = string.Empty;

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, userDto);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public IActionResult UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != userDto.UserId)
                return BadRequest("User ID mismatch.");

            var existingUser = _userRepository.GetUser(id);
            if (existingUser == null) return NotFound();

            if (!IsValidEmail(userDto.Email))
                return BadRequest("Email must be a valid '@singular.co.za' address.");

            if (!string.IsNullOrEmpty(userDto.Password) && !IsValidPassword(userDto.Password))
                return BadRequest("Password does not meet complexity requirements.");

            if (_userRepository.GetUsers()
                .Any(u => u.Email.Equals(userDto.Email, StringComparison.OrdinalIgnoreCase) && u.UserId != id))
                return Conflict("Email is already registered by another user.");

            existingUser.UserName = userDto.UserName;
            existingUser.Email = userDto.Email;
            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.ModifiedOn = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, userDto.Password);
            }

            var updated = _userRepository.UpdateUser(existingUser);
            if (!updated)
            {
                _logger.LogError("An error occurred while updating the user.");
                return StatusCode(500, "An error occurred while updating the user.");
            }

            userDto.Password = string.Empty;
            userDto.CreatedOn = existingUser.CreatedOn;
            userDto.ModifiedOn = existingUser.ModifiedOn;

            return Ok(userDto);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _userRepository.GetUser(id);
            if (user == null) return NotFound();

            var deleted = _userRepository.DeleteUser(user);
            if (!deleted)
            {
                _logger.LogError("An error occurred while deleting the user.");
                return StatusCode(500, "An error occurred while deleting the user.");
            }

            return NoContent();
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _userRepository.GetUserByEmail(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                var timeLeft = user.LockoutEnd.Value - DateTime.UtcNow;
                return Unauthorized($"Account locked. Please try again in {timeLeft.Minutes} minutes and {timeLeft.Seconds} seconds.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;
                int attemptsLeft = 3 - user.FailedLoginAttempts;

                if (user.FailedLoginAttempts >= 3)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(5);
                    _userRepository.UpdateUser(user);
                    return Unauthorized("Account locked due to multiple failed attempts. Please try again in 5 minutes.");
                }

                _userRepository.UpdateUser(user);
                return Unauthorized($"Invalid email or password. {attemptsLeft} attempt(s) remaining before account is locked.");
            }

            
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            _userRepository.UpdateUser(user);

            return Ok("Login successful.");
        }
        [HttpPost("forgot-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                var user = _userRepository.GetUserByEmail(dto.Email);
                if (user == null)
                {
               
                    return Ok("If an account with this email exists, a reset code has been sent.");
                }

                string otp = new Random().Next(0, 10000).ToString("D4");

                _context.PasswordResetTokens.Add(new PasswordResetToken
                {
                    Email = dto.Email,
                    Otp = otp,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                });
                await _context.SaveChangesAsync();

                await _emailService.SendPasswordResetEmailAsync(
                    dto.Email,
                    otp,
                    $"{user.FirstName} {user.LastName}"
                );

                return Ok("If an account with this email exists, a reset code has been sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost("verify-otp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
       
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Otp))
            {
                return BadRequest("Email and OTP must be provided.");
            }

         
            var token = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Email == dto.Email && t.Otp == dto.Otp);


            if (token == null || token.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new { valid = false, message = "Invalid or expired OTP." });
            }

            _context.PasswordResetTokens.Remove(token);
            await _context.SaveChangesAsync();

            return Ok(new { valid = true, message = "OTP is valid." });
        }
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.NewPassword))
                return BadRequest("Email and new password must be provided.");

            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
            if (!regex.IsMatch(model.NewPassword))
                return BadRequest("Password must be at least 8 characters long and include uppercase, lowercase, digit, and special character.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
                return NotFound("User not found.");

            var passwordVerificationResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, model.NewPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                return BadRequest("New password cannot be the same as the old password.");
            }

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, model.NewPassword);
            await _context.SaveChangesAsync();

            await _emailService.SendPasswordResetConfirmationEmailAsync(user.Email, user.UserName);

            return Ok("Password updated successfully.");
        }

    }
}
