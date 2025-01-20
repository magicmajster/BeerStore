using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeerStore.Data;
using BeerStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

public class BeersController : Controller
{
    private readonly ApplicationDbContext _context;

    public BeersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        PopulateCountries();
        var beers = await _context.Beers
            .Include(b => b.Country) // Ładuje dane kraju
            .ToListAsync();

        ViewBag.Countries = await _context.Countries.ToListAsync();
        return View(beers);
    }

    protected void PopulateCountries()
    {
        ViewBag.Countries = _context.Countries.ToList();
    }


    public async Task<IActionResult> ByCountry(int id)
    {
        // Pobiera piwa tylko dla danego kraju
        var beers = await _context.Beers
            .Include(b => b.Country) // Ładujemy dane kraju
            .Where(b => b.CountryId == id)
            .ToListAsync();

        // Jeśli brak piw, można wyświetlić komunikat w widoku
        if (!beers.Any())
        {
            ViewBag.Message = "Brak piw z tego kraju.";
        }

        // Przekazanie piw do widoku
        return View("Index", beers);
    }


    // GET: Beers/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // Pobranie piwa z bazy danych
        var beer = await _context.Beers.FindAsync(id);
        if (beer == null)
        {
            return NotFound(); // Jeśli nie znaleziono piwa, zwracamy błąd 404
        }

        // Pobranie listy krajów i przypisanie do ViewBag.Countries
        ViewBag.Countries = await _context.Countries
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToListAsync();

        return View(beer); // Przekazujemy model Beer do widoku
    }


    // POST: Beers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Beer beer)
    {
        if (!ModelState.IsValid)
        {
            try
            {
                // Sprawdź, czy piwo istnieje
                var existingBeer = _context.Beers.Find(beer.Id);
                if (existingBeer == null)
                {
                    return NotFound(); // Piwo nie istnieje
                }

                // Zaktualizuj dane piwa
                existingBeer.Name = beer.Name;
                existingBeer.Price = beer.Price;
                existingBeer.Description = beer.Description;
                existingBeer.CountryId = beer.CountryId;

                // Zapisz zmiany
                _context.Update(existingBeer);
                _context.SaveChanges();

                // Przekierowanie na listę piw
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the changes: " + ex.Message);
            }
        }

        // Przy błędzie w formularzu, załaduj kraje i zwróć widok
        ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name", beer.CountryId);
        return View(beer);
    }




    private bool BeerExists(int id)
    {
        return _context.Beers.Any(e => e.Id == id);
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var beer = _context.Beers
            .Include(b => b.Country)
            .FirstOrDefault(b => b.Id == id);

        if (beer == null)
        {
            return NotFound(); // Jeśli piwo nie istnieje, zwróć błąd 404
        }

        return View(beer);
    }

    [HttpGet]
    public IActionResult Create()
    {

        ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Beer model)
    {
        if (ModelState.IsValid)
        {

            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name");
            return View(model);
        }

        _context.Beers.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // GET: Beers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var beer = await _context.Beers
            .Include(b => b.Country)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (beer == null)
        {
            return NotFound();
        }

        return View(beer);
    }

    // POST: Beers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var beer = await _context.Beers.FindAsync(id);
        if (beer != null)
        {
            _context.Beers.Remove(beer);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

}
