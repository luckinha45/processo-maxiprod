using FluentValidation;

namespace webapi.Models;

public class Pessoa : BaseModel
{
    public string Nome { get; set; } = "";

    public int Idade { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}

public class PessoaValidator : AbstractValidator<Pessoa>
{
    public PessoaValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Pessoa.Nome deve ter no máximo 200 caracteres!");

        RuleFor(p => p.Idade)
            .InclusiveBetween(0, 150)
            .WithMessage("Pessoa.Idade deve ser um valor entre 0 e 150!");
    }
}
