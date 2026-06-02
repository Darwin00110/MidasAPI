using System.ComponentModel.DataAnnotations;

namespace MidasAPI;

public class GetAllUsersResponse
{
    public Guid ID { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? CPF { get; set; }
    public string? Telefone { get; set; }

}
