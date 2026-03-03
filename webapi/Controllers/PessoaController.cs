using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers;

public class PessoaController(AppDbContext ctx, Models.PessoaValidator validator) : ControllerBase
{


    [HttpPost("pessoa")]
    public async Task<IActionResult> Create([FromBody] JsonNode body)
    {
        var pessoa = new Models.Pessoa {
            Nome = body["nome"]?.ToString() ?? "",
            Idade = body["idade"]?.GetValue<int?>() ?? 0
        };

        var validation = await validator.ValidateAsync(pessoa);

        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        var created = ctx.Pessoas.Add(pessoa);
        ctx.SaveChanges();

        return Ok(new {
            id = created.Entity.Id,
            nome = created.Entity.Nome,
            idade = created.Entity.Idade
        });
    }

    [HttpGet("pessoa")]
    public IActionResult GetAll()
    {
        var pessoas = ctx.Pessoas
            .Select(p => new {
                id = p.Id,
                nome = p.Nome,
                idade = p.Idade,
                created_at = p.CreatedAt,
                updated_at = p.UpdatedAt
            }).ToList();

        return Ok(pessoas);
    }

    [HttpGet("pessoa/{id}")]
    public IActionResult GetById(int id)
    {
        var pessoa = ctx.Pessoas
            .Where(p => p.Id == id)
            .Select(p => new {
                id = p.Id,
                nome = p.Nome,
                idade = p.Idade,
                created_at = p.CreatedAt,
                updated_at = p.UpdatedAt
            }).FirstOrDefault();

        if (pessoa == null) return NotFound();

        return Ok(pessoa);
    }

    [HttpPut("pessoa/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] JsonNode body)
    {
        var pessoa = ctx.Pessoas.Find(id);
        if (pessoa == null) return NotFound();

        pessoa.Nome = body["nome"]?.ToString() ?? pessoa.Nome;
        pessoa.Idade = body["idade"]?.GetValue<int?>() ?? pessoa.Idade;

        var validation = await validator.ValidateAsync(pessoa);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        ctx.SaveChanges();

        return Ok(pessoa);
    }

    [HttpDelete("pessoa/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await ctx.Pessoas
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();
        
        return Ok();
    }

    [HttpGet("pessoa/rel")]
    public async Task<IActionResult> Rel()
    {
        var result = ctx.Pessoas
            .Include(p => p.Transacoes)
            .Select(p => new {
                id = p.Id,
                nome = p.Nome,
                receita = p.Transacoes.Where(t => t.Tipo == Models.TipoTransacao.Receita).Sum(t => t.Valor),
                despesa = p.Transacoes.Where(t => t.Tipo == Models.TipoTransacao.Despesa).Sum(t => t.Valor),
            })
            // segundo select p/ calcular o saldo
            .Select(p => new {
                p.id,
                p.nome,
                p.receita,
                p.despesa,
                saldo=p.receita - p.despesa,
            });

        return Ok(result);
    }
}