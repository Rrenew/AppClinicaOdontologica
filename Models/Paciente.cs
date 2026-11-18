using System;
using System.Collections.Generic;

namespace AppClinicaOdontologica.Models;

public partial class Paciente
{
    public int IdPaciente { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateOnly DataNascimento { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Consultum> Consulta { get; set; } = new List<Consultum>();

    public virtual ICollection<Procedimento> Procedimentos { get; set; } = new List<Procedimento>();
}
