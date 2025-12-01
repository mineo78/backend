using Microsoft.AspNetCore.Mvc;
using GamingPlatform.Models;
using System.Collections.Generic;
using GamingPlatform.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GamingPlatform.Controllers
{
    public class SpeedTypingController : Controller
    {
        public static Dictionary<string, SpeedTypingGame> lobbies = new Dictionary<string, SpeedTypingGame>();
        private readonly IHubContext<SpeedTypingHub> _hubContext;

        public SpeedTypingController(IHubContext<SpeedTypingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View(lobbies);
        }

        [HttpPost]
        public IActionResult CreateLobby(string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                ViewData["ErrorMessage"] = "Le nom du lobby ne peut pas être vide.";
                return View("Index", lobbies); // Retourne à la vue Index avec un message d'erreur
            }

            if (lobbies.ContainsKey(host))
            {
                ViewData["ErrorMessage"] = "Un lobby avec ce nom existe déjà.";
                return View("Index", lobbies); // Retourne à la vue Index avec un message d'erreur
            }

            // Ajoutez le lobby avec le nom comme clé
            var newLobby = new SpeedTypingGame(host);
            lobbies[host] = newLobby;

            // Notifier les autres clients via SignalR
            _hubContext.Clients.All.SendAsync("LobbyCreated", host);

            return RedirectToAction("Lobby", new { id = host });
        }





        public IActionResult Lobby(string id)
        {
            if (id == null)
            {
                return BadRequest("Id cannot be null.");
            }

            if (lobbies.ContainsKey(id))
            {
                ViewBag.LobbyId = id;
                return View(lobbies[id]);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult JoinLobby(string id, string player)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(player))
            {
                TempData["Message"] = "L'identifiant du lobby ou le nom du joueur ne peut pas être vide.";
                return RedirectToAction("Lobby", new { id });
            }

            if (!lobbies.ContainsKey(id))
            {
                TempData["Message"] = "Lobby introuvable.";
                return RedirectToAction("Index");
            }

            var lobby = lobbies[id];

            if (lobby.Players.Contains(player))
            {
                TempData["Message"] = "Un joueur avec ce nom existe déjà dans ce lobby.";
                return RedirectToAction("Lobby", new { id });
            }

            lobby.AddPlayer(player);

            HttpContext.Session.SetString("CurrentPlayer", player);
            HttpContext.Session.SetString("LobbyId", id);

            _hubContext.Clients.Group(id).SendAsync("PlayerJoined", player);
            return RedirectToAction("Lobby", new { id });
        }

/*        [HttpPost]
        public IActionResult StartGame(string id)
        {
			return Ok();*/
			//Console.WriteLine($" controller StartGame called for lobbyId: {id}");

			//if (string.IsNullOrEmpty(id))
			//{
			//    return BadRequest("L'identifiant du lobby ne peut pas être null ou vide.");
			//}

			//if (!lobbies.ContainsKey(id))
			//{
			//    return NotFound("Lobby introuvable.");
			//}

			//var lobby = lobbies[id];

			//try
			//{
			//    string errorMessage = lobby.StartGame();

			//    if (!string.IsNullOrEmpty(errorMessage))
			//    {
			//        TempData["Message"] = errorMessage;
			//        return BadRequest(errorMessage);
			//        return RedirectToAction("Lobby", new { id });
			//    }

			//    _hubContext.Clients.Group(id).SendAsync("GameStarted", lobby.Players);
			//    //return RedirectToAction("Game", new { id });
			//    return Ok();
			//}
			//catch (Exception ex)
			//{
			//    TempData["Message"] = $"Erreur lors du démarrage du jeu : {ex.Message}";
			//    return RedirectToAction("Lobby", new { id });
			//}
		//}

        public IActionResult Game(string id, string currentPlayer)
        {
            if (string.IsNullOrEmpty(id) || !lobbies.ContainsKey(id))
            {
                return NotFound("Lobby introuvable.");
            }

            var lobby = lobbies[id];

            if (!lobby.IsStarted)
            {
                TempData["Message"] = "Le jeu n'a pas encore démarré.";
                return RedirectToAction("Lobby", new { id });
            }


            if (string.IsNullOrEmpty(currentPlayer) || !lobby.Players.Contains(currentPlayer))
            {
                TempData["Message"] = "Vous n'êtes pas autorisé à accéder à ce jeu.";
                return RedirectToAction("Lobby", new { id });
            }

            ViewBag.CurrentPlayer = currentPlayer;
            ViewBag.LobbyId = id;

            return View(lobby);
        }

        [HttpPost]
        [Route("SpeedTyping/UpdateProgress")]
        public IActionResult UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.Player))
            {
                return BadRequest("Données invalides.");
            }

            if (!lobbies.ContainsKey(request.Id))
            {
                return NotFound("Lobby introuvable.");
            }

            var lobby = lobbies[request.Id];
            // Mettez à jour la progression du joueur
            if (lobby.Progress.ContainsKey(request.Player))
            {
                lobby.Progress[request.Player] = (int)request.Progress;
            }
            else
            {
                lobby.Progress.Add(request.Player, (int)request.Progress);
            }

            // Notifier les autres clients via SignalR
            _hubContext.Clients.Group(request.Id).SendAsync("ProgressUpdated", request.Player, request.Progress);

            return Ok();
        }

        public IActionResult GameOver(string id)
        {
            return View("GameOver");
        }

        public class UpdateProgressRequest
        {
            public string Id { get; set; }
            public string Player { get; set; }
            public double Progress { get; set; }
        }

    }
}
