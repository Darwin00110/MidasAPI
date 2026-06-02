using MidasAPI.src.Domain.Interfaces.IServices;
using MidasAPI.src.Domain.Interfaces.UserUseCase_NoService;

namespace MidasAPI.src.Application.UseCase;

public class UserUseCase_NoService : IUserUseCase_NoService
{
    private readonly IUserRepository _repo;
    private readonly IHashService _hashed;
    private readonly ITokenJWT _token;
    public UserUseCase_NoService(IUserRepository repo, IHashService hashed, ITokenJWT token)
    {
        _repo = repo;
        _hashed = hashed;
        _token = token;
    }
    public async Task<User> CreateUser(CreateUserRequest request)
    {
        var verifyExistUser = await _repo.VerifyExistsUser_withEmail(request.Email);
        if (verifyExistUser)
        {
            throw new UseCaseException("Usuario ja cadastrado");
        }
        var HashedSenha = await _hashed.HashPassword(request.Senha);
        var DataUser = new User
        {
            ID = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = HashedSenha.ToString(),
            CPF = request.CPF,
            Telefone = request.Telefone,
            Nome = request.Nome,
            DataNascimento = request.DataNascimento,
            Role = OptionsRole.USER,
            Status = OptionsStatus.ATIVO,
            CreatedAt = DateTime.Now,
        };
        DataUser.Validate_Create();
        var result = await _repo.CreateUser(DataUser);
        return result;
    }

    public async Task<ReadResponse> ReadUser(Guid id)
    {
        var verifyUserExists = await _repo.VerifyExistsUser(id);
        if (!verifyUserExists)
        {
            throw new UseCaseException("Usuario não existe. ");
        }
        var result = await _repo.ReadUser(id);
        return result;
    }

    public async Task<bool> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var verifyUser = await _repo.VerifyExistsUser(id);
        if (!verifyUser)
        {
            throw new UseCaseException("Usuario não existe .");
        }
        var DataUser = await _repo.GetDataUser(id);
        if (DataUser.Status == OptionsStatus.DESATIVADO)
        {
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        }
        if (request.Email == DataUser.Email)
        {
            throw new UseCaseException("Email cadastrado anteriormente, tente novamente.");
        }

        var SenhaHashed = await _hashed.HashPassword(request.Senha);
        var user = new User
        {
            Email = request.Email,
            Telefone = request.Telefone,
            PasswordHash = SenhaHashed.ToString(),
        };
        user.Validate_Email();
        user.Validate_Telefone();

        var result = await _repo.UpdateUser(id, user);
        return result;
    }
    public async Task<string> LoginUser(LoginUserRequest request)
    {
        var verifyUser = await _repo.VerifyExistsUser_withEmail(request.Email);
        if (!verifyUser)
        {
            throw new UseCaseException("Usuario não encontrado");
        }
        var GetDataUser = await _repo.GetDataUserEmail(request.Email);
        if (GetDataUser.Status == OptionsStatus.DESATIVADO)
        {
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        }
        var hashedPassword = await _hashed.VerifyPassword(request.Senha, GetDataUser.PasswordHash);
        if (!hashedPassword)
        {
            throw new UseCaseException("Senha incorreta");
        }
        User DataUser = new User
        {
            ID = GetDataUser.ID,
            Email = GetDataUser.Email,
            Role = GetDataUser.Role,
            Nome = GetDataUser.Nome
        };
        var createToken = await _token.GenerativeToken(DataUser);
        return createToken.ToString();
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        await _repo.VerifyExistsUser(id);
        var result = await _repo.DeleteUser(id);
        return result;
    }

    public async Task<bool> PatchUpdateUser(Guid id, UpdateUserRequest request)
    {
        var verifyUserExists = await _repo.VerifyExistsUser(id);
        if (!verifyUserExists)
        {
            throw new UseCaseException("O usuario não existe");
        }
        var DataUser = await _repo.GetDataUser(id);
        if (DataUser.Status == OptionsStatus.DESATIVADO)
        {
            throw new UseCaseException("Usuario bloqueado, entre em contato com o suporte.");
        }
        if (request.Email?.Equals(DataUser.Email) == true)
            throw new UseCaseException("Email ja cadastrado anteriormente.");

        if (request.Telefone?.Equals(DataUser.Telefone) == true)
            throw new UseCaseException("Telefone ja cadastrado anteriormente.");

        var user = new User
        {
            Email = request.Email ?? DataUser.Email,
            Telefone = request.Telefone ?? DataUser.Telefone,
            PasswordHash = request.Senha ?? DataUser.PasswordHash,
        };
        var senhaHash = await _hashed.HashPassword(user.PasswordHash);
        user.PasswordHash = senhaHash;

        return await _repo.PatchUpdateUser(id, user);
    }
}
