using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers;

public class TransacaoController(AppDbContext ctx, Models.TransacaoValidator validator) : ControllerBase
{
    /// <summary>
    /// Cria uma nova <c>Transacao</c> se os dados são validados.
    /// </summary>
    /// 
    /// <param name="body"> Os dados para fazer o cadastro da <c>Transacao</c>, no formato JSON. </param>
    /// 
    /// <returns>
    /// Caso os dados foram validados, retorna a <c>Transacao</c> criada.<br/>
    /// Caso <c>Categoria</c> ou <c>Pessoa</c> não foram encontrados com os ids fornecidos, retorna erro <c>404</c>.<br/>
    /// Caso haja dados incorretos, retorna erro <c>400</c>, listando os erros.
    /// </returns>
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

    /// <summary>
    /// Retorna todas as <c>Transacoes</c> já criadas.
    /// </summary>
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

    /// <summary>
    /// Busca uma <c>Transacao</c> com o id informado e o retorna.
    /// </summary>
    /// 
    /// <param name="id"> Identificador de uma <c>Transacao</c>. </param>
    /// 
    /// <returns>
    /// Caso exista uma <c>Transacao</c> com o id passado, retorna seus dados.<br/>
    /// Caso não, retorna erro <c>404</c>.
    /// </returns>
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