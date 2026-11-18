using System.ComponentModel.DataAnnotations;

namespace AppClinicaOdontologica.Models
{
    public class ProcedimentoViewModel
    {
        public int IdProcedimento { get; set; }

        [Required(ErrorMessage = "Informe a descrição.")]
        [StringLength(150)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = "";

        [Required(ErrorMessage = "Informe o valor.")]
        [Range(0.01, 1000000, ErrorMessage = "Informe um valor maior que zero.")]
        public decimal? Valor { get; set; }

        [Required(ErrorMessage = "Selecione um paciente.")]
        [Display(Name = "Paciente")]
        public int? IdPaciente { get; set; }
    }
}