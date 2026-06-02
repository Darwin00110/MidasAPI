using MidasAPI.src.Domain.Interfaces.IServices;
using Moq;

namespace MidasAPI.Tests;

public class AdminUseCase_NoServiceTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly Mock<IHashService> _hashMock;
    private readonly Mock<ITokenJWT> _tokenMock;
    private readonly Mock<IAccountsRepository> _repobank;
    private readonly AdminUseCase_NoService _usecase;

    public AdminUseCase_NoServiceTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _hashMock = new Mock<IHashService>();
        _tokenMock = new Mock<ITokenJWT>();
        _repobank = new Mock<IAccountsRepository>();
        _usecase = new AdminUseCase_NoService(_repoMock.Object, _hashMock.Object, _tokenMock.Object, _repobank.Object);
    }

    [Fact]
    public async Task CreateAdmin_CaseUsuarioJaExista_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(true);
        var request = new CreateAdminRequest
        {
            Nome = "Rodolfo",
            Email = "Rodolfo@gmail.com",
            Telefone = "319732348",
            DataNascimento = new DateTime(2009, 04, 23).ToString(),
            CPF = "13139585602",
            Senha = "chuvamolhada8787"
        };
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.CreateAdmin(request));
        Assert.Equal("Email ja cadastrado. ", ex.Message);
    }
    [Fact]
    public async Task CreateAdmin_Caso_CPFJaCadastrado_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(x => x.VerifyExistsUser_withCPF(It.IsAny<string>())).ReturnsAsync(true);
        var request = new CreateAdminRequest
        {
            Nome = "Rodolfo",
            Email = "Rodolfo@gmail.com",
            Telefone = "319732348",
            DataNascimento = new DateTime(2009, 04, 23).ToString(),
            CPF = "13139585602",
            Senha = "chuvamolhada8787"
        };
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.CreateAdmin(request));
        Assert.Equal("CPF ja cadastrado. ", ex.Message);
    }
    [Fact]
    public async Task CreateAdmin_CasoTudoOK_SenhaDeveSerHasheada()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(x => x.VerifyExistsUser_withCPF(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _hashMock.Setup(x => x.HashPassword(It.IsAny<string>())).ReturnsAsync("hashed_password");
        var request = new CreateAdminRequest
        {
            Nome = "Rodolfo",
            Email = "Rodolfo@gmail.com",
            Telefone = "319732348",
            DataNascimento = new DateTime(2009, 04, 23).ToString(),
            CPF = "13139585602",
            Senha = "amora0909!",
        };
        var result = await _usecase.CreateAdmin(request);
        Assert.NotNull(result);
        Assert.NotEqual("amora0909!", result.PasswordHash);
        Assert.Equal("hashed_password", result.PasswordHash);
        Assert.Equal("Rodolfo", result.Nome);
    }
    [Fact]
    public async Task ReadAdmin_Caso_UsuarioNaoExista_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadAdmin(Guid.NewGuid()));
        Assert.Equal("Usuario não existe. ", ex.Message);
    }
    [Fact]
    public async Task ReadAdmin_Caso_UsuarioExista_Com_StatusBloqueado_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadAdmin(Guid.NewGuid()));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task ReadAdmin_Caso_UsuarioExista_Com_TUDO_OK()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.ReadUser(It.IsAny<Guid>())).ReturnsAsync(new ReadResponse
        {
            Nome = "Rodolfo",
            Email = "Rodolfo@gmail.com",
        });
        var result = await _usecase.ReadAdmin(Guid.NewGuid());
        Assert.NotNull(result);
        Assert.Equal("Rodolfo", result.Nome);
        Assert.Equal("Rodolfo@gmail.com", result.Email);
    }
    [Fact]
    public async Task UpdateAdmin_Caso_UsuarioNaoExista_LancarException()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var request = new UpdateAdminRequest
        {
            Email = "exemplo@gmail.com",
            Telefone = "319732348",
            Senha = "amora0909!"
        };
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UpdateAdmin(Guid.NewGuid(), request));
        Assert.Equal("Usuario não existe", ex.Message);
    }
    [Fact]
    public async Task UpdateAdmin_Caso_EmailJaCadastrado_LancarException()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(true);
        var request = new UpdateAdminRequest
        {
            Email = "exemplo@gmail.com",
            Telefone = "319732348",
            Senha = "amora0909!"
        };
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UpdateAdmin(Guid.NewGuid(), request));
        Assert.Equal("Email ja cadastrado", ex.Message);
    }

    [Fact]
    public async Task UpdateAdmin_Caso_Usuario_Esteja_Bloqueado_LancarException()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var request = new UpdateAdminRequest
        {
            Email = "exemplo@gmail.com",
            Telefone = "319732348",
            Senha = "amora0909!"
        };
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UpdateAdmin(Guid.NewGuid(), request));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }
    [Fact]
    public async Task UpdateAdmin_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _hashMock.Setup(x => x.HashPassword(It.IsAny<string>())).ReturnsAsync("hashed_password");
        _repoMock.Setup(x => x.UpdateUser(It.IsAny<Guid>(), It.IsAny<User>())).ReturnsAsync(true);
        var request = new UpdateAdminRequest
        {
            Email = "exemplo@gmail.com",
            Telefone = "319732348",
            Senha = "amora0909!"
        };
        var result = await _usecase.UpdateAdmin(Guid.NewGuid(), request);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAdmin_Caso_UsuarioNaoExista_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.DeleteAdmin(Guid.NewGuid()));
        Assert.Equal("Usuario não existe. ", ex.Message);
    }
    [Fact]
    public async Task DeleteAdmin_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.DeleteUser(It.IsAny<Guid>())).ReturnsAsync(true);
        var result = await _usecase.DeleteAdmin(Guid.NewGuid());
        Assert.True(result);
    }
    [Fact]
    public async Task PathUpdateADM_Caso_UsuarioNaoExista_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PathUpdateADM(Guid.NewGuid(), new UpdateAdminRequest { }));
        Assert.Equal("O usuario não existe", ex.Message);
    }
    [Fact]
    public async Task PathUpdateADM_Caso_EmailJaCadastrado_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Email = "Rodolfo@gmail.com",
            Telefone = "319732348"
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PathUpdateADM(Guid.NewGuid(), new UpdateAdminRequest
        {
            Email = "Rodolfo@gmail.com",
            Telefone = "319732342"
        }));
        Assert.Equal("Email ja cadastrado anteriormente.", ex.Message);
    }

    [Fact]
    public async Task PathUpdateADM_Caso_TelefoneJaCadastrado_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Email = "Rodolfo@gmail.com",
            Telefone = "319732348"
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PathUpdateADM(Guid.NewGuid(), new UpdateAdminRequest
        {
            Email = "Rodolfo2@gmail.com",
            Telefone = "319732348"
        }));
        Assert.Equal("Telefone ja cadastrado anteriormente.", ex.Message);
    }

    [Fact]
    public async Task PathUpdateADM_Caso_SenhaJaCadastrado_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _hashMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Email = "Rodolfo@gmail.com",
            Telefone = "319732348"
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PathUpdateADM(Guid.NewGuid(), new UpdateAdminRequest
        {
            Email = "Rodolfo2@gmail.com",
            Telefone = "319732342"
        }));
        Assert.Equal("Senha ja cadastrada anteriormente.", ex.Message);
    }

    [Fact]
    public async Task PatchUpdateADM_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Email = "Rodolfo@gmail.com",
            Telefone = "319732348"
        });
        _hashMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        _hashMock.Setup(x => x.HashPassword(It.IsAny<string>())).ReturnsAsync("hashed_password");
        _repoMock.Setup(x => x.PatchUpdateUser(It.IsAny<Guid>(), It.IsAny<User>())).ReturnsAsync(true);
        var result = await _usecase.PathUpdateADM(Guid.NewGuid(), new UpdateAdminRequest
        {
            Email = "Rodolfo02@gmail.com",
            Telefone = "319732342",
            Senha = "amora0909!"
        });
        Assert.True(result);
    }
    [Fact]
    public async Task ReadAllUsers_Caso_UsuarioNaoExista_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadAllUsers(It.IsAny<Guid>()));
        Assert.Equal("Usuario não existe. ", ex.Message);
    }

    [Fact]
    public async Task ReadAllUsers_Caso_UsuarioNaoSejaADM_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Role = OptionsRole.USER
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadAllUsers(It.IsAny<Guid>()));
        Assert.Equal("Operação negada, Usuario não é um administrador", ex.Message);
    }

    [Fact]
    public async Task ReadAllUsers_Caso_UsuarioEsteja_DESATIVADO_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Role = OptionsRole.ADMIN,
            Status = OptionsStatus.DESATIVADO
        });

        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadAllUsers(It.IsAny<Guid>()));
        Assert.Equal("Operação negada, Usuario esta bloqueado", ex.Message);
    }

    [Fact]
    public async Task ReadAllUsers_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Role = OptionsRole.ADMIN,
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.GetAllUsers()).ReturnsAsync(new List<GetAllUsersResponse>
        {
            new GetAllUsersResponse
            {
                Nome = "Rodolfo",
                ID = Guid.NewGuid(),
                CPF = "13139585602",
            }});

        var result = await _usecase.ReadAllUsers(It.IsAny<Guid>());
        Assert.NotNull(result);
        Assert.Equal("Rodolfo", result[0].Nome);
    }
    [Fact]
    public async Task BlockAcessUser_Caso_UsuarioJaEstejaBloqueado_LancarException()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.BlockAcessUser(Guid.NewGuid()));
        Assert.Equal("Usuario já esta bloqueado", ex.Message);
    }
    [Fact]
    public async Task BlockAcessUser_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.BlockAcessUser(It.IsAny<Guid>())).ReturnsAsync(true);

        var result = await _usecase.BlockAcessUser(Guid.NewGuid());
        Assert.True(result);
    }
    [Fact]
    public async Task LoginADM_Caso_UsuarioEstejaDesativado_LancarException()
    {
        _repoMock.Setup(u => u.GetDataUserEmail(It.IsAny<string>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.LoginAdm(new LoginUserRequest { }));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task LoginADM_Caso_SenhaIncorreta_LancarException()
    {
        _repoMock.Setup(u => u.GetDataUserEmail(It.IsAny<string>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _hashMock.Setup(u => u.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.LoginAdm(new LoginUserRequest { }));
        Assert.Equal("Senha incorreta.", ex.Message);
    }
    [Fact]
    public async Task LoginADM_Caso_TudoOK()
    {
        _repoMock.Setup(u => u.GetDataUserEmail(It.IsAny<string>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO,
            Role = OptionsRole.ADMIN,
            ID = Guid.NewGuid(),
            Email = "rodolfo@gmail.com",
            Nome = "Rodolfo",
        });
        _hashMock.Setup(u => u.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _tokenMock.Setup(u => u.GenerativeToken(It.IsAny<User>())).ReturnsAsync("token");

        var result = await _usecase.LoginAdm(new LoginUserRequest
        {
            Email = "rodolfo@gmail.com",
            Senha = "amora0909!",
        });
        Assert.Equal("token", result);
    }
    [Fact]
    public async Task UnlockedAcessUser_CasoUsuarioJaEstejaDesbloqueado_LancarException()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UnlockedAcessUser(Guid.NewGuid()));
        Assert.Equal("Usuario já esta desbloqueado", ex.Message);
    }
    [Fact]
    public async Task UnlockedAcessUser_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        _repoMock.Setup(x => x.UnlockedAcessUser(It.IsAny<Guid>())).ReturnsAsync(true);
        var result = await _usecase.UnlockedAcessUser(Guid.NewGuid());
        Assert.True(result);
    }

    [Fact]
    public async Task GetDataUser_Adm_Caso_UsuarioNaoExista_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.GetDataUser_Adm(Guid.NewGuid()));
        Assert.Equal("Usuario não existe", ex.Message);
    }

    [Fact]
    public async Task GetDataUser_Adm_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.ReadUser(It.IsAny<Guid>())).ReturnsAsync(new ReadResponse
        {
            Nome = "Rodolfo"
        });
        var result = await _usecase.GetDataUser_Adm(It.IsAny<Guid>());
        Assert.NotNull(result);
        Assert.Equal("Rodolfo", result.Nome);
    }

    [Fact]
    public async Task BlockAcessAccount_Caso_ContaNaoExista_LancarException()
    {
        _repobank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.BlockAcessAccount(Guid.NewGuid()));
        Assert.Equal("A Conta não existe.", ex.Message);
    }

    [Fact]
    public async Task BlockAcessAccount_Caso_ContaJaEstejaBloqueada_LancarException()
    {
        _repobank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repobank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "",
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.BlockAcessAccount(Guid.NewGuid()));
        Assert.Equal("A Conta já esta bloqueada.", ex.Message);
    }

    [Fact]
    public async Task BlockAcessAccount_Caso_TudoOK()
    {
        _repobank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repobank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.BlockAcessAccount(It.IsAny<Guid>())).ReturnsAsync(true);
        var result = await _usecase.BlockAcessAccount(Guid.NewGuid());
        Assert.True(result);
    }
    [Fact]
    public async Task UnlockedAcessAccount_Caso_ContaNaoExista_LancarException()
    {
        _repobank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UnlockedAcessAccount(Guid.NewGuid()));
        Assert.Equal("A Conta não existe.", ex.Message);
    }

    [Fact]
    public async Task UnlockedAcessAccount_Caso_ContaJaDesbloqueada_LancarException()
    {
        _repobank.Setup(x => x.VerifyAccountExists(It.IsAny<Guid>())).ReturnsAsync(true);
        _repobank.Setup(x => x.GetDataAccounts(It.IsAny<Guid>())).ReturnsAsync(new Accounts
        {
            User = new User { },
            ChavePix = "",
            Status = OptionsStatus.ATIVO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UnlockedAcessAccount(Guid.NewGuid()));
        Assert.Equal("A Conta já esta Desbloqueada.", ex.Message);
    }
}
