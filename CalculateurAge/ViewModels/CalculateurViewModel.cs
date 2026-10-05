using System.Collections.ObjectModel;
using CalculateurAge.Models;
using CalculateurAge.Services;

namespace CalculateurAge.ViewModels;

// Contient l ETAT de l ecran et les ACTIONS possibles.
public class CalculateurViewModel : BaseViewModel
{
    private const int TailleMaxHistorique = 30;
    private readonly StockageService _stockage = new StockageService();

    // Champs prives : la vraie donnee.
    private string _nom = "";
    private DateTime _dateNaissance
        = DateTime.Today.AddYears(-20);
    private string _resultat = "";
    private bool _resultatVisible;
    private bool _estMajeur;
    private string _statutPhrase = "";
    private string _anniversaireTitre = "";
    private string _anniversaireDetail = "";
    private string _anniversaireResume = "";
    private HistoriqueEntree? _dernierResultat;

    // Proprietes publiques : ce que le XAML voit.
    public string Nom
    {
        get => _nom;
        set
        {
            if (SetField(ref _nom, value))
            {
                OnPropertyChanged(nameof(NomManquant));
                CalculerCommand.Rafraichir();
            }
        }
    }

    public DateTime DateNaissance
    {
        get => _dateNaissance;
        set
        {
            if (SetField(ref _dateNaissance, value))
            {
                OnPropertyChanged(nameof(DateFuture));
                CalculerCommand.Rafraichir();
            }
        }
    }

    public string Resultat
    {
        get => _resultat;
        set => SetField(ref _resultat, value);
    }

    public bool ResultatVisible
    {
        get => _resultatVisible;
        set => SetField(ref _resultatVisible, value);
    }

    // Validations : nom vide et date de naissance dans le futur
    public bool NomManquant => string.IsNullOrWhiteSpace(Nom);

    public bool DateFuture => DateNaissance.Date > DateTime.Today;

    // Fonctionnalite 3 : statut majeur ou mineur
    public bool EstMajeur
    {
        get => _estMajeur;
        set => SetField(ref _estMajeur, value);
    }

    public string StatutPhrase
    {
        get => _statutPhrase;
        set => SetField(ref _statutPhrase, value);
    }

    // Fonctionnalite 4 : nombre de jours avant le prochain anniversaire
    public string AnniversaireTitre
    {
        get => _anniversaireTitre;
        set => SetField(ref _anniversaireTitre, value);
    }

    public string AnniversaireDetail
    {
        get => _anniversaireDetail;
        set => SetField(ref _anniversaireDetail, value);
    }

    // Fonctionnalite 1 : derniere sauvegarde
    public HistoriqueEntree? DernierResultat
    {
        get => _dernierResultat;
        set
        {
            if (SetField(ref _dernierResultat, value))
            {
                OnPropertyChanged(nameof(ADernierResultat));
                RestaurerDernierResultatCommand.Rafraichir();
            }
        }
    }

    public bool ADernierResultat => DernierResultat != null;

    // Fonctionnalite 5 : historique des calculs
    public ObservableCollection<HistoriqueEntree> Historique { get; }
        = new ObservableCollection<HistoriqueEntree>();

    public bool HistoriqueVide => Historique.Count == 0;

    // Liee a Button.Command dans le XAML.
    public RelayCommand CalculerCommand { get; }

    public RelayCommand ReinitialiserCommand { get; }

    public RelayCommand RestaurerDernierResultatCommand { get; }

    public RelayCommand EffacerHistoriqueCommand { get; }

