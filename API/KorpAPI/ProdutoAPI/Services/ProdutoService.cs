using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using ProdutoAPI.Models;
using Repository.Context;
using Repository.Models;
using System.Text;

namespace ProdutoAPI.Services;

public class ProdutoService : IProdutoService
{
    private readonly KorpContext ctx;
    public PaginationModel Pagination { get; set; } = new PaginationModel();

    private static SemaphoreSlim sem = new SemaphoreSlim(1, 1);

    public ProdutoService(KorpContext context)
    {
        ctx = context;
    }

    public async Task<List<ProdutoModel>> GetProducts(string name)
    {
        IQueryable<Produto> query = ctx.Produtos;
        if (name.StartsWith('#'))
            query = ctx.Produtos.Where(e => e.Id.Contains(name.Substring(1).ToUpperInvariant()));
        else
            query = ctx.Produtos.Where(e => e.Descricao.StartsWith(name));

        Pagination.TotalItems = await query.CountAsync();

        if (Pagination.GetSkipCount() >= Pagination.TotalItems)
            throw new BaseException("A página solicitada não existe.");

        return (await query
            .Skip(Pagination.GetSkipCount())
            .Take(Pagination.PageSize)
            .ToListAsync())
            .ToObject<ProdutoModel>()!;
    }

    public async Task<ProdutoModel> AddProduct(ProdutoModel produto)
    {
        StringBuilder errorMessage = new StringBuilder();
        if (produto.Descricao == string.Empty)
            errorMessage.AppendLine("A descrição do produto é obrigatória.");
        if (produto.Saldo < 0)
            errorMessage.AppendLine("O saldo do produto não pode ser negativo.");

        if (await ctx.Produtos.AnyAsync(e => e.Id == produto.Codigo))
            errorMessage.AppendLine("Já existe um produto cadastrado com este código.");

        if (errorMessage.Length > 0)
            throw new BaseException(errorMessage.ToString());


        var p = produto.ToObject<Produto>();
        p.Id = produto.Codigo;
        await ctx.Produtos.AddAsync(p);
        await ctx.SaveChangesAsync();

        return p.ToObject<ProdutoModel>()!;
    }

    public async Task<ProdutoModel> UpdateProduct(ProdutoModel produto)
    {
        StringBuilder errorMessage = new StringBuilder();
        if (produto.Descricao == string.Empty)
            errorMessage.AppendLine("A descrição do produto é obrigatória.");
        if (produto.Saldo < 0)
            errorMessage.AppendLine("O saldo do produto não pode ser negativo.");

        if (errorMessage.Length > 0)
            throw new BaseException(errorMessage.ToString());

        var p = await ctx.Produtos.FirstOrDefaultAsync(e => e.Id == produto.Codigo)
            ?? throw new BaseException("Produto não encontrado.");

        p.Saldo = produto.Saldo;
        p.Descricao = produto.Descricao;

        ctx.Produtos.Update(p);
        await ctx.SaveChangesAsync();

        return produto;
    }

    public async Task CheckProdutos(List<ProdutoNotaModel> produtos)
    {
        await sem.WaitAsync();
        try
        {
            StringBuilder sb = new StringBuilder();
            var cods = produtos.Select(p => p.Codigo).ToList();
            var produtosDb = await ctx.Produtos.Where(p => cods.Contains(p.Id)).ToListAsync();
            foreach (var p in produtos)
            {
                var produtoDb = produtosDb.FirstOrDefault(pd => pd.Id == p.Codigo);
                if (produtoDb == null)
                {
                    sb.AppendLine($"O produto de código {p.Codigo} não foi encontrado.");
                    continue;
                }
                if (produtoDb.Saldo < p.Quantidade)
                {
                    sb.AppendLine($"O produto de código {p.Codigo} não possui saldo suficiente.");
                    continue;
                }

                produtoDb.Saldo -= p.Quantidade;
            }

            if (sb.Length > 0)
                throw new BaseException(sb.ToString());

            ctx.Produtos.UpdateRange(produtosDb);
            await ctx.SaveChangesAsync();
        }
        finally
        {
            sem.Release();
        }
    }
}