using Newtonsoft.Json;

namespace ProdutoAPI.Models;

public class ProdutoModel
{
    [JsonProperty("Id")]
    private string IdSetter { set { Codigo = value; } }
    public string Codigo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Saldo { get; set; }
}
