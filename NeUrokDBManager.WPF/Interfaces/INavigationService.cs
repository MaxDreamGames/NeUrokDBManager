using System.Windows.Controls;

namespace NeUrokDBManager.WPF.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : Page;
    }
}
