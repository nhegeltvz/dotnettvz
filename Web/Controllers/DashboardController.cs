using Data.Data;
using Data.Services.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web.Models;

namespace Web.Controllers;

[Route("dashboard")]
[Authorize(Roles = AppRoles.ADMIN_ROLE)]
public class DashboardController : Controller
{
    private readonly PlayerStore _playerStore;

    public DashboardController(PlayerStore playerStore) => _playerStore = playerStore;

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("parties")]
    public IActionResult Parties()
    {
        return View("Parties");
    }

    [HttpGet("match-records")]
    public IActionResult MatchRecords()
    {
        return View("MatchRecords");
    }

    [HttpGet("players")]
    public IActionResult Players()
    {
        return View("Players");
    }

    [HttpGet("playing-fields")]
    public IActionResult PlayingFields()
    {
        return View("Stadiums");
    }

    [HttpGet("logs")]
    public IActionResult Logs()
    {
        var logsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data", "Logs"));
        var entries = new List<LogEntryViewModel>();

        if (Directory.Exists(logsDir))
        {
            var files = Directory.GetFiles(logsDir, "*.clef")
                .OrderByDescending(f => f)
                .Take(2);

            foreach (var file in files)
            {
                try
                {
                    var lines = ReadLastLines(file, 500);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        try
                        {
                            var doc = JsonDocument.Parse(line);
                            var root = doc.RootElement;

                            var timestamp = root.TryGetProperty("@t", out var t) ? t.GetString() : null;
                            var level = root.TryGetProperty("@l", out var l) ? l.GetString() : "Information";
                            var template = root.TryGetProperty("@mt", out var mt) ? mt.GetString() : null;
                            var exception = root.TryGetProperty("@x", out var x) ? x.GetString() : null;

                            var props = root.EnumerateObject()
                                .Where(p => !p.Name.StartsWith('@'))
                                .ToDictionary(p => p.Name, p => p.Value.ToString());

                            var message = template;
                            if (message != null)
                                foreach (var prop in props)
                                    message = message.Replace("{" + prop.Key + "}", prop.Value);

                            entries.Add(new LogEntryViewModel
                            {
                                Timestamp = timestamp,
                                Level = level ?? "Information",
                                Message = message ?? "(no message)",
                                Exception = exception,
                                Properties = props
                            });
                        }
                        catch { /* skip malformed lines */ }
                    }
                }
                catch { /* skip unreadable files */ }
            }
        }

        entries = entries.OrderByDescending(e => e.Timestamp).ToList();
        return View(entries);
    }

    private static IEnumerable<string> ReadLastLines(string path, int count)
    {
        var lines = new LinkedList<string>();
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lines.AddLast(line);
            if (lines.Count > count)
                lines.RemoveFirst();
        }
        return lines;
    }
}
