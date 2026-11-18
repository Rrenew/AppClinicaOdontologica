using System;
using System.Collections.Generic;

namespace AppClinicaOdontologica.Models;

public partial class Consultum
{
    public int IdConsulta { get; set; }

    public DateTime DataConsulta { get; set; }

    public string? Observacoes { get; set; }

    public int IdPaciente { get; set; }

    public int IdProcedimento { get; set; }

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;

    public virtual Procedimento IdProcedimentoNavigation { get; set; } = null!;
}
