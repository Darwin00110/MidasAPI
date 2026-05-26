using FinTrackAI.src.Domain.Interfaces.IServices;
using Moq;

namespace FinTrackAI.Tests;

public class AdminUseCase_NoServiceTests
{
    private readonly Mock<IUserRepository> _repoMock;
    private readonly Mock<IHashService> _hashMock;
    private readonly Mock<ITokenJWT> _tokenMock;
    private readonly AdminUseCase_NoService _usecase;

    public AdminUseCase_NoServiceTests()
    {
        _repoMock = new Mock<IUserRepository>();
        _hashMock = new Mock<IHashService>();
        _tokenMock = new Mock<ITokenJWT>();
        _usecase = new AdminUseCase_NoService(_repoMock.Object, _hashMock.Object, _tokenMock.Object);
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
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadAdmin(Guid.NewGuid()));
        Assert.Equal("Usuario não existe. ", ex.Message);
    }
    [Fact]
    public async Task ReadAdmin_Caso_UsuarioExista_Com_StatusBloqueado_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.DESATIVADO
        });
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadAdmin(Guid.NewGuid()));
        Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
    }

    [Fact]
    public async Task ReadAdmin_Caso_UsuarioExista_Com_TUDO_OK()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
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
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(false);
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
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(true);
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
    public async Task UpdateAdmin_Caso_TelefoneJaCadastrado_LancarException()
    {
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(x => x.VerifyExistsUser_withTelephone(It.IsAny<string>())).ReturnsAsync(true);

        var request = new UpdateAdminRequest
        {
            Email = "exemplo@gmail.com",
            Telefone = "319732348",
            Senha = "amora0909!"
        };
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UpdateAdmin(Guid.NewGuid(), request));
        Assert.Equal("Telefone ja cadastrado", ex.Message);
    }

    [Fact]
    public async Task UpdateAdmin_Caso_Usuario_Esteja_Bloqueado_LancarException()
    {
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
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
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
        {
            Status = OptionsStatus.ATIVO
        });
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(false);
        _repoMock.Setup(x => x.GetDataUser_ID(It.IsAny<Guid>())).ReturnsAsync(new User
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
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.DeleteAdmin(Guid.NewGuid()));
        Assert.Equal("Usuario não existe. ", ex.Message);
    }
    [Fact]
    public async Task DeleteAdmin_Caso_TudoOK()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(true);
        _repoMock.Setup(x => x.DeleteUser(It.IsAny<Guid>())).ReturnsAsync(true);
        var result = await _usecase.DeleteAdmin(Guid.NewGuid());
        Assert.True(result);
    }
    [Fact]
    public async Task PathUpdateADM_Caso_UsuarioNaoExista_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(false);
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PathUpdateADM(Guid.NewGuid(), new UpdateAdminRequest {}));
        Assert.Equal("O usuario não existe", ex.Message);
    }
    [Fact]
    public async Task PathUpdateADM_Caso_EmailJaCadastrado_LancarException()
    {
        _repoMock.Setup(x => x.VerifyExistsUser_withID(It.IsAny<Guid>())).ReturnsAsync(false);
        
        var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PathUpdateADM(Guid.NewGuid(), new UpdateAdminRequest {}));
        Assert.Equal("O usuario não existe", ex.Message);
    }

}
