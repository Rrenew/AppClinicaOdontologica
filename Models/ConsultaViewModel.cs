using System.ComponentModel.DataAnnotations;

namespace AppClinicaOdontologica.Models
{
    public class ConsultaViewModel
    {
        public int IdConsulta { get; set; }

        [Required(ErrorMessage = "Informe a data e a hora da consulta.")]
        [Display(Name = "Data da consulta")]
        public DateTime? DataConsulta { get; set; }

        [StringLength(255)]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        [Required(ErrorMessage = "Selecione um procedimento.")]
        [Display(Name = "Procedimento")]
        public int? IdProcedimento { get; set; }
    }

    public class ConsultaListaItem
    {
        public int IdConsulta { get; set; }
        public DateTime DataConsulta { get; set; }
        public string? Observacoes { get; set; }

        public string PacienteNome { get; set; } = "";
        public string PacienteCpf { get; set; } = "";
        public DateOnly PacienteNascimento { get; set; }
        public string? PacienteTelefone { get; set; }
        public string? PacienteEmail { get; set; }

        public string ProcedimentoDescricao { get; set; } = "";
        public decimal ProcedimentoValor { get; set; }
    }
}