    public CalculateurViewModel()
    {
        CalculerCommand = new RelayCommand(
            Calculer,
            () => !string.IsNullOrWhiteSpace(Nom) && !DateFuture);
        ReinitialiserCommand = new RelayCommand(Reinitialiser);
        RestaurerDernierResultatCommand = new RelayCommand(
            RestaurerDernierResultat,
            () => DernierResultat != null);
        EffacerHistoriqueCommand = new RelayCommand(
            EffacerHistorique,
            () => Historique.Count > 0);

        foreach (HistoriqueEntree entree in _stockage.ChargerHistorique())
            Historique.Add(entree);
        Historique.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HistoriqueVide));
            EffacerHistoriqueCommand.Rafraichir();
        };

        DernierResultat = _stockage.ChargerDernierResultat();
    }

    private void Calculer() => EnregistrerCalcul(AfficherResultat());

    // La logique metier : aucun controle d interface ici.
    private int AfficherResultat()
    {
        int age = DateTime.Today.Year
                - DateNaissance.Year;
        if (DateNaissance.Date >
            DateTime.Today.AddYears(-age)) age--;

        Resultat = $"{Nom}, vous avez {age} ans";
        ResultatVisible = true;

        // Fonctionnalite 3 : majeur si 18 ans ou plus, sinon mineur
        EstMajeur = age >= 18;
        StatutPhrase = EstMajeur ? "Vous êtes majeur(e)." : "Vous êtes mineur(e).";

        // Fonctionnalite 4 : jours restants avant l anniversaire, annees bissextiles comprises
        DateTime aujourdhui = DateTime.Today;
        DateTime prochain = AnniversaireEn(aujourdhui.Year);
        if (prochain < aujourdhui || DateNaissance.Date == aujourdhui)
            prochain = AnniversaireEn(aujourdhui.Year + 1);
        int jours = (prochain - aujourdhui).Days;
        string unite = jours > 1 ? "jours" : "jour";

        if (jours == 0)
        {
            AnniversaireTitre = "Joyeux anniversaire !";
            AnniversaireDetail = "Passez une excellente journée";
            _anniversaireResume = "Joyeux anniversaire !";
        }
        else
        {
            AnniversaireTitre = "Prochain anniversaire";
            AnniversaireDetail = $"Dans {jours} {unite}";
            _anniversaireResume = $"Prochain anniversaire dans {jours} {unite}";
        }

        return age;
    }

    private DateTime AnniversaireEn(int annee)
        => DateNaissance.Month == 2 && DateNaissance.Day == 29
           && !DateTime.IsLeapYear(annee)
            ? new DateTime(annee, 3, 1)
            : new DateTime(annee, DateNaissance.Month, DateNaissance.Day);

    // Fonctionnalites 1 et 5 : sauvegarder le dernier resultat et l ajouter a l historique
    private void EnregistrerCalcul(int age)
    {
        HistoriqueEntree entree = new HistoriqueEntree
        {
            Nom = Nom.Trim(),
            DateNaissance = DateNaissance.Date,
            Age = age,
            EstMajeur = EstMajeur,
            Anniversaire = _anniversaireResume,
            DateCalcul = DateTime.Now
        };

        Historique.Insert(0, entree);
        while (Historique.Count > TailleMaxHistorique)
            Historique.RemoveAt(Historique.Count - 1);

        _stockage.SauvegarderHistorique(Historique);
        _stockage.SauvegarderDernierResultat(entree);
        DernierResultat = entree;
    }

    // Fonctionnalite 1 : consulter la derniere sauvegarde
    private void RestaurerDernierResultat()
    {
        if (DernierResultat == null) return;
        Nom = DernierResultat.Nom;
        DateNaissance = DernierResultat.DateNaissance;
        AfficherResultat();
    }

    // Fonctionnalite 2 : remettre le formulaire a zero sans toucher a l historique ni au dernier resultat
    private void Reinitialiser()
    {
        Nom = "";
        DateNaissance = DateTime.Today.AddYears(-20);
        Resultat = "";
        ResultatVisible = false;
        EstMajeur = false;
        StatutPhrase = "";
        AnniversaireTitre = "";
        AnniversaireDetail = "";
        _anniversaireResume = "";
    }

    // Fonctionnalite 5 : vider l historique sans toucher au formulaire
    private void EffacerHistorique()
    {
        Historique.Clear();
        _stockage.EffacerHistorique();
    }
}
