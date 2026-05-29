using System.Security.Claims;
using FinTrackAI.src.Domain.Interfaces.UserUseCase_NoService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrackAI;

[ApiController]
[Route("conta")]
public class AccountsController : ControllerBase
{
    private readonly IAccountsUseCase_NoService _accountsUseCase;
    private readonly IUserUseCase_NoService _userUseCase;
    public AccountsController(IAccountsUseCase_NoService accountsUseCase, IUserUseCase_NoService userUseCase)
    {
        _accountsUseCase = accountsUseCase;
        _userUseCase = userUseCase;
    }
    [Authorize(Policy = "AtivoApenas")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        try
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserID == null)
                return BadRequest(new
                {
                    error = "Usuario não fez login."
                });
            var result = await _accountsUseCase.CreateAccount(Guid.Parse(UserID), request);
            return Ok();
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
    [HttpGet("saldo/get")]
    public async Task<IActionResult> GetSaldo()
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userID == null)
                return BadRequest(new
                {
                    error = "Usuario não fez login."
                });
            var result = await _accountsUseCase.GetSaldo(Guid.Parse(userID));
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
    [HttpGet("me")]
    public async Task<IActionResult> GetConta()
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userID == null)
                return BadRequest(new
                {
                    error = "Usuario não fez login."
                });
            var result = await _accountsUseCase.GetDataConta(Guid.Parse(userID));
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
}
