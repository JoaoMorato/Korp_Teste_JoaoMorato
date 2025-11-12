using Infrastructure.Models;
using ProdutoAPI.Models;

namespace ProdutoAPI.Services;

public interface IProdutoService
{
    PaginationModel Pagination { get; set; }

    Task<List<ProdutoModel>> GetProducts(string name);
    Task<ProdutoModel> AddProduct(ProdutoModel produto);
    Task<ProdutoModel> UpdateProduct(ProdutoModel produtoModel);
    Task CheckProdutos(List<ProdutoNotaModel> produtos);
}