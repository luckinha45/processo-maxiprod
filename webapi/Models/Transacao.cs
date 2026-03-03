namespace webapi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

public enum TipoTransacao
{
    Despesa = 1,
    Receita = 2
}

public class Transacao : BaseModel
{
    [StringLength(400, ErrorMessage = "Transacao.Descricao deve ter no máximo 400 caracteres.")]
    public string Descricao { get; set; } = "";

    [Range(0, double.MaxValue, ErrorMessage = "Transacao.Valor deve ser positivo.")]
    public decimal Valor { get; set; }

    public TipoTransacao Tipo { get; set; }

    [ForeignKey("CategoriaId")]
    public required Categoria Categoria { get; set; }

    [ForeignKey("PessoaId")]
    public required Pessoa Pessoa { get; set; }
}


public class TransacaoValidator : AbstractValidator<Transacao>
{
    public TransacaoValidator()
    {
        RuleFor(t => t.Descricao)
            .MaximumLength(400)
            .WithMessage("Transacao.Descricao deve ter no máximo 400 caracteres!");

        RuleFor(t => t.Valor)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Transacao.Valor deve ser positivo!");

        RuleFor(t => t.Tipo)
            .IsInEnum()
            .WithMessage("Transacao.Tipo deve ser um valor 1 ou 2!");

        RuleFor(t => t.Categoria)
            .NotNull()
            .WithMessage("Transacao.CategoriaID não encontrada!");

        RuleFor(t => t.Categoria)
            .Must((t, c) => (int)t.Tipo == (int)c.Finalidade || c.Finalidade == FinalidadeCategoria.Ambas)
            .WithMessage("Transacao.Categoria.Finalidade deve ser igual ao Transacao.Tipo, ou Transacao.Categoria.Finalidade ser tipo AMBAS!");

        RuleFor(t => t.Pessoa)
            .NotNull()
            .WithMessage("Transacao.PessoaID não encontrada!");

        RuleFor(t => t.Pessoa)
            .Must((t, p) => !(p.Idade < 18 && t.Tipo == TipoTransacao.Receita))
            .WithMessage("Transacao.Pessoa.Idade deve ser maior ou igual a 18 quando Transação.Tipo for Receita!");
    }
}
