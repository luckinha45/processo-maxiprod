using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers;

public class PessoaController(AppDbContext ctx, Models.PessoaValidator validator) : ControllerBase
{

    /// <summary>
    /// Cria uma nova <c>Pessoa</c> se os dados são validados.
    /// </summary>
    /// 
    /// <param name="body"> Os dados para fazer o cadastro da <c>Pessoa</c>, no formato JSON. </param>
    /// 
    /// <returns>
    /// Caso os dados foram validados, retorna a <c>Pessoa</c> criada.<br/>
    /// Caso não, retorna erro <c>400</c>, listando os campos incorretos.
    /// </returns>
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
    
    /// <summary>
    /// Retorna todas as <c>Pessoas</c> já criadas.
    /// </summary>
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

    /// <summary>
    /// Busca uma <c>Pessoa</c> com o id informado e o retorna.
    /// </summary>
    /// 
    /// <param name="id"> Identificador de uma <c>Pessoa</c>. </param>
    /// 
    /// <returns>
    /// Caso exista uma <c>Pessoa</c> com o id passado, retorna seus dados.<br/>
    /// Caso não, retorna erro <c>404</c>.
    /// </returns>
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

    /// <summary>
    /// Faz um update numa <c>Pessoa</c> existente se os dados são validados.
    /// </summary>
    /// 
    /// <param name="id"> Identificador de uma <c>Pessoa</c>. </param>
    /// <param name="body"> Os dados para fazer o update da <c>Pessoa</c>, no formato JSON. </param>
    /// 
    /// <returns>
    /// Caso exista uma <c>Pessoa</c> com o id passado, e os dados passados foram validados, faz o update e retorna a <c>Pessoa</c> com os novos dados.<br/>
    /// Caso não encontrado a pessoa, retorna erro <c>404</c>.<br/>
    /// Caso não validado os dados, retorna erro <c>400</c> e uma lista com os dados incorretos.<br/>
    /// </returns>
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

    /// <summary>
    /// Deleta uma <c>Pessoa</c> com o id informado.
    /// </summary>
    /// 
    /// <param name="id"> Identificador de uma <c>Pessoa</c>. </param>
    /// 
    /// <returns>
    /// Caso exista uma <c>Pessoa</c> com o id passado, a deleta.<br/>
    /// Caso não, retorna erro <c>404</c>.
    /// </returns>
    [HttpDelete("pessoa/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await ctx.Pessoas
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();
        
        return Ok();
    }

    /// <summary>
    /// Gera um relatório com as <c>Pessoas</c> existentes; Calculando a receita, despesa e saldo de cada.
    /// </summary>
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