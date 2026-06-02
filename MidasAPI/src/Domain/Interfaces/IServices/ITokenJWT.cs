namespace MidasAPI.src.Domain.Interfaces.IServices
{
    public interface ITokenJWT
    {
        public Task<string> GenerativeToken(User user);
    }
}
