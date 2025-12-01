using System.Collections.Generic;

namespace GamePlateforme.Models
{
    public class Lobby
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Players { get; set; } = new List<string>();
        public bool IsStarted { get; set; } = false;
    }
}
