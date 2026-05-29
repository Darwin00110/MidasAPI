using FinTrackAI.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FinTrackAI.Tests;

public class TransacaoUseCase_NoServiceTests
{
    private readonly Mock<IAccountsRepository> _accountsRepo;
    private readonly Mock<IUserRepository> _repoUser;
    private readonly Mock<ITransacaoRepository> _repoTransfer;
    private readonly TransacaoUseCase _usecase;
    public TransacaoUseCase_NoServiceTests()
    {
        _accountsRepo = new Mock<IAccountsRepository>();
        _repoUser = new Mock<IUserRepository>();
        _repoTransfer = new Mock<ITransacaoRepository>();
        _usecase = new TransacaoUseCase(_repoUser.Object, _repoTransfer.Object, _accountsRepo.Object);
    }

    [Fact]
    public async Task Transferir_CasoUsuario_Origem_NaoExista_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Usuario_Origem não existe.", ex.Message);
    }
    [Fact]
    public async Task Transferir_CasoConta_Origem_NaoExista_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Conta_Origem não existe.", ex.Message);
    }

    [Fact]
    public async Task Transferir_CasoConta_Origem_Esteja_DESATIVADO_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "",
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Conta desativada, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Transferir_Caso_Usuario_Origem_Esteja_DESATIVADO_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.DESATIVADO
            },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Transferir_Caso_SaldoInsuficiente_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "",
            Saldo = 10
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Saldo insuficiente.", ex.Message);
    }

    [Fact]
    public async Task Transferir_Caso_TransferenciaNaPropriaConta_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "amora",
            Saldo = 10000
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Não é possivel transferir para a propria conta.", ex.Message);
    }

    [Fact]
    public async Task Transferir_Caso_Conta_Destino_Desativada_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora01",
            Status = OptionsStatus.ATIVO,
            Saldo = 10000
        });
        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora",
            Status = OptionsStatus.DESATIVADO,
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Conta_Destino desativada, entre em contato com o suporte.", ex.Message);
    }
    [Fact]
    public async Task Transferir_Caso_Usuario_Destino_Desativado_LancarException()
    {
        _repoUser.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora01",
            Status = OptionsStatus.ATIVO,
            Saldo = 10000
        });
        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.DESATIVADO
            },
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO,
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000
        }, Guid.NewGuid()));
        Assert.Equal("Usuario_Destino bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Transferir_Caso_Usuario_Destino_NaoExista_LancarException()
    {
        var User_OrigemID = Guid.NewGuid();
        var User_DestinoID = Guid.NewGuid();
        _repoUser.Setup(x => x.VerifyExistsUser(User_OrigemID)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora01",
            Status = OptionsStatus.ATIVO,
            Saldo = 10000
        });
        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO,
        });
        _repoUser.Setup(x => x.VerifyExistsUser(User_DestinoID)).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000
        }, User_OrigemID));
        Assert.Equal("Usuario_Destino não existe.", ex.Message);
    }

    [Fact]
    public async Task Transferir_Caso_Conta_Destino_NaoExista_LancarException()
    {
        var User_OrigemID = Guid.NewGuid();
        var User_DestinoID = Guid.NewGuid();
        _repoUser.Setup(x => x.VerifyExistsUser(User_OrigemID)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(User_OrigemID)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora01",
            Status = OptionsStatus.ATIVO,
            Saldo = 10000
        });
        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO,
            UserID = User_DestinoID
        });
        _repoUser.Setup(x => x.VerifyExistsUser(User_DestinoID)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(User_DestinoID)).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Transferir(new TransferirRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000
        }, User_OrigemID));
        Assert.Equal("Conta_Destino não existe.", ex.Message);
    }

    [Fact]
    public async Task Transferir_Caso_TudoOk()
    {
        var User_OrigemID = Guid.NewGuid();
        var User_DestinoID = Guid.NewGuid();

        _repoUser.Setup(x => x.VerifyExistsUser(User_OrigemID)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(User_OrigemID)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.GetDataAccounts(User_OrigemID)).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Nome = "Rodrigo",
                ID = User_OrigemID,
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora01",
            Status = OptionsStatus.ATIVO,
            Saldo = 10000,
            UserID = User_OrigemID
        });
        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Nome = "Ronaldo",
                ID = User_DestinoID,
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO,
            UserID = User_DestinoID
        });
        _repoUser.Setup(x => x.VerifyExistsUser(User_DestinoID)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(User_DestinoID)).ReturnsAsync(true);
        _repoTransfer.Setup(x => x.Transferir(It.IsAny<Transacao>(), It.IsAny<Guid>())).ReturnsAsync(new TransferirResponse
        {
            Nome_Origem = "Rodrigo",
            Nome_Destino = "Ronaldo",
            ChavePix_Alvo = "amora",
            Valor = 150,
        });

        var result = await _usecase.Transferir(new TransferirRequest
        {
            Valor = 1000,
            Chave_Alvo = "amora"
        }, User_OrigemID); // <- aqui estava o problema

        Assert.NotNull(result);
        Assert.Equal(150, result.Valor);
    }

    [Fact]
    public async Task Depositar_Caso_Conta_Destino_Desativada_LancarException()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                ID = userID_origem,
                Status = OptionsStatus.ATIVO,
            },
            UserID = userID_origem,
            Status = OptionsStatus.DESATIVADO,
            ChavePix = "amora",
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Depositar(new DepositarRequest
        {
            Chave_Alvo = "amora",
            Valor = 100,
        }, userID_origem));
        Assert.Equal("Conta desativada, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Depositar_Caso_Usuario_Destino_NaoExista_LancarException()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                ID = userID_origem,
                Status = OptionsStatus.ATIVO,
            },
            UserID = userID_origem,
            Status = OptionsStatus.ATIVO,
            ChavePix = "amora",
        });
        _repoUser.Setup(x => x.VerifyExistsUser(userID_origem)).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Depositar(new DepositarRequest
        {
            Chave_Alvo = "amora",
            Valor = 100,
        }, userID_origem));
        Assert.Equal("Usuario não existe.", ex.Message);
    }

    [Fact]
    public async Task Depositar_Caso_Conta_Destino_NaoExista_LancarException()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                ID = userID_origem,
                Status = OptionsStatus.ATIVO,
            },
            UserID = userID_origem,
            Status = OptionsStatus.ATIVO,
            ChavePix = "amora",
        });
        _repoUser.Setup(x => x.VerifyExistsUser(userID_origem)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(userID_origem)).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Depositar(new DepositarRequest
        {
            Chave_Alvo = "amora",
            Valor = 100,
        }, userID_origem));
        Assert.Equal("Conta não existe.", ex.Message);
    }

    [Fact]
    public async Task Depositar_Caso_Conta_TudoOK()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                ID = userID_origem,
                Status = OptionsStatus.ATIVO,
            },
            UserID = userID_origem,
            Status = OptionsStatus.ATIVO,
            ChavePix = "amora",
        });
        _repoUser.Setup(x => x.VerifyExistsUser(userID_origem)).ReturnsAsync(true);
        _accountsRepo.Setup(x => x.VerifyAccountExists(userID_origem)).ReturnsAsync(true);
        _repoTransfer.Setup(x => x.Depositar(It.IsAny<Transacao>(), It.IsAny<Guid>())).ReturnsAsync(new DepositarResponse
        {
            Saldo_Atual = 1000
        });
        var result = await _usecase.Depositar(new DepositarRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000,
        }, userID_origem);
        Assert.NotNull(result);
        Assert.Equal(1000, result.Saldo_Atual);
    }

    [Fact]
    public async Task Sacar_Caso_Conta_Origem_Desativada_LancarException()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "amora",
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Sacar(new SacarRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000,
            CPF = ""
        }, userID_origem));
        Assert.Equal("Conta desativada, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Sacar_Caso_Usuario_Origem_Desativada_LancarException()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.DESATIVADO
            },
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Sacar(new SacarRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000,
            CPF = ""
        }, userID_origem));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Sacar_Caso_CPF_INCORRETO_LancarException()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO,
                CPF = "232325"
            },
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO,
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Sacar(new SacarRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000,
            CPF = "232323"
        }, userID_origem));
        Assert.Equal("CPF incorreto, tente novamente.", ex.Message);
    }

    [Fact]
    public async Task Sacar_Caso_SALDO_Insuficiente_LancarException()
    {
        var userID_origem = Guid.NewGuid();

        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO,
                CPF = "232323"
            },
            Saldo = 100,
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO,
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Sacar(new SacarRequest
        {
            Chave_Alvo = "amora",
            Valor = 1000,
            CPF = "232323"
        }, userID_origem));
        Assert.Equal("Saldo insuficiente.", ex.Message);
    }

    [Fact]
    public async Task Sacar_Caso_TudoOK()
    {
        var userID_origem = Guid.NewGuid();
        _accountsRepo.Setup(x => x.GetDataAccounts_WithKey(It.IsAny<string>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Nome = "Rodolfo",
                Status = OptionsStatus.ATIVO,
                CPF = "232323"
            },
            Saldo = 10000,
            ChavePix = "amora",
            Status = OptionsStatus.ATIVO,
        });
        _repoTransfer.Setup(x => x.Sacar(It.IsAny<Transacao>())).ReturnsAsync(1000);
        var result = await _usecase.Sacar(new SacarRequest
        {
            CPF = "232323",
            Chave_Alvo = "amora",
            Valor = 1000,
        }, userID_origem);
        Assert.Equal(1000, result);
    }
    [Fact]
    public async Task Extrato_Caso_Conta_Origem_Desativada_LancarException()
    {
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User {},
            ChavePix = "",
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Extrato(Guid.NewGuid()));
        Assert.Equal("Conta desativada, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Extrato_Caso_Usuario_Origem_Desativada_LancarException()
    {
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.DESATIVADO
            },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.Extrato(Guid.NewGuid()));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task Extrato_Caso_TudoOK()
    {
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        _repoTransfer.Setup(x => x.Extrato(It.IsAny<Guid>())).ReturnsAsync(new ExtratoResponse
        {
            
        });
        var result = await _usecase.Extrato(Guid.NewGuid());
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ExtratoID_Caso_Conta_Origem_Desativada_LancarException()
    {
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User {},
            ChavePix = "",
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ExtratoPorID(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal("Conta desativada, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task ExtratoID_Caso_Usuario_Origem_Desativada_LancarException()
    {
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.DESATIVADO
            },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ExtratoPorID(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task ExtratoID_Caso_TudoOK()
    {
        _accountsRepo.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User
            {
                Status = OptionsStatus.ATIVO
            },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        _repoTransfer.Setup(x => x.ExtratoPorID(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new TransacaoExtratoDTO
        {
            
        });
        var result = await _usecase.ExtratoPorID(Guid.NewGuid(), Guid.NewGuid());
        Assert.NotNull(result);
    }
}
