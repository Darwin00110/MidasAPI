using FinTrackAI.src.Domain.Interfaces.IServices;
using FinTrackAI.src.Domain.Interfaces.UserUseCase_NoService;

namespace FinTrackAI.src.Application.UseCase;

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
            Senha = HashedSenha.ToString(),
            CPF = request.CPF,
            Telefone = request.Telefone,
            Nome = request.Nome,
            Data_nascimento = request.DataNascimento,
            Role = OptionsRole.USER
        };
        DataUser.Validate_Create();
        var result = await _repo.CreateUser(DataUser);
        return result;
    }

    public async Task<ReadResponse> ReadUser(Guid id)
    {
        await _repo.VerifyExistsUser(id);
        var result = await _repo.ReadUser(id);
        return result;
    }

    public async Task<bool> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var verifyUser = await _repo.VerifyExistsUser(id);

        var user = new User
        {
            Email = verifyUser.Email,
            Nome = verifyUser.Nome
        };
        user.Validate_Email();
        user.Validate_Nome();

        var result = await _repo.UpdateUser(id, user);
        return result;
        
    }
    public async Task<string> LoginUser(LoginUserRequest request)
    {
        var verifyUser = await _repo.VerifyExistsUser_withEmail(request.Email);
        if(!verifyUser)
        {
            throw new UseCaseException("Usuario não encontrado");
        }
        var GetDataUser = await _repo.GetDataUserEmail(request.Email);
        var hashedPassword = await _hashed.VerifyPassword(request.Senha, GetDataUser.Senha);
        if(!hashedPassword)
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
}
