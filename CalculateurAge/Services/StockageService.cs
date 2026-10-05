using System.Text.Json;
using System.Text.Json.Serialization;
using CalculateurAge.Models;
using Microsoft.Maui.Storage;

namespace CalculateurAge.Services;

// Fonctionnalites 1 et 5 : sauvegarde locale du dernier resultat et de l historique avec Preferences
public class StockageService
{
    private const string CleDernierResultat = "derniere_sauvegarde_v2";
    private const string CleHistorique = "historique_v2";

    public void SauvegarderDernierResultat(HistoriqueEntree entree)
        => Preferences.Default.Set(CleDernierResultat,
            JsonSerializer.Serialize(entree, StockageJsonContext.Default.HistoriqueEntree));

    public HistoriqueEntree? ChargerDernierResultat()
    {
        string json = Preferences.Default.Get(CleDernierResultat, "");
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize(json, StockageJsonContext.Default.HistoriqueEntree);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public void SauvegarderHistorique(IEnumerable<HistoriqueEntree> entrees)
        => Preferences.Default.Set(CleHistorique,
            JsonSerializer.Serialize(entrees.ToList(), StockageJsonContext.Default.ListHistoriqueEntree));

    public List<HistoriqueEntree> ChargerHistorique()
    {
        string json = Preferences.Default.Get(CleHistorique, "");
        if (string.IsNullOrEmpty(json)) return new List<HistoriqueEntree>();
        try
        {
            return JsonSerializer.Deserialize(json, StockageJsonContext.Default.ListHistoriqueEntree)
                   ?? new List<HistoriqueEntree>();
        }
        catch (JsonException)
        {
            return new List<HistoriqueEntree>();
        }
    }

    public void EffacerHistorique() => Preferences.Default.Remove(CleHistorique);
}

[JsonSerializable(typeof(HistoriqueEntree))]
[JsonSerializable(typeof(List<HistoriqueEntree>))]
internal partial class StockageJsonContext : JsonSerializerContext
{
}
