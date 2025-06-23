
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using System.Text.Json;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Queries;
using Users.Application.Validators;
using Users.Domain.Entities;
using Users.Infrastructure.Data;
using static MassTransit.ValidationResultExtensions;

namespace Users.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public readonly Serilog.ILogger logger;
        public UserDbContext _UserDbContext { get; set; }
        public UserController(IMediator mediator, Serilog.ILogger logger, UserDbContext userDbContext)
        {
            this.mediator = mediator;
            this.logger = logger;
            _UserDbContext = userDbContext;
        }

        [HttpGet("GetUsersProfile")]
        public async Task<ActionResult<UserProfileDTO>> GetUser()
        {
            logger.Information("GetUser method called.");

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.Warning("GetUser called but userId is null or empty.");
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new GetUserProfileQuery(userId));

            if (result == null)
            {
                logger.Warning("User with ID {UserId} not found.", userId);
                return NotFound("There is no information for the given user ID.");
            }

            logger.Information("User with ID {UserId} retrieved successfully.", userId);
            return Ok(result);


        }

        [HttpPost("ChangeUserSettings")]
        public async Task<ActionResult> ChangeUserSettings([FromBody] ChangeProfileInformationDTO model)
        {
            var validator = new ChangeProfileInformationDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    logger.Information("ChangeUserSettings was called but no user ID was found in the claims.");
                    return Unauthorized("User ID not found.");
                }

                logger.Information("Starting ChangeUserSettings for user {UserId}.", userId);

                model.Id = userId;
                await mediator.Send(new ChangeUserInformationCommand(model));

                logger.Information("Successfully changed settings for user {UserId}.", userId);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error changing settings for user {UserId}.", model.Id);
                return StatusCode(500, "An error occurred while changing user settings.");
            }

        }

        [HttpPost("UploadProfilePhoto")]
        public async Task<ActionResult<UserProfileDTO>> UploadProfilePhoto([FromBody] ProfilePhotoDTO model)
        {
          
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            model.Id = userId;
            if (string.IsNullOrEmpty(userId))
            {
                logger.Information("UploadProfilePhoto called but user ID is missing.");
                return Unauthorized("User ID is required.");
            }

            await mediator.Send(new ChangeUserAvatarCommand(model));

            logger.Information("Profile photo updated successfully for user {UserId}. Fetching updated user profile.", userId);

            var result = await mediator.Send(new GetUserProfileQuery(model.Id));

            if (result == null)
            {
                logger.Warning("Failed to fetch updated profile for user {UserId} after uploading photo.", userId);
                return NotFound("User profile not found.");
            }

            logger.Information("Successfully retrieved updated profile for user {UserId} after photo upload.", userId);
            return Ok(result);

            //try
            //{
            //    logger.Information("Attempting to upload profile photo for user {UserId}.", userId);

            //    model.Id = userId;

            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri("http://127.0.0.1:5000");

            //        var imageBytes = Convert.FromBase64String(model.Avatar);  
            //        using (var content = new MultipartFormDataContent())
            //        {
            //            var imageContent = new ByteArrayContent(imageBytes);
            //            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            //            content.Add(imageContent, "image", "avatar.jpg");

            //            var response = await client.PostAsync("/detect_person", content);

            //            if (response.IsSuccessStatusCode)
            //            {
            //                var responseContent = await response.Content.ReadAsStringAsync();

            //                var jsonDoc = JsonDocument.Parse(responseContent);
            //                bool isPerson = jsonDoc.RootElement.GetProperty("is_person").GetBoolean();

            //                if (!isPerson)
            //                {
            //                    logger.Warning("Avatar photo for user {UserId} is not detected as a person.", userId);
            //                    return BadRequest("Uploaded image is not recognized as a person.");
            //                }

            //                await mediator.Send(new ChangeUserAvatarCommand(model));

            //                logger.Information("Profile photo updated successfully for user {UserId}. Fetching updated user profile.", userId);

            //                var result = await mediator.Send(new GetUserProfileQuery(model.Id));

            //                if (result == null)
            //                {
            //                    logger.Warning("Failed to fetch updated profile for user {UserId} after uploading photo.", userId);
            //                    return NotFound("User profile not found.");
            //                }

            //                logger.Information("Successfully retrieved updated profile for user {UserId} after photo upload.", userId);
            //                return Ok(result);
            //            }
            //            else
            //            {
            //                logger.Warning("Failed to validate avatar photo for user {UserId}. Response: {Response}", userId, response.ReasonPhrase);
            //                return BadRequest("Failed to validate the avatar photo.");
            //            }
            //        }
            //    }

               
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex, "An error occurred while uploading profile photo for user {UserId}.", userId);
            //    return BadRequest("Something went wrong.");
            //}
        }

        [AllowAnonymous]
        [HttpPost("GetSomeonesProfile")]
        public async Task<ActionResult<GiveSmbProfileDTO>> GetSomeonesProfile([FromBody] SomeonesProfileDTO request)
        {
            var validator = new SomeonesProfileDTOValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }

            try
            {

                logger.Information("Fetching user data for ProfileId {ProfileId}.", request.ProfileId);


                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("UploadProfilePhoto called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }


                var result = await mediator.Send(new GetSmbProfileQuery(request.ProfileId, userId));

                if (result == null)
                {
                    logger.Warning("No information found for ProfileId {ProfileId}.", request.ProfileId);
                    return Ok("There is no information");
                }

                logger.Information("Successfully retrieved user data for ProfileId {ProfileId}.", request.ProfileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while fetching user data for ProfileId {ProfileId}.", request.ProfileId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("Follow")]
        public async Task<ActionResult<UserProfileDTO>> Follow([FromBody] SomeonesProfileDTO request)
        {
            var validator = new SomeonesProfileDTOValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }

            try
            {

                logger.Information("Follow/unfollow {ProfileId}.", request.ProfileId);

                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("Follow called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                var result = await mediator.Send(new CreateFollowingCommand(userId, request.ProfileId));

                if (result == null)
                {
                    logger.Warning("No information found for ProfileId {ProfileId}.", request.ProfileId);
                    return Ok("There is no information");
                }

                logger.Information("Successfully started following {ProfileId}.", request.ProfileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to follow {ProfileId}.", request.ProfileId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("GetSearchedUsers")]
        public async Task<ActionResult<UserProfileDTO>> GetSearchedUsers([FromBody] GetUsersByLettersDTO request)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("UploadProfilePhoto called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                var result = await mediator.Send(new GetUsersByLettersQuery(request.SearchingField, userId));

                if (result == null)
                {
                    logger.Warning("Nothing found while GetSearchedUsers.");
                    return Ok(null);
                }

                logger.Information("Successfully returned GetSearchedUsers.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to GetSearchedUsers.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetUserFriends")]
        public async Task<ActionResult<UserProfileDTO>> GetUserFriends()
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    logger.Information("GetUserFriends called but user ID is missing.");
                    return Unauthorized("User ID is required.");
                }

                var result = await mediator.Send(new GetUserFriendsQuery(userId));

                if (result == null)
                {
                    logger.Warning("Nothing found while GetUserFriends.");
                    return Ok(null);
                }

                logger.Information("Successfully returned GetUserFriends.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while trying to GetUserFriends.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [AllowAnonymous]
        [HttpPost("GetSomeonesProfile2")]
        public async Task<ActionResult<GiveSmbProfileDTO>> GetSomeonesProfile2([FromBody] SomeonesProfileDTO request)
        {
            var validator = new SomeonesProfileDTOValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }

            try
            {

                logger.Information("Fetching user data for ProfileId {ProfileId}.", request.ProfileId);

                var user = await _UserDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.ProfileId);

                GiveSmbProfileDTO userInfo = new GiveSmbProfileDTO
                {
                    Email = user.Email,
                    NickName = user.NickName,
                    Name = user.Name,
                    Phone = user.Phone,
                    Bio = user.Bio,
                    Photo = user.Photo,
                    DateOfBirth = user.DateOfBirth,
                    IsPrivate = user.IsPrivate,
                    FollowersCount = 0,
                    FollowsCount = 0
                };


                logger.Information("Successfully retrieved user data for ProfileId {ProfileId}.", request.ProfileId);
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while fetching user data for ProfileId {ProfileId}.", request.ProfileId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}