using MidasAPI.src.Domain.Entities;

namespace MidasAPI;

public class TransacaoUseCase : ITransacaoUseCase
{
    private readonly ITransacaoRepository _transferRepo;
    private readonly IUserRepository _repoUser;
    private readonly IAccountsRepository _accountsRepo;
    public TransacaoUseCase(IUserRepository repo, ITransacaoRepository transferRepo, IAccountsRepository accountsRepo)
    {
        _repoUser = repo;
        _transferRepo = transferRepo;
        _accountsRepo = accountsRepo;
    }
    public async Task<TransferirResponse> Transferir(TransferirRequest request, Guid UserID)
    {
        var verifyUserExists_Origem = await _repoUser.VerifyExistsUser(UserID);
        if (!verifyUserExists_Origem)
            throw new UseCaseException("Usuario_Origem não existe.");
        var verifyAccountsExists_Origem = await _accountsRepo.VerifyAccountExists(UserID);
        if (!verifyAccountsExists_Origem)
            throw new UseCaseException("Conta_Origem não existe.");
        var GetDataAccount_Origem = await _accountsRepo.GetDataAccounts(UserID);
        if (GetDataAccount_Origem.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta desativada, entre em contato com o suporte.");
        if (GetDataAccount_Origem.User.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        if (request.Valor > GetDataAccount_Origem.Saldo)
            throw new UseCaseException("Saldo insuficiente.");
        if (request.Chave_Alvo == GetDataAccount_Origem.ChavePix)
            throw new UseCaseException("Não é possivel transferir para a propria conta.");

        var GetDataAccount_Destino = await _accountsRepo.GetDataAccounts_WithKey(request.Chave_Alvo);

        if (GetDataAccount_Destino.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta_Destino desativada, entre em contato com o suporte.");
        if (GetDataAccount_Destino.User.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario_Destino bloqueado, entre em contato com o suporte.");
        var verifyUserExists_Destino = await _repoUser.VerifyExistsUser(GetDataAccount_Destino.UserID);
        if (!verifyUserExists_Destino)
            throw new UseCaseException("Usuario_Destino não existe.");
        var verifyAccountsExists_Destino = await _accountsRepo.VerifyAccountExists(GetDataAccount_Destino.UserID);
        if (!verifyAccountsExists_Destino)
            throw new UseCaseException("Conta_Destino não existe.");

        Transacao transacaoData = new Transacao
        {
            Nome_Origem = GetDataAccount_Origem.User.Nome,
            Nome_Destino = GetDataAccount_Destino.User.Nome,
            ContaOrigemId = GetDataAccount_Origem.ID,
            ContaDestinoId = GetDataAccount_Destino.ID,
            Tipo = OptionsTipoDaTransferencia.PIX,
            ChavePix_ALVO = request.Chave_Alvo,
            Valor = request.Valor,
            Descricao = request.Descricao ?? "",
            Status = OptionsStatusDaTransferencia.PENDENTE,
        };
        var result = await _transferRepo.Transferir(transacaoData, UserID);
        return result;
    }

    public async Task<DepositarResponse> Depositar(DepositarRequest request, Guid UserID)
    {
        var GetDataAccount_Destino = await _accountsRepo.GetDataAccounts_WithKey(request.Chave_Alvo ?? throw new UseCaseException("Chave_PIX não informada."));
        if (GetDataAccount_Destino.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta desativada, entre em contato com o suporte.");
        var verifyUserExists_Destino = await _repoUser.VerifyExistsUser(GetDataAccount_Destino.UserID);
        if (!verifyUserExists_Destino)
            throw new UseCaseException("Usuario não existe.");
        var verifyAccountsExists_Destino = await _accountsRepo.VerifyAccountExists(GetDataAccount_Destino.UserID);
        if (!verifyAccountsExists_Destino)
            throw new UseCaseException("Conta não existe.");
        Transacao transacaoData = new Transacao
        {
            Nome_Destino = GetDataAccount_Destino.User.Nome,
            ContaDestinoId = GetDataAccount_Destino.ID,
            Tipo = OptionsTipoDaTransferencia.PIX,
            ChavePix_ALVO = request.Chave_Alvo,
            Valor = request.Valor,
            Descricao = request.Descricao ?? "",
            Status = OptionsStatusDaTransferencia.PENDENTE,
        };
        return await _transferRepo.Depositar(transacaoData, UserID);
    }

    public async Task<decimal> Sacar(SacarRequest request, Guid UserID)
    {
        var GetDataAccount_Origem = await _accountsRepo.GetDataAccounts_WithKey(request.Chave_Alvo ?? throw new UseCaseException("Chave_PIX não informada."));
        if (GetDataAccount_Origem.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta desativada, entre em contato com o suporte.");
        if (GetDataAccount_Origem.User.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        if (request.CPF != GetDataAccount_Origem.User.CPF)
            throw new UseCaseException("CPF incorreto, tente novamente.");
        if (request.Valor > GetDataAccount_Origem.Saldo)
            throw new UseCaseException("Saldo insuficiente.");

        Transacao Datatransacao = new Transacao
        {
            ChavePix_ALVO = request.Chave_Alvo,
            Valor = request.Valor,
            CPF = request.CPF,
            Nome_Destino = GetDataAccount_Origem.User.Nome,
            Descricao = request.Descricao ?? "",
            Status = OptionsStatusDaTransferencia.PENDENTE,
            Tipo = OptionsTipoDaTransferencia.Saque,
            CriadoEm = DateTime.Now,
        };
        return await _transferRepo.Sacar(Datatransacao);
    }

    public async Task<ExtratoResponse> Extrato(Guid UserID)
    {
        var GetDataAccount_Origem = await _accountsRepo.GetDataAccounts(UserID);
        if (GetDataAccount_Origem.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta desativada, entre em contato com o suporte.");
        if (GetDataAccount_Origem.User.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");

        return await _transferRepo.Extrato(UserID);
    }

    public async Task<TransacaoExtratoDTO> ExtratoPorID(Guid transacaoID, Guid UserID)
    {
        var conta = await _accountsRepo.GetDataAccounts(UserID);

        if (conta.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Conta desativada, entre em contato com o suporte.");
        if (conta.User.Status == OptionsStatus.DESATIVADO)
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");

        return await _transferRepo.ExtratoPorID(transacaoID, UserID);
    }
}
