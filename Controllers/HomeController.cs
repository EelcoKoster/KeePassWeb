using KeePassWeb.Data;
using KeePassWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace KeePassWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly KeePassApi _keePassApi;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, KeePassApi keePassApi)
        {
            _logger = logger;
            _keePassApi = keePassApi;
        }

        // GET: KeePass
        public IActionResult Index()
        {
            return View(_keePassApi.GetEntries());
        }

        // GET: KeePass/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KeePass/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ID,Group,Title,Username,Password,URL,Notes")] KeePassEntry keePassEntry)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"{User.Identity.Name} added entry with Title: '{keePassEntry.Title}'");
                _keePassApi.AddEntry(keePassEntry);
                return RedirectToAction(nameof(Index));
            }
            return View(keePassEntry);
        }

        // GET: KeePass/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keePassEntry = _keePassApi.GetEntry(id);
            if (keePassEntry == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"{User.Identity.Name} retrieved entry: {id} ({keePassEntry.Title})");
            return View(keePassEntry);
        }

        // POST: KeePass/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("ID,Group,Title,Username,Password,URL,Notes")] KeePassEntry keePassEntry)
        {
            if (id != keePassEntry.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation($"{User.Identity.Name} updated entry: {id} ({keePassEntry.Title})");
                _keePassApi.EditEntry(keePassEntry);
                return RedirectToAction(nameof(Index));
            }
            return View(keePassEntry);
        }

        // GET: KeePass/Delete/5
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keePassEntry = _keePassApi.GetEntry(id);
            if (keePassEntry == null)
            {
                return NotFound();
            }

            _logger.LogInformation($"{User.Identity.Name} will remove entry: {id} ({keePassEntry.Title})");
            return View(keePassEntry);
        }

        // POST: KeePass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _logger.LogInformation($"{User.Identity.Name} removed entry: {id}");
            _keePassApi.RemoveEntry(id);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public string GetPassword(string id)
        {
            _logger.LogInformation($"{User.Identity.Name} retrieved password for id: {id}");
            return _keePassApi.GetEntryPassword(id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
