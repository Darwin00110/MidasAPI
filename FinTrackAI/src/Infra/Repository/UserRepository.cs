using Microsoft.EntityFrameworkCore;

namespace FinTrackAI;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<User> VerifyExistsUser(Guid id)
    {
        try
        {
            var usuario = await _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                throw new RepositoryException("Usuario não existe.");
            }
            return usuario;
        } catch (Exception e)
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
    public async Task<bool> VerifyExistsUser_withTelephone(string telefone)
    {
        try
        {
            var query = await _context.Users.Where(u => u.Telefone == telefone).FirstOrDefaultAsync();
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
    public async Task<bool> VerifyExistsUser_withCPF(string cpf)
    {
        try
        {
            var usuario = await _context.Users.Where(u => u.CPF == cpf).FirstOrDefaultAsync();
            if(usuario == null)
            {
                return false;
            }
            return true;       
        } catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }

    public async Task<bool> VerifyExistsUser_withID(Guid id)
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
            if(usuario == null)
            {
                throw new RepositoryException("Usuario não encontrado");
            }
            return new ReadResponse
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Data_Nascimento = usuario.Data_nascimento,
                CPF = usuario.CPF
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
            .SetProperty(u => u.Senha, user.Senha)
            );
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
            await _context.Users.Where(u => u.ID == id).ExecuteDeleteAsync();
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
            var result = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if(result == null)
            {
                throw new RepositoryException("Usuario não encontrado");
            }
            return result;
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
        } catch(Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<bool> BlockAcessUser(Guid id)
    {
        try
        {
            var query = _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if(query == null)
            {
                throw new RepositoryException("Usuario não existe. ");
            }
            query.Result.StatusUsuario = OptionsStatusUser.DESATIVADO;
            await _context.SaveChangesAsync();
            return true;
        } catch(Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<bool> UnlockedAcessUser(Guid id)
    {
        try
        {
            var query = _context.Users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Usuario não existe. ");
            }
            query.Result.StatusUsuario = OptionsStatusUser.ATIVO;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
}
