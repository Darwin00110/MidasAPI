using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MidasAPI.src.Domain.Interfaces.UserUseCase_NoService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MidasAPI;

[ApiController]
[Route("User")]
public class UserController : ControllerBase
{
    private readonly IUserUseCase_NoService _userUseCase;
    public UserController(IUserUseCase_NoService userUseCase)
    {
        _userUseCase = userUseCase;
    }
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var result = await _userUseCase.CreateUser(request);
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        try
        {
            var result = await _userUseCase.LoginUser(request);
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [Authorize(Policy = "AtivoApenas")]
    [Authorize(Roles = "USER")]
    [HttpGet("me")]
    public async Task<IActionResult> ReadUser()
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _userUseCase.ReadUser(Guid.Parse(userID));
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [Authorize(Policy = "AtivoApenas")]
    [Authorize(Roles = "USER")]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _userUseCase.UpdateUser(Guid.Parse(userID), request);
            if (!result)
            {
                return BadRequest(new
                {
                    error = "Erro na atualização do usuario. "
                });
            }
            return new StatusCodeResult(204);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [Authorize(Policy = "AtivoApenas")]
    [Authorize(Roles = "USER")]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _userUseCase.DeleteUser(Guid.Parse(userID));
            if (!result)
            {
                return BadRequest(new
                {
                    error = "Erro ao deletar o usuario"
                });
            }
            return new StatusCodeResult(204);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [Authorize(Roles = "USER")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpPatch("me")]
    public async Task<IActionResult> PatchUpdateUser(UpdateUserRequest request)
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _userUseCase.PatchUpdateUser(Guid.Parse(userID), request);
            return Ok(200);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
}


