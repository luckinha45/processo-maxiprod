namespace webapi.Models;

using System.ComponentModel.DataAnnotations;
using FluentValidation;

public enum FinalidadeCategoria
{
    Despesa = 1,
    Receita = 2,
    Ambas = 3
}

public class Categoria : BaseModel
{
    public string Descricao { get; set; } = "";

    public FinalidadeCategoria Finalidade { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}

public class CategoriaValidator : AbstractValidator<Categoria>
{
    public CategoriaValidator()
    {
        RuleFor(c => c.Descricao)
            .NotEmpty()
            .MaximumLength(400)
            .WithMessage("Categoria.Descricao deve ter no máximo 400 caracteres!");

        RuleFor(c => c.Finalidade)
            .IsInEnum()
            .WithMessage("Categoria.Finalidade deve ser um valor entre 1, 2 ou 3!");
    }
}
