using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers;

public class CategoriaController(AppDbContext ctx, Models.CategoriaValidator validator) : ControllerBase
{
    /// <summary>
    /// Cria uma nova <c>Categoria</c> se os dados são validados.
    /// </summary>
    /// 
    /// <param name="body"> Os dados para fazer o cadastro da <c>Categoria</c>, no formato JSON. </param>
    /// 
    /// <returns>
    /// Caso os dados foram validados, retorna a <c>Categoria</c> criada.<br/>
    /// Caso não, retorna erro <c>400</c>, listando os campos incorretos.
    /// </returns>
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

    /// <summary>
    /// Retorna todas as <c>Categorias</c> já criadas.
    /// </summary>
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

    /// <summary>
    /// Busca uma <c>Categoria</c> com o id informado e o retorna.
    /// </summary>
    /// 
    /// <param name="id"> Identificador de uma <c>Categoria</c>. </param>
    /// 
    /// <returns>
    /// Caso exista uma <c>Categoria</c> com o id passado, retorna seus dados.<br/>
    /// Caso não, retorna erro <c>404</c>.
    /// </returns>
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

    /// <summary>
    /// Gera um relatório com as <c>Categorias</c> existentes; Calculando a receita, despesa e saldo de cada.
    /// </summary>
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