using CalculateurAge.Views;

namespace CalculateurAge;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Declare la route : sans cette ligne, GoToAsync
        // leve une exception "route inconnue".
        Routing.RegisterRoute(nameof(ResultatPage),
                              typeof(ResultatPage));
    }
}
