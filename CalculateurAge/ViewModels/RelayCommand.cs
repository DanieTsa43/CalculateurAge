using System.Windows.Input;

namespace CalculateurAge.ViewModels;

// Transforme une methode en objet liable a un Button.
public class RelayCommand : ICommand
{
    private readonly Action _executer;          // quoi faire
    private readonly Func<bool> _peutExecuter;  // si possible

    public RelayCommand(Action executer,
                        Func<bool> peutExecuter = null)
    {
        _executer = executer;
        _peutExecuter = peutExecuter;
    }

    // Le Button appelle ceci et se grise si false.
    public bool CanExecute(object p)
        => _peutExecuter?.Invoke() ?? true;

    // Execute l action au clic.
    public void Execute(object p) => _executer();

    public event EventHandler CanExecuteChanged;

    // A appeler pour forcer le bouton a reposer la question.
    public void Rafraichir()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
