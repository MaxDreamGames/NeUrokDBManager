using System.Windows.Controls;
using NeUrokDBManager.Infrastructure;

namespace NeUrokDBManager.WPF
{
    public static class MyNavigationService
    {
        public static MainWindow? MainWindow;
        public static Frame? MainFrame;
        public static ApplicationDbContext? DbContext;

        public static void NavigateTo<T>() where T : Page, new()
        {
            if (MainFrame is not null)
                MainFrame.Navigate(new T());
        }
    }
}
