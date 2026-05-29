using System.Security.Claims;
using FinTrackAI.src.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrackAI;

[ApiController]
[Route("Transação")]
public class TransacaoController : ControllerBase
{
    private readonly ITransacaoUseCase _usecase;
    public TransacaoController(ITransacaoUseCase usecase)
    {
        _usecase = usecase;
    }
    [Authorize(Policy = "AtivoApenas")]
    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir([FromBody] TransferirRequest request)
    {
        try
        {
            var IDUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (IDUser == null)
                throw new UseCaseException("Falha no login.");
            var result = await _usecase.Transferir(request, Guid.Parse(IDUser));
            return Ok(new
            {
                Data = result
            });
        }
        catch (UseCaseException e)
        {
            return BadRequest(new
            {
                error = e.Message
            });
        }
    }
    [Authorize(Policy = "AtivoApenas")]
    [HttpPost("Depositar")]
    public async Task<IActionResult> Depositar([FromBody] DepositarRequest request)
    {
        try
        {
            var IDUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (IDUser == null)
                throw new UseCaseException("Falha no login.");
            var result = await _usecase.Depositar(request, Guid.Parse(IDUser));
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [Authorize(Policy = "AtivoApenas")]
    [HttpPost("sacar")]
    public async Task<IActionResult> Sacar([FromBody] SacarRequest request)
    {
        try
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserID == null)
                return BadRequest("Falha no login.");
            var result = await _usecase.Sacar(request, Guid.Parse(UserID));
            return Ok(new
            {
                Data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [Authorize(Policy = "AtivoApenas")]
    [HttpGet("extrato")]
    public async Task<IActionResult> Extrato([FromHeader] Guid id)
    {
        try
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserID == null)
                return Unauthorized();
            var result = await _usecase.Extrato(Guid.Parse(UserID));
            return Ok(new
            {
                Data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [Authorize(Policy = "AtivoApenas")]
    [HttpGet("extrato/{id}")]
    public async Task<IActionResult> ExtratoPorID(Guid id)
    {
        try
        {
            var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(UserID == null)
                return Unauthorized();
            var result = await _usecase.ExtratoPorID(id, Guid.Parse(UserID));
            return Ok(new
            {
                data = result
            });
        }
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

}
