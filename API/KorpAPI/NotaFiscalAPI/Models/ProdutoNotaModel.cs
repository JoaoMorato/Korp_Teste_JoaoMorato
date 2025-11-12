using Newtonsoft.Json;

namespace NotaFiscalAPI.Models;

public class ProdutoNotaModel
{
    [JsonProperty("ProdutoId")]
    private string ProdutoIdSetter { set { Codigo = value; } }
    public string Codigo { get; set; }
    public int Quantidade { get; set; }
}
