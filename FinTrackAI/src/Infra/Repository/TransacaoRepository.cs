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

    public async Task<Transacao> ExecutarTransferencia(
        Transacao transferencia,
        Accounts contaOrigem,
        Accounts contaDestino,
        decimal totalDebito)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Debitar origem
            contaOrigem.Saldo -= totalDebito;
            _context.Accounts.Update(contaOrigem);

            // 2. Creditar destino
            contaDestino.Saldo += transferencia.ValorLiquido;
            _context.Accounts.Update(contaDestino);

            // 3. Salvar transação
            await _context.Transacao.AddAsync(transferencia);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return transferencia;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Transacao?> BuscarPorId(Guid id)
    {
        return await _context.Transacao
            .Include(t => t.ContaOrigem)
            .Include(t => t.ContaDestino)
            .FirstOrDefaultAsync(t => t.ID == id);
    }

    public async Task<Transacao?> BuscarPorProtocolo(string protocolo)
    {
        return await _context.Transacao
            .FirstOrDefaultAsync(t => t.Protocolo == protocolo);
    }

    public async Task<List<Transacao>> ListarPorConta(Guid contaId)
    {
        return await _context.Transacao
            .Where(t => t.ContaOrigemId == contaId || t.ContaDestinoId == contaId)
            .OrderByDescending(t => t.CriadoEm)
            .ToListAsync();
    }
}
