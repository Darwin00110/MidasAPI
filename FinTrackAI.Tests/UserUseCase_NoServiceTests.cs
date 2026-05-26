using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinTrackAI.src.Application.UseCase;
using FinTrackAI.src.Domain.Interfaces.IServices;
using Moq;

namespace FinTrackAI.Tests
{
    public class UserUseCase_NoServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<IHashService> _hashMock;
        private readonly Mock<ITokenJWT> _tokenMock;
        private readonly UserUseCase_NoService _usecase;

        private void Simulacao_UsuarioNaoExiste_ID()
        {
            _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ThrowsAsync(new UseCaseException("Usuario não existe."));
        }
        private void Simulacao_UsuarioExiste_ID()
        {
            _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Email = "darwin@gmail.com",
                Nome = "Darwin"
            });
        }
        private void Simulacao_UsuarioNaoExiste_Email()
        {
            _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(false);
        }
        private void Simulacao_UsuarioExiste_Email()
        {
            _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(true);
        }

        private void Simulacao_HashSenha_VALIDA()
        {
            _hashMock.Setup(x => x.HashPassword("chuvamolhada8787")).ReturnsAsync("hashed_password");
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
        public async Task CreateUserTest_CasoUsuario_EXISTA()
        {
            Simulacao_UsuarioExiste_Email();
            Simulacao_HashSenha_VALIDA();
            var request = new CreateUserRequest
            {
                Nome = "Rodolfo",
                Email = "rodolfo2323@gmail.com",
                Telefone = "319732348",
                DataNascimento = new DateTime(2000, 05, 23),
                CPF = "13139585602",
                Senha = "chuvamolhada8787"
            };
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.CreateUser(request));
            Assert.Equal("Usuario ja cadastrado", ex.Message);
        }
        [Fact]
        public async Task CreateUserTest_CasoUsuario_NAO_EXISTA()
        {
            Simulacao_UsuarioNaoExiste_Email();
            Simulacao_HashSenha_VALIDA();
            _repoMock.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync(new User
            {
                Email = "rodolfo2323@gmail.com",
                Nome = "Rodolfo",
            });
            var request = new CreateUserRequest
            {
                Nome = "Rodolfo",
                Email = "rodolfo2323@gmail.com",
                Telefone = "319732348",
                DataNascimento = new DateTime(2000, 05, 23),
                CPF = "13139585602",
                Senha = "chuvamolhada8787"
            };
            var result = await _usecase.CreateUser(request);
            Assert.NotNull(result);
            Assert.Equal("rodolfo2323@gmail.com", result.Email);
        }
        //=============READ User=================//
        [Fact]
        public async Task ReadUserTest_CasoUsuario_NAO_EXISTA()
        {
            var ID_User = Guid.NewGuid();
            Simulacao_UsuarioNaoExiste_ID();
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.ReadUser(ID_User));
            Assert.Equal("Usuario não existe.", ex.Message);
        }
        [Fact]
        public async Task ReadUser_CasoUsuario_EXISTA()
        {
            var ID_User = Guid.NewGuid();
            Simulacao_UsuarioExiste_ID();
            _repoMock.Setup(x => x.ReadUser(It.IsAny<Guid>())).ReturnsAsync(new ReadResponse
            {
                Nome = "Darwin",
            });
            var result = await _usecase.ReadUser(ID_User);
            Assert.NotNull(result);
            Assert.Equal("Darwin", result.Nome);
        }
        //=============UPDATE User=================//
        [Fact]
        public async Task UpdateUser_EmailIgual_DeveLancarException()
        {
            _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Email = "darwin@gmail.com",
                Nome = "Darwin"
            });

            var request = new UpdateUserRequest { Email = "darwin@gmail.com" };

            var ex = await Assert.ThrowsAsync<UseCaseException>(() =>
                _usecase.UpdateUser(Guid.NewGuid(), request));

            Assert.Equal("Email cadastrado anteriormente, tente novamente.", ex.Message);
        }

        [Fact]
        public async Task UpdateUser_CasoUsuario_EXISTA()
        {
            _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(new User
            {
                Email = "darwin@gmail.com",
                Nome = "Darwin"
            });
            _hashMock.Setup(x => x.HashPassword(It.IsAny<string>())).ReturnsAsync("Hashed_Password");
            _repoMock.Setup(x => x.UpdateUser(It.IsAny<Guid>(), It.IsAny<User>())).ReturnsAsync(true);

            // Email DIFERENTE do cadastrado
            var request = new UpdateUserRequest
            {
                Email = "novo@gmail.com",
                Senha = "senha",
                Telefone = "319732348"
            };

            var result = await _usecase.UpdateUser(Guid.NewGuid(), request);
            Assert.True(result);
        }
        [Fact]
        public async Task LoginUser_CasoSenhaSeja_INCORRETA()
        {
            _repoMock.Setup(x => x.VerifyExistsUser_withEmail(It.IsAny<string>())).ReturnsAsync(true);
            _repoMock.Setup(x => x.GetDataUserEmail(It.IsAny<string>())).ReturnsAsync(new User
            {
                PasswordHash = "senha_segredo"
            });
            _hashMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new UseCaseException("Senha incorreta"));
            var request = new LoginUserRequest
            {

            };
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.LoginUser(request));
            Assert.Equal("Senha incorreta", ex.Message);
        }

        //===============DELETE User=================//
        [Fact]
        public async Task DeleteUser_CasoUsuario_NAO_EXISTA()
        {
            _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ThrowsAsync(new UseCaseException("Usuario não existe"));
            var ex = await Assert.ThrowsAsync<UseCaseException>(() => _usecase.DeleteUser(Guid.NewGuid()));
            Assert.Equal("Usuario não existe", ex.Message);
        }
        [Fact]
        public async Task DeleteUser_CasoUsuario_EXISTA()
        {
            _repoMock.Setup(x => x.VerifyExistsUser(It.IsAny<Guid>())).ReturnsAsync(new User { });
            _repoMock.Setup(x => x.DeleteUser(It.IsAny<Guid>())).ReturnsAsync(true);
            var result = await _usecase.DeleteUser(Guid.NewGuid());
            Assert.True(result);
        }

    }
}
