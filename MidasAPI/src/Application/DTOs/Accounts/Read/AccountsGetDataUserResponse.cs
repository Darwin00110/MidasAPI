using MidasAPI.src.Domain.Entities;

namespace MidasAPI;

public class AccountsGetDataUserResponse
{
    public string? Nome { get; set; }
    public string? N_Agencia { get; set; }
    public string? Saldo { get; set; }
    public string? Chave_PIX { get; set; }
    public ICollection<Transacao>? TransacaoEnviadas { get; set; }
    public ICollection<Transacao>? TransacaoRecebidas { get; set; }
}
