using System.Text.Json.Serialization;

namespace CalculateurAge.Models;

// Fonctionnalite 5 : une entree de l historique des calculs
public class HistoriqueEntree
{
    public string Nom { get; set; } = "";
    public DateTime DateNaissance { get; set; }
    public int Age { get; set; }
    public bool EstMajeur { get; set; }
    public string Anniversaire { get; set; } = "";
    public DateTime DateCalcul { get; set; }

    [JsonIgnore]
    public string Resultat => $"{Nom}, vous avez {Age} ans";

    [JsonIgnore]
    public string StatutPhrase
        => EstMajeur ? "Vous êtes majeur(e)." : "Vous êtes mineur(e).";

    [JsonIgnore]
    public string LigneHistorique => $"{Nom}, {Age} ans";

    [JsonIgnore]
    public string DateCalculTexte
        => $"Calculé le {DateCalcul:dd/MM/yyyy} à {DateCalcul:HH:mm}";
}
