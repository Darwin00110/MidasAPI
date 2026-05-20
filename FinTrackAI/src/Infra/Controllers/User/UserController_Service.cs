using System.Collections;
using FinTrackAI.src.Domain.Interfaces.IUserUseCase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace FinTrackAI;

[ApiController]
[Route("User/cpf")]
public class UserController_Service : ControllerBase
{
    private readonly IUserUseCase_WithService _userUseCase;
    
    public UserController_Service(IUserUseCase_WithService userUseCase)
    {
        _userUseCase = userUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequest_withCPF request)
    {
        try
        {
            var result = await _userUseCase.CreateUser_withCPF(request);
            return Ok(new
            {
                Data = result
            });
        } catch(Exception e)
        {
            return BadRequest(new { 
                error = e.Message
            });
        }
    }
    [Authorize(Roles = "USER")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpGet("me")]
    public async Task<IActionResult> ReadUser([FromHeader] Guid id)
    {
        try
        {
            var result = await _userUseCase.ReadUser_withCPF(id);
            return Ok(new { 
                Data = result
            });
        } catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
}
