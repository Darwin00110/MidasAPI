using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrackAI;

[ApiController]
[Route("transação")]
public class TransacaoController : ControllerBase
{
    private readonly ITransacaoUseCase _usecase;
    public TransacaoController(ITransacaoUseCase usecase)
    {
        _usecase = usecase;
    }
    [Authorize(Policy = "AtivoApenas")]
    [HttpPost("create")]
    public async Task<IActionResult> CriarTransacao([FromBody] CriarTransacaoRequest request)
    {
        try
        {
            var IDUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _usecase.CriarTransacao(request, Guid.Parse(IDUser));
            return Ok(result);
        }
        catch (UseCaseException e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize(Policy = "AtivoApenas")]
    [HttpGet("me")]
    public async Task<IActionResult> GetDadosTransacao()
    {
        try
        {
            return Ok();
        } catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}
