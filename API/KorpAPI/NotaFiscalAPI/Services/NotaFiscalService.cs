using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NotaFiscalAPI.Models;
using Repository.Context;
using Repository.Models;
using System.Text;

namespace NotaFiscalAPI.Services;

public class NotaFiscalService : INotaFiscalService
{
    private readonly KorpContext ctx;

    public NotaFiscalService(KorpContext ctx)
    {
        this.ctx = ctx;
    }

    public PaginationModel Pagination { get; set; } = new PaginationModel();

    public async Task<List<NotaFiscalModel>> GetNotas(int id)
    {
        IQueryable<NotaFiscal> query = ctx.NotaFiscals.AsQueryable();
        if (id > 0)
            query = ctx.NotaFiscals.Where(nf => nf.Id == id);

        Pagination.TotalItems = await query.CountAsync();

        if (Pagination.GetSkipCount() >= Pagination.TotalItems)
            throw new BaseException("A página solicitada não existe.");

        var notaFiscal = await query
            .Skip(Pagination.GetSkipCount())
            .Take(Pagination.PageSize)
            .Include(e => e.ProdutoNotaFiscals)
            .ToListAsync();

        List<NotaFiscalModel> notasModel = notaFiscal.ToObject<NotaFiscalModel>()!;
        foreach(var n in notasModel)
        {
            var l = notaFiscal.FirstOrDefault(x => x.Id == n.Id);
            n.Produtos = l!.ProdutoNotaFiscals.ToObject<ProdutoNotaModel>()!;
        }

        return notasModel;
    }

    public async Task<NotaFiscalModel> AddNotaFiscal(NotaFiscalModel notaFiscal)
    {
        var nf = notaFiscal.ToObject<NotaFiscal>()!;

        foreach (var p in notaFiscal.Produtos)
            nf.ProdutoNotaFiscals.Add(new ProdutoNotaFiscal
            {
                ProdutoId = p.Codigo,
                Quantidade = p.Quantidade
            });

        nf.DataCriacao = DateTime.UtcNow;

        await ctx.NotaFiscals.AddAsync(nf);
        await ctx.SaveChangesAsync();

        notaFiscal.Id = nf.Id;
        return notaFiscal;
    }

    public async Task<NotaFiscalModel> UpdateNotaFiscal(NotaFiscalModel notaFiscal)
    {
        var nf = await ctx.NotaFiscals
            .Include(e => e.ProdutoNotaFiscals)
            .FirstOrDefaultAsync(n => n.Id == notaFiscal.Id)
            ?? throw new BaseException("Nota fiscal não encontrada.");

        var l = notaFiscal.Produtos;

        foreach (var item in nf.ProdutoNotaFiscals)
        {
            var p = l.FirstOrDefault(x => x.Codigo == item.ProdutoId);
            if (p == null)
            {
                ctx.ProdutoNotaFiscals.Remove(item);
                continue;
            }
            item.Quantidade = p.Quantidade;
            l.Remove(p);
        }

        foreach (var p in l)
        {
            nf.ProdutoNotaFiscals.Add(new ProdutoNotaFiscal
            {
                Id = 0,
                Quantidade = p.Quantidade,
                ProdutoId = p.Codigo
            });
        }

        ctx.Update(nf);
        await ctx.SaveChangesAsync();

        return notaFiscal;
    }

    public async Task InprimirNotaFiscal(int id)
    {
        var nf = await ctx.NotaFiscals
            .Include(e => e.ProdutoNotaFiscals)
            .FirstOrDefaultAsync(n => n.Id == id)
            ?? throw new BaseException("Nota fiscal não encontrada.");

        if (!nf.Aberta)
            throw new BaseException("A nota fiscal já está fechada.");

        if(nf.ProdutoNotaFiscals.Count == 0)
            throw new BaseException("A nota fiscal não possui produtos.");

        nf.Aberta = false;
        nf.DataFechamento = DateTime.UtcNow;

        HttpClient client = new HttpClient();


        StringContent content = new StringContent(JsonConvert.SerializeObject(nf.ProdutoNotaFiscals.ToObject<ProdutoNotaModel>()), Encoding.UTF8, "application/json");

        var resp = await client.PostAsync("http://produtoapi:8080/Produto/Check", content);

        Console.WriteLine(await resp.Content.ReadAsStringAsync());

        if (resp.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            throw new BaseException((await resp.Content.ReadFromJsonAsync<BaseResponse>()).Message);
        else if(!resp.IsSuccessStatusCode)
            throw new BaseException("Erro ao verificar os produtos na API de Produtos.");

        ctx.NotaFiscals.Update(nf);
        await ctx.SaveChangesAsync();
    }
}
