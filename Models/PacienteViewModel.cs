using System.ComponentModel.DataAnnotations;

namespace AppClinicaOdontologica.Models
{
    public class PacienteViewModel
    {
        public int IdPaciente { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100)]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Informe o CPF.")]
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = "";

        [Required(ErrorMessage = "Informe a data de nascimento.")]
        [Display(Name = "Data de nascimento")]
        public DateOnly? DataNascimento { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(100)]
        public string? Email { get; set; }
    }
}