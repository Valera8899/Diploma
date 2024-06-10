using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Monitor_2.Data;
using Monitor_2.Models.Currency;

namespace Monitor_2.Controllers
{
    public class CurrentCpValueController : Controller
    {
        private readonly Monitor_2Context _context;

        public CurrentCpValueController(Monitor_2Context context)
        {
            _context = context;
        }

        // GET: CurrentCpValue
        public async Task<IActionResult> Index()
        {
            var monitor_2Context = _context.CurrentCpValue.Include(c => c.CurrencyPair);
            return View(await monitor_2Context.ToListAsync());
        }

        // GET: CurrentCpValue/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var CurrentCpValue = await _context.CurrentCpValue
                .Include(c => c.CurrencyPair)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (CurrentCpValue == null)
            {
                return NotFound();
            }

            return View(CurrentCpValue);
        }

        // GET: CurrentCpValue/Create
        public IActionResult Create()
        {
            ViewData["CurrencyPairId"] = new SelectList(_context.CurrencyPair, "Id", "Id");
            return View();
        }

        // POST: CurrentCpValue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReleaseDate,BuyRate,SellRate,CurrencyPairId")] CurrentCpValue CurrentCpValue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(CurrentCpValue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrencyPairId"] = new SelectList(_context.CurrencyPair, "Id", "Id", CurrentCpValue.CurrencyPairId);
            return View(CurrentCpValue);
        }

        // GET: CurrentCpValue/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var CurrentCpValue = await _context.CurrentCpValue.FindAsync(id);
            if (CurrentCpValue == null)
            {
                return NotFound();
            }
            ViewData["CurrencyPairId"] = new SelectList(_context.CurrencyPair, "Id", "Id", CurrentCpValue.CurrencyPairId);
            return View(CurrentCpValue);
        }

        // POST: CurrentCpValue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReleaseDate,BuyRate,SellRate,CurrencyPairId")] CurrentCpValue CurrentCpValue)
        {
            if (id != CurrentCpValue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(CurrentCpValue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CurrentCpValueExists(CurrentCpValue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrencyPairId"] = new SelectList(_context.CurrencyPair, "Id", "Id", CurrentCpValue.CurrencyPairId);
            return View(CurrentCpValue);
        }

        // GET: CurrentCpValue/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var CurrentCpValue = await _context.CurrentCpValue
                .Include(c => c.CurrencyPair)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (CurrentCpValue == null)
            {
                return NotFound();
            }

            return View(CurrentCpValue);
        }

        // POST: CurrentCpValue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var CurrentCpValue = await _context.CurrentCpValue.FindAsync(id);
            if (CurrentCpValue != null)
            {
                _context.CurrentCpValue.Remove(CurrentCpValue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CurrentCpValueExists(int id)
        {
            return _context.CurrentCpValue.Any(e => e.Id == id);
        }
    }
}
