using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;

namespace GamingPlatform.Models
{
    public class SpeedTypingGame
    {
        // Hôte du jeu
        public string Host { get; set; }

        // Liste des joueurs
        public List<string> Players { get; private set; } = new List<string>();

        // Progression des joueurs (en pourcentage ou points)
        public Dictionary<string, int> Progress { get; private set; } = new Dictionary<string, int>();

        // Indicateur si le jeu a commencé
        public bool IsStarted { get; private set; } = false;

        // Texte à taper pour le jeu
        public string TextToType { get; private set; } = "Welcome to the Speed Typing Game!";

        // Constructeur
        public SpeedTypingGame(string host)
        {
            Host = host;
        }

        // Ajouter un joueur au jeu
        public void AddPlayer(string player)
        {
            if (!IsStarted && !Players.Contains(player))
            {
                Players.Add(player);
                Progress[player] = 0;
            }
        }

        // Mettre à jour la progression d'un joueur
        public void UpdateProgress(string player, int progress)
        {
            if (Players.Contains(player) && IsStarted)
            {
                Progress[player] = Math.Clamp(progress, 0, 100); // S'assurer que la progression est entre 0 et 100
            }
        }

        // Commencer le jeu
        public string StartGame()
        {
            if (Players.Count < 2)
            {
                return "Il faut un minimum de 2 joueurs pour commencer le jeu.";
            }

            IsStarted = true;
            GenerateRandomText();
            return null; // Aucun message d'erreur, le jeu peut commencer
        }

        // Calculer le score d'un joueur en fonction de l'entrée utilisateur
        public int CalculateScore(string player, string userInput)
        {
            if (!Players.Contains(player))
            {
                throw new InvalidOperationException("Le joueur n'existe pas dans ce lobby.");
            }

            if (!IsStarted)
            {
                throw new InvalidOperationException("Le jeu n'a pas encore commencé.");
            }

            // Normalisation des chaînes pour comparaison insensible aux accents
            string normalizedTextToType = TextToType.Normalize(NormalizationForm.FormD);
            string normalizedUserInput = userInput.Normalize(NormalizationForm.FormD);

            if (string.Compare(normalizedTextToType, normalizedUserInput, CultureInfo.InvariantCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
            {
                Progress[player] = 100; // 100% pour marquer la fin
                return 1000; // Score pour avoir terminé
            }
            else if (normalizedTextToType.StartsWith(normalizedUserInput, StringComparison.OrdinalIgnoreCase))
            {
                return userInput.Length * 10; // Score basé sur la longueur correcte
            }

            return 0; // Aucun score si l'entrée ne correspond pas
        }

        // Générer un texte aléatoire à taper

        private void GenerateRandomText()
    {
        var sampleTexts = new List<string>
    {
        "Le soleil brille après la pluie.",
        "Un sourire est le plus court chemin vers un cœur.",
        "La vie est belle quand on sourit.",
        "Qui vole un œuf vole un bœuf.",
        "C'est en forgeant qu'on devient forgeron.",
        "Petit à petit, l'oiseau fait son nid.",
        "Rien ne sert de courir, il faut partir à point.",
        "Le chat retombe toujours sur ses pattes.",
        "Mieux vaut tard que jamais.",
        "Les chiens aboient, la caravane passe.",
        "Une pomme par jour éloigne le médecin.",
        "L'habit ne fait pas le moine.",
        "À cœur vaillant rien d'impossible.",
        "Un tiens vaut mieux que deux tu l'auras.",
        "L'arbre cache souvent la forêt.",
        "On ne fait pas d'omelette sans casser des œufs.",
        "La curiosité est un vilain défaut.",
        "Trop de cuisiniers gâtent la sauce.",
        "La nuit porte conseil.",
        "Quand on parle du loup, on en voit la queue.",
        "Les cordonniers sont les plus mal chaussés.",
        "Il vaut mieux prévenir que guérir.",
        "Chassez le naturel, il revient au galop.",
        "Ce n'est pas la mer à boire.",
        "L'espoir fait vivre.",
        "Pierre qui roule n'amasse pas mousse.",
        "Mieux vaut être seul que mal accompagné.",
        "Qui ne risque rien n'a rien.",
        "Un coup de chance peut changer une vie.",
        "Le travail acharné finit toujours par payer.",
        "Quand on veut, on peut.",
        "Il n'y a pas de fumée sans feu.",
        "Le silence est d'or.",
        "On récolte ce que l'on sème.",
        "Chaque chose en son temps.",
        "Une main lave l'autre.",
        "Les bons comptes font les bons amis.",
        "Il n'y a pas de petit profit.",
        "Les paroles s'envolent, les écrits restent.",
        "La patience est la mère de toutes les vertus.",
        "Ne mets pas tous tes œufs dans le même panier.",
        "Quand le chat n'est pas là, les souris dansent.",
        "Rome ne s'est pas faite en un jour.",
        "Tout vient à point à qui sait attendre.",
        "L'argent ne fait pas le bonheur.",
        "Une épine d'expérience vaut une forêt de conseils.",
        "Fais ce que je dis, pas ce que je fais.",
        "L'erreur est humaine.",
        "On n'apprend pas à un vieux singe à faire des grimaces."
    };

        var random = new Random();
        TextToType = HttpUtility.HtmlDecode(sampleTexts[random.Next(sampleTexts.Count)]);
    }

    // Obtenir les joueurs triés par progression décroissante
    public List<KeyValuePair<string, int>> GetLeaderboard()
        {
            return new List<KeyValuePair<string, int>>(Progress.OrderByDescending(p => p.Value));
        }
    }
}
