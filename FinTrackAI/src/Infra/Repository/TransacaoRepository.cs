using FinTrackAI.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTrackAI;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly ApplicationDbContext _context;
    public TransacaoRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Transacao> GetUserByKey(string key)
    {
        try
        {
            return new Transacao { };
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<Transacao> CriarTransacao(Guid ID, CriarTransacaoRequest request)
    {
        try
        {
            return new Transacao { };
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
}
