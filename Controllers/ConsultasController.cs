using AppClinicaOdontologica.Models;
using AppClinicaOdontologica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppClinicaOdontologica.Controllers
{
    [Authorize]
    public class ConsultasController : Controller
    {
        private readonly DbClinicaOdontologicaContext _context;
        private readonly CpfProtector _cpf;

        public ConsultasController(DbClinicaOdontologicaContext context, CpfProtector cpf)
        {
            _context = context;
            _cpf = cpf;
        }

        public async Task<IActionResult> Index()
        {
            var consultas = await _context.Consulta
                .Include(c => c.IdPacienteNavigation)
                .Include(c => c.IdProcedimentoNavigation)
                .OrderBy(c => c.DataConsulta)
                .ToListAsync();

            var lista = consultas.Select(c => new ConsultaListaItem
            {
                IdConsulta = c.IdConsulta,
                DataConsulta = c.DataConsulta,
                Observacoes = c.Observacoes,
                PacienteNome = c.IdPacienteNavigation.Nome,
                PacienteCpf = CpfProtector.Mascarar(_cpf.Revelar(c.IdPacienteNavigation.Cpf)),
                PacienteNascimento = c.IdPacienteNavigation.DataNascimento,
                PacienteTelefone = c.IdPacienteNavigation.Telefone,
                PacienteEmail = c.IdPacienteNavigation.Email,
                ProcedimentoDescricao = c.IdProcedimentoNavigation.Descricao,
                ProcedimentoValor = c.IdProcedimentoNavigation.Valor
            }).ToList();

            return View(lista);
        }

        public async Task<IActionResult> Create()
        {
            await CarregarProcedimentos();
            return View(new ConsultaViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsultaViewModel vm)
        {
            var proc = vm.IdProcedimento == null
                ? null
                : await _context.Procedimentos.FindAsync(vm.IdProcedimento);

            if (vm.IdProcedimento != null && proc == null)
                ModelState.AddModelError(nameof(vm.IdProcedimento), "Procedimento não cadastrado.");

            if (!ModelState.IsValid)
            {
                await CarregarProcedimentos(vm.IdProcedimento);
                return View(vm);
            }

            var consulta = new Consultum
            {
                DataConsulta = vm.DataConsulta!.Value,
                Observacoes = vm.Observacoes,
                IdProcedimento = proc!.IdProcedimento,
                IdPaciente = proc.IdPaciente
            };

            _context.Consulta.Add(consulta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var c = await _context.Consulta
                .Include(x => x.IdPacienteNavigation)
                .Include(x => x.IdProcedimentoNavigation)
                .FirstOrDefaultAsync(x => x.IdConsulta == id);
            if (c == null) return NotFound();

            ViewBag.Paciente = c.IdPacienteNavigation.Nome;
            ViewBag.Procedimento = c.IdProcedimentoNavigation.Descricao;

            return View(new ConsultaViewModel
            {
                IdConsulta = c.IdConsulta,
                DataConsulta = c.DataConsulta,
                Observacoes = c.Observacoes,
                IdProcedimento = c.IdProcedimento
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConsultaViewModel vm)
        {
            if (id != vm.IdConsulta) return NotFound();

            var c = await _context.Consulta
                .Include(x => x.IdPacienteNavigation)
                .Include(x => x.IdProcedimentoNavigation)
                .FirstOrDefaultAsync(x => x.IdConsulta == id);
            if (c == null) return NotFound();

            ModelState.Remove(nameof(vm.IdProcedimento));

            if (!ModelState.IsValid)
            {
                ViewBag.Paciente = c.IdPacienteNavigation.Nome;
                ViewBag.Procedimento = c.IdProcedimentoNavigation.Descricao;
                return View(vm);
            }

            c.DataConsulta = vm.DataConsulta!.Value;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarProcedimentos(int? selecionado = null)
        {
            var itens = await _context.Procedimentos
                .Include(p => p.IdPacienteNavigation)
                .OrderBy(p => p.IdPacienteNavigation.Nome)
                .Select(p => new
                {
                    p.IdProcedimento,
                    Texto = p.Descricao + " — " + p.IdPacienteNavigation.Nome
                })
                .ToListAsync();

            ViewBag.Procedimentos = new SelectList(itens, "IdProcedimento", "Texto", selecionado);
        }
    }
}