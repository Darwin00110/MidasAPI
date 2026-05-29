using FinTrackAI.src.Domain.Entities;
using Moq;

namespace FinTrackAI.Tests;

public class AccountsUseCase_NoServiceTests
{
    private readonly Mock<IAccountsRepository> _repoBank;
    private readonly Mock<IUserRepository> _repoUser;
    private readonly AccountsUseCase_NoService _usecase;
    public AccountsUseCase_NoServiceTests()
    {
        _repoBank = new Mock<IAccountsRepository>();
        _repoUser = new Mock<IUserRepository>();
        _usecase = new AccountsUseCase_NoService(_repoBank.Object, _repoUser.Object);
    }

    [Fact]
    public async Task CreateAccount_Caso_UsuarioNaoExista_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.CreateAccount(Guid.NewGuid(), new CreateAccountRequest
        {
            Email = ""
        }));
        Assert.Equal("Usuario não existe.", ex.Message);
    }

    [Fact]
    public async Task CreateAccount_Caso_ContaJaExiste_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.CreateAccount(Guid.NewGuid(), new CreateAccountRequest
        {
            Email = ""
        }));
        Assert.Equal("Conta ja cadastrada .", ex.Message);
    }

    [Fact]
    public async Task CreateAccount_Caso_UsuarioDesativado_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(false);
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.CreateAccount(Guid.NewGuid(), new CreateAccountRequest
        {
            Email = ""
        }));
        Assert.Equal("Usuario desativado., entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task CreateAccount_Caso_TudoOK()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(false);
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO,
            CPF = "13139585602"
        });

        _repoBank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            ChavePix = "",
            Status = OptionsStatus.ATIVO,
            User = new User { },
            TransacoesEnviadas = new List<Transacao>(),
            TransacoesRecebidas = new List<Transacao>()
        });
        _repoBank.Setup(x => x.CreateAccount(It.IsAny<Accounts>())).ReturnsAsync(true);

        var result = await _usecase.CreateAccount(Guid.NewGuid(), new CreateAccountRequest
        {
            NumeroAgencia = "0001",
            Email = "",
            TipoDaConta = OptionsTipoDaConta.CORRENTE
        });
        Assert.True(result);
    }
    [Fact]
    public async Task GetSaldo_Caso_UsuarioNaoExista_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetSaldo(Guid.NewGuid()));
        Assert.Equal("Usuario não existe.", ex.Message);
    }

    [Fact]
    public async Task GetSaldo_Caso_ContaNaoExiste_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetSaldo(Guid.NewGuid()));
        Assert.Equal("Conta não existe.", ex.Message);
    }

    [Fact]
    public async Task GetSaldo_Caso_UsuarioBloqueado_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetSaldo(Guid.NewGuid()));
        Assert.Equal("Usuario desativado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task GetSaldo_Caso_Conta_Esteja_Bloqueada_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoBank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            ChavePix = "",
            User = new User { },
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetSaldo(Guid.NewGuid()));
        Assert.Equal("Conta desativada, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task GetSaldo_Caso_TudoOk()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.GetSaldo(It.IsAny<Guid>())).ReturnsAsync("1000");
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoBank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        var result = await _usecase.GetSaldo(Guid.NewGuid());
        Assert.Equal("1000", result);
    }
    [Fact]
    public async Task GetDataConta_Caso_UsuarioNaoExista_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetDataConta(Guid.NewGuid()));
        Assert.Equal("Usuario não existe.", ex.Message);
    }

    [Fact]
    public async Task GetDataConta_Caso_ContaNaoExista_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetDataConta(Guid.NewGuid()));
        Assert.Equal("Conta não existe.", ex.Message);
    }

    [Fact]
    public async Task GetDataConta_Caso_UsuarioEstejaDesativado_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetDataConta(Guid.NewGuid()));
        Assert.Equal("Usuario desativado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task GetDataConta_Caso_ContaEstejaDesativada_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoBank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "",
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetDataConta(Guid.NewGuid()));
        Assert.Equal("Conta desativada, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task GetDataConta_Caso_TudoOk()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoBank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoUser.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoBank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "",
            Status = OptionsStatus.ATIVO,
            NumeroAgencia = "",
            Saldo = 1000
        });
        var result = await _usecase.GetDataConta(Guid.NewGuid());
        Assert.NotNull(result);
        Assert.Equal("1000", result.Saldo);
    }
}
