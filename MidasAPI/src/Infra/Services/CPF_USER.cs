namespace MidasAPI;

using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using Superpower.Parsers;

public class CPF_USER : ICPF_USER
{
    private readonly HttpClient _client;
    public CPF_USER()
    {
        _client = new HttpClient();
        var token = Environment.GetEnvironmentVariable("TOKEN_API_VALIDATECPF");
        _client.BaseAddress = new Uri("https://api.cpf-brasil.org/");
        _client.DefaultRequestHeaders.Add("X-API-Key", token);
    }

    public async Task<CPFResponse_data> GetDataUser(string cpf)
    {
        var response = await _client.GetAsync($"cpf/{cpf}");
        var raw = await response.Content.ReadAsStringAsync();
        if (raw.Contains("Não autorizado"))
        {
            throw new ServiceException("Serviço temporariamente indisponivel, Tente novamente mais tarde");
        }
        if (response.StatusCode.Equals(400))
        {
            throw new ServiceException("Erro ao obter dados do CPF");
        }
        var result = await response.Content.ReadFromJsonAsync<CPFResponse_data>();
        if (result == null)
            throw new ServiceException("Erro ao obter dados do CPF");
        if (!result.Success)
        {
            throw new ServiceException("CPF invalido");
        }
        return result;
    }
}

public class CPFResponse_valid
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}

public class CPFData
{
    [JsonPropertyName("NOME")]
    public required string Nome { get; set; }
    [JsonPropertyName("NASC")]
    public string? Data_nascimento { get; set; }
    [JsonPropertyName("CPF")]
    public string? CPF { get; set; }
    [JsonPropertyName("SEXO")]
    public string? Sexo { get; set; }
    [JsonPropertyName("NOME_MAE")]
    public string? Nome_mae { get; set; }
}

public class CPFResponse_data
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("data")]
    public required CPFData Data { get; set; }
}
