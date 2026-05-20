using System.ComponentModel.DataAnnotations;

namespace FinTrackAI.src.Application.DTOs;

public class GetEmailAsyncRequest
{
    [Required(ErrorMessage = "Email é obrigatorio.")]
    [EmailAddress(ErrorMessage = "Formato de Email invalido, Ex: (exemplo@gmail.com)")]
    public string Email { get; set; }
}
