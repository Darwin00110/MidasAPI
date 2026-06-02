using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidasAPI.src.Application.UseCase;
using MidasAPI.src.Domain.Interfaces.IServices;
using Moq;

namespace MidasAPI.Tests
{
    public class UserUseCase_NoServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<IHashService> _hashMock;
        private readonly Mock<ITokenJWT> _tokenMock;
        private readonly UserUseCase_NoService _usecase;

        private void Simulacao_VerifyExistsUser_withEmail(bool x)
        {
            _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(x);
        }
        private void Simulacao_verifyExistUser(bool x)
        {
            _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(x);
        }
        public UserUseCase_NoServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _hashMock = new Mock<IHashService>();
            _tokenMock = new Mock<ITokenJWT>();
            _usecase = new UserUseCase_NoService(_repoMock.Object, _hashMock.Object, _tokenMock.Object);
        }
        //=============CREATE User=================//
        [Fact]
        public async Task CreateUser_Caso_UsuarioJaExista_LancarException()
        {
            Simulacao_VerifyExistsUser_withEmail(true);
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.CreateUser(new CreateUserRequest { }));
            Assert.Equal("Usuario ja cadastrado", ex.Message);
        }
        [Fact]
        public async Task CreateUser_Caso_TudoOK()
        {
            Simulacao_VerifyExistsUser_withEmail(false);
            _hashMock.Setup(x => x.HashPassword(It.IsAny<string>())).ReturnsAsync("hashed_password");
            _repoMock.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync(new User
            {
                Nome = "Rodolfo",
                PasswordHash = "hashed_password",
                Email = "exemplo@gmail.com",
                CPF = "13139585602",
                Telefone = "319732348",
                Role = OptionsRole.USER,
                Status = OptionsStatus.ATIVO,
                CreatedAt = DateTime.Now,
            });
            var result = await _usecase.CreateUser(new CreateUserRequest
            {
                Senha = "amora0909!",
                Nome = "Rodolfo",
                CPF = "13139585602",
                Email = "exemplo@gmail.com",
                DataNascimento = new DateTime(2009, 04, 23),
                Telefone = "319732348",
            });
            Assert.NotNull(result);
            Assert.Equal("Rodolfo", result.Nome);
        }

        [Fact]
        public async Task ReadUser_Caso_UsuarioNaoExista_LancarException()
        {
            Simulacao_verifyExistUser(false);
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadUser(Guid.NewGuid()));
            Assert.Equal("Usuario não existe. ", ex.Message);
        }

        [Fact]
        public async Task ReadUser_Caso_TudoOK()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(x => x.ReadUser(It.IsAny<Guid>())).ReturnsAsync(new ReadResponse
            {
                Nome = "Rodolfo"
            });
            var result = await _usecase.ReadUser(Guid.NewGuid());
            Assert.NotNull(result);
            Assert.Equal("Rodolfo", result.Nome);
        }
        [Fact]
        public async Task UpdateUser_Caso_UsuarioNaoExista_LancarException()
        {
            Simulacao_verifyExistUser(false);
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UpdateUser(Guid.NewGuid(), new UpdateUserRequest { }));
            Assert.Equal("Usuario não existe .", ex.Message);
        }

        [Fact]
        public async Task UpdateUser_Caso_UsuarioBloqueado_LancarException()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(u => u.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.DESATIVADO
            });
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UpdateUser(Guid.NewGuid(), new UpdateUserRequest { }));
            Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
        }

        [Fact]
        public async Task UpdateUser_Caso_EmailAnteriorCadastradoNovamente_LancarException()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(u => u.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.ATIVO,
                Email = "exemplo@gmail.com"
            });
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.UpdateUser(Guid.NewGuid(), new UpdateUserRequest
            {
                Email = "exemplo@gmail.com"
            }));
            Assert.Equal("Email cadastrado anteriormente, tente novamente.", ex.Message);
        }

        [Fact]
        public async Task UpdateUser_Caso_TudoOK()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(u => u.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Email = "exemplo@gmail.com",
                Status = OptionsStatus.ATIVO
            });
            _hashMock.Setup(u => u.HashPassword(It.IsAny<string>())).ReturnsAsync("hash_password");
            _repoMock.Setup(u => u.UpdateUser(It.IsAny<Guid>(), It.IsAny<User>())).ReturnsAsync(true);

            var result = await _usecase.UpdateUser(Guid.NewGuid(), new UpdateUserRequest
            {
                Email = "exemplo02@gmail.com",
                Telefone = "322345678",
                Senha = "amora0909!"
            });
            Assert.True(result);
        }

        [Fact]
        public async Task LoginUser_Caso_UsuarioNaoExista_LancarException()
        {
            Simulacao_VerifyExistsUser_withEmail(false);
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.LoginUser(new LoginUserRequest
            {

            }));
            Assert.Equal("Usuario não encontrado", ex.Message);
        }

        [Fact]
        public async Task LoginUser_Caso_UsuarioEstejaDESATIVADO_LancarException()
        {
            Simulacao_VerifyExistsUser_withEmail(true);
            _repoMock.Setup(x => x.GetDataUserEmail(It.IsAny<string>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.DESATIVADO
            });
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.LoginUser(new LoginUserRequest
            {
                Email = "exemplo@gmail.com"
            }));
            Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
        }

        [Fact]
        public async Task LoginUser_Caso_SenhaIncorreta_LancarException()
        {
            Simulacao_VerifyExistsUser_withEmail(true);
            _repoMock.Setup(x => x.GetDataUserEmail(It.IsAny<string>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.ATIVO
            });
            _hashMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.LoginUser(new LoginUserRequest
            {
                Email = "exemplo@gmail.com"
            }));
            Assert.Equal("Senha incorreta", ex.Message);
        }

        [Fact]
        public async Task LoginUser_Caso_TudoOK()
        {
            Simulacao_VerifyExistsUser_withEmail(true);
            _repoMock.Setup(x => x.GetDataUserEmail(It.IsAny<string>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.ATIVO,
                ID = Guid.NewGuid(),
                Email = "exemplo@gmail.com",
                Role = OptionsRole.USER,
                Nome = "Rodolfo",
                PasswordHash = "amora0909!"
            });
            _hashMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _tokenMock.Setup(x => x.GenerativeToken(It.IsAny<User>())).ReturnsAsync("token");
            var result = await _usecase.LoginUser(new LoginUserRequest
            {
                Email = "exemplo@gmail.com",
                Senha = "amora0909!"
            });
            Assert.Equal("token", result);
        }
        [Fact]
        public async Task PatchUpdateUser_Caso_UsuarioNaoExista_LancarException()
        {
            Simulacao_verifyExistUser(false);
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PatchUpdateUser(Guid.NewGuid(), new UpdateUserRequest { }));
            Assert.Equal("O usuario não existe", ex.Message);
        }

        [Fact]
        public async Task PatchUpdateUser_Caso_UsuarioEstejaBloqueado_LancarException()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.DESATIVADO
            });
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PatchUpdateUser(Guid.NewGuid(), new UpdateUserRequest
            {

            }));
            Assert.Equal("Usuario bloqueado, entre em contato com o suporte.", ex.Message);
        }

        [Fact]
        public async Task PatchUpdateUser_Caso_EmailJaCadastrado_LancarException()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Email = "exemplo@gmail.com",
                Status = OptionsStatus.ATIVO
            });
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PatchUpdateUser(Guid.NewGuid(), new UpdateUserRequest
            {
                Email = "exemplo@gmail.com"
            }));
            Assert.Equal("Email ja cadastrado anteriormente.", ex.Message);
        }

        [Fact]
        public async Task PatchUpdateUser_Caso_TelefoneJaCadastrado_LancarException()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.ATIVO,
                Email = "exemplo@gmail.com",
                Telefone = "111111111"
            });
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.PatchUpdateUser(Guid.NewGuid(), new UpdateUserRequest
            {
                Email = "exemplo2@gmail.com",
                Telefone = "111111111"
            }));
            Assert.Equal("Telefone ja cadastrado anteriormente.", ex.Message);
        }

        [Fact]
        public async Task PatchUpdateUser_Caso_TudoOK()
        {
            Simulacao_verifyExistUser(true);
            _repoMock.Setup(x => x.GetDataUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Status = OptionsStatus.ATIVO,
                Email = "exemplo@gmail.com",
                Telefone = "111111111"
            });
            _hashMock.Setup(x => x.HashPassword(It.IsAny<string>())).ReturnsAsync("hash_password");
            _repoMock.Setup(x => x.PatchUpdateUser(It.IsAny<Guid>(), It.IsAny<User>())).ReturnsAsync(true);

            var result = await _usecase.PatchUpdateUser(Guid.NewGuid(), new UpdateUserRequest
            {
                Email = "exemplo2@gmail.com",
                Senha = "amora0909!",
                Telefone = "111111112"
            });
            Assert.True(result);
        }

    }
}
