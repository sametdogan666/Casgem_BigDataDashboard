using BigDataDashboard.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace BigDataDashboard.Controllers;

public class CarController : Controller
{
    private readonly CarRepository _carRepository;
    private readonly IMemoryCache _memoryCache;



    public CarController(CarRepository carRepository, IMemoryCache memoryCache)
    {
        _carRepository = carRepository;
        _memoryCache = memoryCache;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        var benzinliAracSayisi = await _carRepository.GetBenzinliAracSayisiAsync();
        ViewData["BenzinliAracSayisi"] = benzinliAracSayisi;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string searchString)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var allCars = await _carRepository.SearchCarsAsync(searchString);
        stopwatch.Stop();

        return Json(new { queryTime = stopwatch.Elapsed, data = allCars });
    }
}
