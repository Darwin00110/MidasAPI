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

    public async Task<TransferirResponse> Transferir(Transacao Data, Guid UserID)
    {
        try
        {
            var query_UserOrigem = await _context.Accounts.Where(a => a.UserID == UserID).FirstOrDefaultAsync();
            var query_UserDestino = await _context.Accounts.Where(a => a.ChavePix == Data.ChavePix_ALVO).FirstOrDefaultAsync();
            if (query_UserOrigem == null)
                throw new RepositoryException("[Conta_Origem] Conta não existe.");

            if (query_UserDestino == null)
                throw new RepositoryException("[Conta_Destino] Chave não compativel com nenhuma conta.");

            Data.SaldoOrigemAntes = query_UserOrigem.Saldo;
            Data.SaldoOrigemDepois = query_UserOrigem.Saldo - Data.Valor;
            Data.ContaOrigemId = query_UserOrigem.ID;
            Data.Status = OptionsStatusDaTransferencia.EFETUADA;

            query_UserOrigem.Saldo = Data.SaldoOrigemDepois;
            query_UserOrigem.TransacoesEnviadas.Add(Data);

            query_UserDestino.Saldo = query_UserDestino.Saldo + Data.Valor;
            query_UserDestino.TransacoesRecebidas.Add(Data);

            await _context.SaveChangesAsync();
            return new TransferirResponse
            {
                Nome_Origem = query_UserOrigem.User.Nome,
                Nome_Destino = query_UserDestino.User.Nome,
                Valor = Data.Valor,
                Descricao = Data.Descricao,
                Tipo = OptionsTipoDaTransferencia.PIX.ToString(),
                Status = OptionsStatusDaTransferencia.EFETUADA.ToString(),
                ChavePix_Alvo = Data.ChavePix_ALVO,
                CriadoEm = DateTime.Now,
            };
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<DepositarResponse> Depositar(Transacao Data, Guid UserID)
    {
        try
        {
            var query_ContaDestino = await _context.Accounts
                .Where(a => a.ChavePix == Data.ChavePix_ALVO)
                .FirstOrDefaultAsync();

            if (query_ContaDestino == null)
                throw new RepositoryException("[Conta_Destino] Chave não compatível com nenhuma conta.");

            // Garante que é a própria conta do usuário logado
            if (query_ContaDestino.UserID != UserID)
                throw new RepositoryException("Você só pode depositar na própria conta.");

            Data.ContaOrigemId = query_ContaDestino.ID; // origem = destino no depósito
            Data.ContaDestinoId = query_ContaDestino.ID;

            query_ContaDestino.Saldo += Data.Valor;
            Data.Status = OptionsStatusDaTransferencia.EFETUADA;
            Data.ConcluidoEm = DateTime.Now;
            query_ContaDestino.TransacoesRecebidas.Add(Data);
            await _context.SaveChangesAsync();
            return new DepositarResponse
            {
                ID = query_ContaDestino.ID,
                Saldo_Anterior = query_ContaDestino.Saldo - Data.Valor,
                Saldo_Atual = query_ContaDestino.Saldo,
                EfetuadoEm = DateTime.Now,
                Status = OptionsStatusDaTransferencia.EFETUADA,
                Descricao = Data.Descricao,
            };
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }

    public async Task<decimal> Sacar(Transacao Data)
    {
        try
        {
            var query_UserOrigem = await _context.Accounts
                .Where(a => a.ChavePix == Data.ChavePix_ALVO)
                .Include(a => a.User)
                .FirstOrDefaultAsync();

            if (query_UserOrigem == null)
                throw new RepositoryException("[Conta_Destino] Chave não compativel com nenhuma conta.");
            if (query_UserOrigem.User.CPF != Data.CPF)
                throw new RepositoryException("[CPF] CPF incompativel com a conta.");
            query_UserOrigem.Saldo = query_UserOrigem.Saldo - Data.Valor;
            Data.ContaOrigemId = query_UserOrigem.ID;
            Data.ContaDestinoId = query_UserOrigem.ID;
            Data.ConcluidoEm = DateTime.Now;
            Data.Status = OptionsStatusDaTransferencia.EFETUADA;
            query_UserOrigem.TransacoesEnviadas.Add(Data);
            await _context.SaveChangesAsync();
            return query_UserOrigem.Saldo;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }

    public async Task<ExtratoResponse> Extrato(Guid UserID)
    {
        try
        {
            var conta = await _context.Accounts
                .Where(a => a.UserID == UserID)
                .FirstOrDefaultAsync()
                ?? throw new RepositoryException("Conta não encontrada.");

            var MapDTO = (Transacao t) => new TransacaoExtratoDTO
            {
                ID = t.ID,
                Protocolo = t.Protocolo,
                Valor = t.Valor,
                Tipo = t.Tipo.ToString(),
                Status = t.Status.ToString(),
                ChavePix_ALVO = t.ChavePix_ALVO,
                Descricao = t.Descricao,
                CriadoEm = t.CriadoEm,
                ConcluidoEm = t.ConcluidoEm,
                SaldoOrigemAntes = t.SaldoOrigemAntes,
                SaldoOrigemDepois = t.SaldoOrigemDepois,
            };

            return new ExtratoResponse
            {
                TransacaoEnviada = await _context.Transacao
                    .Where(t => t.ContaOrigemId == conta.ID)
                    .OrderByDescending(t => t.CriadoEm)
                    .Select(t => MapDTO(t))
                    .ToListAsync(),

                TransacaoRecebida = await _context.Transacao
                    .Where(t => t.ContaDestinoId == conta.ID)
                    .OrderByDescending(t => t.CriadoEm)
                    .Select(t => MapDTO(t))
                    .ToListAsync(),
            };
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    // Repository — só adiciona o DTO e corrige o catch
    public async Task<TransacaoExtratoDTO> ExtratoPorID(Guid transacaoID, Guid UserID)
    {
        try
        {
            var conta = await _context.Accounts
                .Where(a => a.UserID == UserID)
                .FirstOrDefaultAsync()
                ?? throw new RepositoryException("Conta não encontrada.");

            var transacao = await _context.Transacao
                .Where(t => t.ID == transacaoID &&
                           (t.ContaOrigemId == conta.ID || t.ContaDestinoId == conta.ID))
                .FirstOrDefaultAsync()
                ?? throw new RepositoryException("Transação não encontrada ou não pertence a esta conta.");

            return new TransacaoExtratoDTO
            {
                ID = transacao.ID,
                Protocolo = transacao.Protocolo,
                Valor = transacao.Valor,
                Tipo = transacao.Tipo.ToString(),
                Status = transacao.Status.ToString(),
                ChavePix_ALVO = transacao.ChavePix_ALVO,
                Descricao = transacao.Descricao,
                CriadoEm = transacao.CriadoEm,
                ConcluidoEm = transacao.ConcluidoEm,
                SaldoOrigemAntes = transacao.SaldoOrigemAntes,
                SaldoOrigemDepois = transacao.SaldoOrigemDepois,
            };
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
}
