using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers;

public class CategoriaController(AppDbContext ctx, Models.CategoriaValidator validator) : ControllerBase
{
    [HttpPost("categoria")]
    public async Task<IActionResult> Create([FromBody] JsonNode body)
    {
        var categoria = new Models.Categoria {
            Descricao = body["descricao"]?.ToString() ?? "",
            Finalidade = (Models.FinalidadeCategoria) (body["finalidade"]?.GetValue<int?>() ?? 0),
        };

        var validation = await validator.ValidateAsync(categoria);

        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        var created = ctx.Categorias.Add(categoria);
        ctx.SaveChanges();

        return Ok(new {
            id = created.Entity.Id,
            descricao = created.Entity.Descricao,
            finalidade = created.Entity.Finalidade
        });
    }

    [HttpGet("categoria")]
    public IActionResult GetAll()
    {
        var categorias = ctx.Categorias
            .Select(c => new {
                id = c.Id,
                descricao = c.Descricao,
                finalidade = c.Finalidade,
                created_at = c.CreatedAt,
                updated_at = c.UpdatedAt
            }).ToList();

        return Ok(categorias);
    }

    [HttpGet("categoria/{id}")]
    public IActionResult GetById(int id)
    {
        var categoria = ctx.Categorias
            .Where(c => c.Id == id)
            .Select(p => new {
                id = p.Id,
                descricao = p.Descricao,
                finalidade = p.Finalidade,
                created_at = p.CreatedAt,
                updated_at = p.UpdatedAt
            }).FirstOrDefault();

        if (categoria == null) return NotFound();

        return Ok(categoria);
    }

    [HttpGet("categoria/rel")]
    public async Task<IActionResult> Rel()
    {
        var result = ctx.Categorias
            .Include(p => p.Transacoes)
            .Select(c => new {
                id = c.Id,
                descricao = c.Descricao,
                receita = c.Transacoes.Where(t => t.Tipo == Models.TipoTransacao.Receita).Sum(t => t.Valor),
                despesa = c.Transacoes.Where(t => t.Tipo == Models.TipoTransacao.Despesa).Sum(t => t.Valor),
            })
            // segundo select p/ calcular o saldo
            .Select(c => new {
                c.id,
                c.descricao,
                c.receita,
                c.despesa,
                saldo=c.receita - c.despesa,
            });

        return Ok(result);
    }
}