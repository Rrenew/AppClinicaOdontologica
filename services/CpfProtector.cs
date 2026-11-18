using Microsoft.AspNetCore.DataProtection;

namespace AppClinicaOdontologica.Services
{
    public class CpfProtector
    {
        private readonly IDataProtector _protector;

        public CpfProtector(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("Paciente.CPF");
        }

        public string Proteger(string cpf)
        {
            var digitos = new string(cpf.Where(char.IsDigit).ToArray());
            return _protector.Protect(digitos);
        }

        public string? Revelar(string? protegido)
        {
            if (string.IsNullOrEmpty(protegido)) return null;
            try { return _protector.Unprotect(protegido); }
            catch { return null; }
        }

        public static string Mascarar(string? digitos)
        {
            if (digitos == null || digitos.Length != 11) return "***.***.***-**";
            return $"***.{digitos.Substring(3, 3)}.{digitos.Substring(6, 3)}-**";
        }
    }
}