using Microsoft.EntityFrameworkCore;

namespace FinTrackAI;

public class AccountsRepository : IAccountsRepository
{
    private readonly ApplicationDbContext _context;
    public AccountsRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> CreateAccount(Accounts Data)
    {
        try
        {
            var query = await _context.Accounts.AddAsync(Data);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<string> GetSaldo(Guid id)
    {
        try
        {
            var query = await _context.Accounts.Where(a => a.UserID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Conta não existe.");
            }
            return query.Saldo.ToString();
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<Accounts> GetDataAccounts(Guid id)
    {
        try
        {
            var query = await _context.Accounts.Where(a => a.UserID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                throw new RepositoryException("Conta não existe.");
            }
            return query;
        }
        catch (Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    public async Task<bool> VerifyAccountExists(Guid id)
    {
        try
        {
            var query = await _context.Accounts.Where(a => a.UserID == id).FirstOrDefaultAsync();
            if (query == null)
            {
                return false;
            }
            return true;
        } catch(Exception e)
        {
            throw new RepositoryException($"Repository Error: {e.Message}");
        }
    }
    
}
