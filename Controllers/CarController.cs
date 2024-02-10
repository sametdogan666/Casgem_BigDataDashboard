using BigDataDashboard.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using Newtonsoft.Json;

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

    public async Task<IActionResult> Index()
    {
        var benzinliAracSayisi = await _carRepository.GetBenzinliAracSayisiAsync();
        ViewData["BenzinliAracSayisi"] = benzinliAracSayisi;

        var beyazAracSayisi = await _carRepository.GetBeyazAracSayisiAsync();
        ViewData["BeyazAracSayisi"] = beyazAracSayisi;

        var otomatikAracSayisi = await _carRepository.GetOtomatikAracSayisiAsync();
        ViewData["OtomatikAracSayisi"] = otomatikAracSayisi;

        var sedanAracSayisi = await _carRepository.GetSedanAracSayisiAsync();
        ViewData["SedanAracSayisi"] = sedanAracSayisi;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Search(string searchString)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var allCars = await _carRepository.SearchCarsAsync(searchString);
        stopwatch.Stop();

        var json = JsonConvert.SerializeObject(allCars);

        return Json(new { queryTime = stopwatch.Elapsed, data = json });
    }
}
