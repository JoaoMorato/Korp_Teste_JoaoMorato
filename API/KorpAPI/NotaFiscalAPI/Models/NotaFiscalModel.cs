namespace NotaFiscalAPI.Models;

public class NotaFiscalModel
{
    public int Id { get; set; }
    public bool Aberta { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFechamento { get; set; }
    public List<ProdutoNotaModel> Produtos { get; set; }
}
