using AppClinicaOdontologica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppClinicaOdontologica.Controllers
{
    [Authorize]
    public class ProcedimentosController : Controller
    {
        private readonly DbClinicaOdontologicaContext _context;

        public ProcedimentosController(DbClinicaOdontologicaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Procedimentos
                .Include(p => p.IdPacienteNavigation)
                .OrderBy(p => p.Descricao)
                .ToListAsync();
            return View(lista);
        }

        public async Task<IActionResult> Create()
        {
            await CarregarPacientes();
            return View(new ProcedimentoViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProcedimentoViewModel vm)
        {
            await ValidarPaciente(vm);
            if (!ModelState.IsValid)
            {
                await CarregarPacientes(vm.IdPaciente);
                return View(vm);
            }

            var proc = new Procedimento
            {
                Descricao = vm.Descricao,
                Valor = vm.Valor!.Value,
                IdPaciente = vm.IdPaciente!.Value
            };

            _context.Procedimentos.Add(proc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _context.Procedimentos.FindAsync(id);
            if (p == null) return NotFound();

            var vm = new ProcedimentoViewModel
            {
                IdProcedimento = p.IdProcedimento,
                Descricao = p.Descricao,
                Valor = p.Valor,
                IdPaciente = p.IdPaciente
            };

            await CarregarPacientes(vm.IdPaciente);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProcedimentoViewModel vm)
        {
            if (id != vm.IdProcedimento) return NotFound();

            await ValidarPaciente(vm);
            if (!ModelState.IsValid)
            {
                await CarregarPacientes(vm.IdPaciente);
                return View(vm);
            }

            var p = await _context.Procedimentos.FindAsync(id);
            if (p == null) return NotFound();

            p.Descricao = vm.Descricao;
            p.Valor = vm.Valor!.Value;
            p.IdPaciente = vm.IdPaciente!.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.Procedimentos.FindAsync(id);
            if (p == null) return NotFound();

            if (await _context.Consulta.AnyAsync(c => c.IdProcedimento == id))
            {
                TempData["Erro"] = "Não é possível excluir: o procedimento possui consultas agendadas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Procedimentos.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarPacientes(int? selecionado = null)
        {
            var pacientes = await _context.Pacientes.OrderBy(p => p.Nome).ToListAsync();
            ViewBag.Pacientes = new SelectList(pacientes, "IdPaciente", "Nome", selecionado);
        }

        private async Task ValidarPaciente(ProcedimentoViewModel vm)
        {
            if (vm.IdPaciente != null &&
                !await _context.Pacientes.AnyAsync(p => p.IdPaciente == vm.IdPaciente))
            {
                ModelState.AddModelError(nameof(vm.IdPaciente), "Paciente não cadastrado.");
            }
        }
    }
}