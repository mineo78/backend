using GamingPlatform.Hubs;
using GamingPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GamingPlatform.Controllers
{
    public class MorpionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}

