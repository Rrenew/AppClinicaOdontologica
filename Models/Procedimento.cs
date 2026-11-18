using System;
using System.Collections.Generic;

namespace AppClinicaOdontologica.Models;

public partial class Procedimento
{
    public int IdProcedimento { get; set; }

    public string Descricao { get; set; } = null!;

    public decimal Valor { get; set; }

    public int IdPaciente { get; set; }

    public virtual ICollection<Consultum> Consulta { get; set; } = new List<Consultum>();

    public virtual Paciente IdPacienteNavigation { get; set; } = null!;
}
