using AppClinicaOdontologica.Models;
using AppClinicaOdontologica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppClinicaOdontologica.Controllers
{
    [Authorize]
    public class PacientesController : Controller
    {
        private readonly DbClinicaOdontologicaContext _context;
        private readonly CpfProtector _cpf;

        public PacientesController(DbClinicaOdontologicaContext context, CpfProtector cpf)
        {
            _context = context;
            _cpf = cpf;
        }

        public async Task<IActionResult> Index(string? termo)
        {
            var pacientes = await _context.Pacientes.OrderBy(p => p.Nome).ToListAsync();

            if (!string.IsNullOrWhiteSpace(termo))
            {
                var t = termo.Trim();
                var digitos = new string(t.Where(char.IsDigit).ToArray());

                pacientes = pacientes.Where(p =>
                    p.Nome.Contains(t, StringComparison.OrdinalIgnoreCase) ||
                    (digitos.Length > 0 && (_cpf.Revelar(p.Cpf) ?? "").Contains(digitos))
                ).ToList();
            }

            ViewBag.Termo = termo;

            var lista = pacientes.Select(p => new PacienteViewModel
            {
                IdPaciente = p.IdPaciente,
                Nome = p.Nome,
                Cpf = CpfProtector.Mascarar(_cpf.Revelar(p.Cpf)),
                DataNascimento = p.DataNascimento,
                Telefone = p.Telefone,
                Email = p.Email
            }).ToList();

            return View(lista);
        }

        public IActionResult Create() => View(new PacienteViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteViewModel vm)
        {
            ValidarCpf(vm);
            if (!ModelState.IsValid) return View(vm);

            var paciente = new Paciente
            {
                Nome = vm.Nome,
                Cpf = _cpf.Proteger(vm.Cpf),
                DataNascimento = vm.DataNascimento!.Value,
                Telefone = vm.Telefone,
                Email = vm.Email
            };

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _context.Pacientes.FindAsync(id);
            if (p == null) return NotFound();

            var vm = new PacienteViewModel
            {
                IdPaciente = p.IdPaciente,
                Nome = p.Nome,
                Cpf = _cpf.Revelar(p.Cpf) ?? "",
                DataNascimento = p.DataNascimento,
                Telefone = p.Telefone,
                Email = p.Email
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PacienteViewModel vm)
        {
            if (id != vm.IdPaciente) return NotFound();

            ValidarCpf(vm);
            if (!ModelState.IsValid) return View(vm);

            var p = await _context.Pacientes.FindAsync(id);
            if (p == null) return NotFound();

            p.Nome = vm.Nome;
            p.Cpf = _cpf.Proteger(vm.Cpf);
            p.DataNascimento = vm.DataNascimento!.Value;
            p.Telefone = vm.Telefone;
            p.Email = vm.Email;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Pacientes.FindAsync(id);
            if (p == null) return NotFound();

            bool temVinculo =
                await _context.Procedimentos.AnyAsync(x => x.IdPaciente == id) ||
                await _context.Consulta.AnyAsync(x => x.IdPaciente == id);

            if (temVinculo)
            {
                TempData["Erro"] = "Não é possível excluir: o paciente possui procedimentos ou consultas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Pacientes.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void ValidarCpf(PacienteViewModel vm)
        {
            var digitos = new string((vm.Cpf ?? "").Where(char.IsDigit).ToArray());
            if (digitos.Length != 11)
                ModelState.AddModelError(nameof(vm.Cpf), "O CPF deve ter 11 dígitos.");
        }
    }
}