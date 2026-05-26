using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrackAI;

[ApiController]
[Route("Adm")]
public class AdminController : ControllerBase
{
    private readonly IAdminUseCase_NoService _adminUseCase;
    public AdminController(IAdminUseCase_NoService adminUseCase)
    {
        _adminUseCase = adminUseCase;
    }
    [Authorize(Roles = "ADMIN")]
    [HttpPost("register")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
    {
        try
        {
            var result = await _adminUseCase.CreateAdmin(request);
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
    [Authorize(Roles = "ADMIN")]
    [HttpGet("me")]
    public async Task<IActionResult> ReadAdmin(Guid id)
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _adminUseCase.ReadAdmin(Guid.Parse(userID));
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
    [Authorize(Roles = "ADMIN")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var result = await _adminUseCase.LoginAdm(request);
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
    [Authorize(Roles = "ADMIN")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminRequest request)
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _adminUseCase.UpdateAdmin(Guid.Parse(userID), request);
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
    [Authorize(Roles = "ADMIN")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteAdmin()
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _adminUseCase.DeleteAdmin(Guid.Parse(userID));
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
    [Authorize(Roles = "ADMIN")]
    [HttpPatch("me")]
    public async Task<IActionResult> PathUpdateADM(UpdateAdminRequest request)
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _adminUseCase.PathUpdateADM(Guid.Parse(userID), request);
            if (!result)
            {
                return BadRequest();
            }
            return Ok(200);
        } catch(Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }

    [Authorize(Roles = "ADMIN")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpGet("Users")]
    public async Task<IActionResult> ListAllUsers()
    {
        try
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _adminUseCase.ReadAllUsers(Guid.Parse(userID));
            return Ok(new
            {
                data = result
            });
        } catch (Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [Authorize(Roles = "ADMIN")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpPatch("Users/{id}/block")]
    public async Task<IActionResult> BloquearAcessoUsuario(Guid id)
    {
        try
        {
            var result = await _adminUseCase.BlockAcessUser(id);
            if (!result)
            {
                return BadRequest(new
                {
                    error = "Usuario não foi bloqueado, tente novamente mais tarde"
                });
            } 
            return Ok(new
            {
                sucess = "Usuario bloqueado."
            });
        }catch(Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [Authorize(Roles = "ADMIN")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpPatch("Users/{id}/unlocked")]
    public async Task<IActionResult> LiberarAcessoUsuario(Guid id)
    {
        try
        {
            var result = await _adminUseCase.UnlockedAcessUser(id);
            if (!result)
            {
                return BadRequest(new
                {
                    error = "Usuario não foi desbloqueado, tente novamente mais tarde"
                });
            }
            return Ok(new
            {
                sucess = "Usuario desbloqueado."
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
    [Authorize(Roles = "ADMIN")]
    [Authorize(Policy = "AtivoApenas")]
    [HttpGet("Users/{id}/Data")]
    public async Task<IActionResult> GetDataUser(Guid id)
    {
        try
        {
            var result = await _adminUseCase.GetDataUser_Adm(id);
            return Ok(new
            {
                data = result
            });
        } catch(Exception e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
}
