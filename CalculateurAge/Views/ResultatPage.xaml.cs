namespace CalculateurAge.Views;

// Relie le parametre "nom" de l URL a la propriete Nom.
[QueryProperty(nameof(Nom), "nom")]
[QueryProperty(nameof(Age), "age")]
public partial class ResultatPage : ContentPage
{
    // Ces proprietes sont remplies par la navigation,
    // APRES le constructeur.
    public string Nom { get; set; }
    public string Age { get; set; }

    // Construit l arbre visuel decrit par le XAML.
    public ResultatPage() => InitializeComponent();

    // Appele a CHAQUE affichage de la page.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        lblMessage.Text = $"{Nom}, vous avez {Age} ans";
    }

    // ".." = revenir a la page precedente.
    private async void OnRetourClicked(object s, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
