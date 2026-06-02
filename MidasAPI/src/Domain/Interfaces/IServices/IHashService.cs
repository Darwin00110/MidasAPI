namespace MidasAPI.src.Domain.Interfaces.IServices
{
    public interface IHashService
    {
        public Task<string> HashPassword(string password);
        public Task<bool> VerifyPassword(string password, string hash);
    }
}
