namespace FinTrackAI;

public interface ICPF_USER
{
    public Task<CPFResponse_data> GetDataUser(string cpf);
}
