using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers;

public class TransacaoController(AppDbContext ctx, Models.TransacaoValidator validator) : ControllerBase
{
    [HttpPost("transacao")]
    public async Task<IActionResult> Create([FromBody] JsonNode body)
    {
        // puxa a categoria pelo id
        var categoriaId = body["categoriaId"]?.GetValue<int?>() ?? 0;
        var categoria = ctx.Categorias.Find(categoriaId);
        if (categoria == null)
        {
            return NotFound($"Categoria com id {categoriaId} não encontrada!");
        }

        // puxa a pessoa pelo id
        var pessoaId = body["pessoaId"]?.GetValue<int?>() ?? 0;
        var pessoa = ctx.Pessoas.Find(pessoaId);
        if (pessoa == null)
        {
            return NotFound($"Pessoa com id {pessoaId} não encontrada!");
        }

        var transacao = new Models.Transacao {
            Descricao = body["descricao"]?.ToString() ?? "",
            Valor = body["valor"]?.GetValue<decimal?>() ?? 0,
            Tipo = (Models.TipoTransacao) (body["tipo"]?.GetValue<int?>() ?? 0),
            Categoria = categoria,
            Pessoa = pessoa
        };


        var validate = await validator.ValidateAsync(transacao);

        if (!validate.IsValid)
        {
            return BadRequest(validate.Errors);
        }

        var created = ctx.Transacoes.Add(transacao);
        ctx.SaveChanges();

        return Ok(new
        {
            id = created.Entity.Id,
            descricao = created.Entity.Descricao,
            valor = created.Entity.Valor,
            tipo = created.Entity.Tipo,
            categoriaId = created.Entity.Categoria.Id,
            pessoaId = created.Entity.Pessoa.Id,
        });
    }

    [HttpGet("transacao")]
    public IActionResult GetAll()
    {
        var transacoes = ctx.Transacoes
            .Take(100)
            .Include(t => t.Categoria)
            .Include(t => t.Pessoa)
            .Select(t => new {
                id = t.Id,
                descricao = t.Descricao,
                categoriaId = t.Categoria.Id,
                pessoaId = t.Pessoa.Id,
                created_at = t.CreatedAt,
                updated_at = t.UpdatedAt
            }).ToList();

        return Ok(transacoes);
    }

    [HttpGet("transacao/{id}")]
    public IActionResult GetById(int id)
    {
        var transacao = ctx.Transacoes
            .Where(c => c.Id == id)
            .Select(t => new {
                id = t.Id,
                descricao = t.Descricao,
                categoriaId = t.Categoria.Id,
                pessoaId = t.Pessoa.Id,
                created_at = t.CreatedAt,
                updated_at = t.UpdatedAt
            }).FirstOrDefault();

        if (transacao == null) return NotFound();

        return Ok(transacao);
    }
}