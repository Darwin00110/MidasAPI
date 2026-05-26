using Microsoft.EntityFrameworkCore;

namespace FinTrackAI;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> VerifyExistsUser(Guid id)
    {
        try
        {
            var usuario = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return false;
            }
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }

    public async Task<bool> VerifyExistsUser_withEmail(string email)
    {
        try
        {
            var usuario = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (usuario == null)
            {
                return false;
            }
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<bool> VerifyExistsUser_withCPF(string CPF)
    {
        try
        {
            var query = await _context.Users.Where(u => u.CPF == CPF).FirstOrDefaultAsync();
            if (query == null)
            {
                return false;
            }
            return true;

        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<User> CreateUser(User user)
    {
        try
        {
            var query = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<ReadResponse> ReadUser(Guid id)
    {
        try
        {
            var usuario = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                throw new RepositoryException("Usuario não encontrado");
            }
            return new ReadResponse
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Data_Nascimento = usuario.DataNascimento,
                CPF = usuario.CPF,
            };
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }

    public async Task<bool> UpdateUser(Guid id, User user)
    {
        try
        {
            var query = await _context.Users.Where(u => u.ID == id).ExecuteUpdateAsync(setters =>
            setters.SetProperty(u => u.Email, user.Email)
            .SetProperty(u => u.Telefone, user.Telefone)
            .SetProperty(u => u.PasswordHash, user.PasswordHash)
            );
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        try
        {
            var query = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Usuario não existe. ");
            }
            query.Status = OptionsStatus.DESATIVADO;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }



    public async Task<List<GetAllUsersResponse>> GetAllUsers()
    {
        try
        {
            var Usuarios = await _context.Users.Select(u => new GetAllUsersResponse
            {
                ID = u.ID,
                CPF = u.CPF,
                Nome = u.Nome,
                Email = u.Email,
                Telefone = u.Telefone
            }).ToListAsync();
            return Usuarios;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<bool> BlockAcessUser(Guid id)
    {
        try
        {
            var query = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Usuario não existe. ");
            }
            query.Status = OptionsStatus.DESATIVADO;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<User> GetDataUserEmail(string email)
    {
        try
        {
            var query = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Usuario não existe. ");
            }
            return query;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<bool> UnlockedAcessUser(Guid id)
    {
        try
        {
            var query = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Usuario não existe. ");
            }
            query.Status = OptionsStatus.ATIVO;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<bool> PatchUpdateUser(Guid id, User user)
    {
        try
        {
            var query = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                return false;
            }
            if (query.Email != null || query.Email != "")
            {
                query.Email = user.Email;
            }
            if (query.Telefone != null || query.Telefone != "")
            {
                query.Telefone = user.Telefone;
            }
            if (query.PasswordHash != null || query.PasswordHash != "")
            {
                query.PasswordHash = user.PasswordHash;
            }
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<User> GetDataUser(Guid id)
    {
        try
        {
            var query = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Usuario não existe");
            }
            return query;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
}
