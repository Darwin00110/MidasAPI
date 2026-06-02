using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidasAPI.src.Application.UseCase;
using Moq;

namespace MidasAPI.Tests
{
    public class UserUseCase_WithServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<ICPF_USER> _serviceMock;
        private readonly UserUseCase_WithService _usecase;
        public UserUseCase_WithServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _serviceMock = new Mock<ICPF_USER>();
            _usecase = new UserUseCase_WithService(_repoMock.Object, _serviceMock.Object);
        }
        [Fact]
        public async Task CreateUser_WithService_USUARIO_NAO_EXISTE()
        {
            _serviceMock.Setup(x => x.GetDataUser(It.IsAny<string>())).ThrowsAsync(new RepositoryException("Serviço temporariamente indisponivel, Tente novamente mais tarde"));

            var ex = await Assert.ThrowsAsync<RepositoryException>(() => _usecase.CreateUser_withCPF(new CreateUserRequest_withCPF()));
            Assert.Equal("Serviço temporariamente indisponivel, Tente novamente mais tarde", ex.Message);
        }
        [Fact]
        public async Task CreateUser_withCPF_DadosValidos_DeveCriarUsuario()
        {
            _serviceMock
                .Setup(x => x.GetDataUser(It.IsAny<string>()))
                .ReturnsAsync(new CPFResponse_data
                {
                    Data = new CPFData
                    {
                        Nome = "Darwin",
                        Data_nascimento = "23/04/2000"
                    }
                });

            _repoMock.Setup(x => x.CreateUser(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            var request = new CreateUserRequest_withCPF
            {
                Email = "darwin@gmail.com",
                Senha = "senha123",
                CPF = "13139585602",
                Telefone = "312378980"
            };

            var result = await _usecase.CreateUser_withCPF(request);

            Assert.NotNull(result);
            Assert.Equal("Darwin", result.Nome);
        }
    }
}
