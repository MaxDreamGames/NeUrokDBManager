using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NeUrokDBManager.Core.Interfaces.Reposoitories;
using NeUrokDBManager.Infrastructure.Reposoitories;
using Notification.Wpf;


namespace NeUrokDBManager.WPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private readonly IClientRepository? _clientRepository;
        public static bool isCreated = false;


        public MenuPage()
        {
            InitializeComponent();

            if (MyNavigationService.DbContext != null)
            {
                _clientRepository = new ClientRepository(MyNavigationService.DbContext);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyNavigationService.NavigateTo<ManagePage>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_clientRepository is null) return;
            if (!isCreated)
                ChangeWindowState(WindowState.Minimized);
            if (DateTime.Now.Month == 9 && DateTime.Now.Day == 1)
            {
                bool isUpdated = await _clientRepository.UpdateClassesAsync();
                if (isUpdated)
                    ShowToastNotification("1 сентября", "Классы учеников были успешно увеличены\nХорошего учебного года!");
            }

            SetBirthdays();
        }

        private void SetBirthdays()
        {
            if (_clientRepository is null) return;
            var birthdays = _clientRepository.GetClosestBirthdays();
            if (birthdays is null) return;

            BirthdaysTodayContainer.Text = string.Empty;
            BirthdaysTomorrowContainer.Text = string.Empty;
            BirthdaysAfterTomorrowContainer.Text = string.Empty;
            foreach (var birthday in birthdays)
            {
                string str = $"{birthday.StudentName} - {birthday.age}\n";
                if (birthday.dayLeft == 0)
                    BirthdaysTodayContainer.Text += str;
                else if (birthday.dayLeft == 1)
                    BirthdaysTomorrowContainer.Text += str;
                else
                    BirthdaysAfterTomorrowContainer.Text += str;
            }
            if (!isCreated)
            {
                if (!string.IsNullOrEmpty(BirthdaysTodayContainer.Text))
                    ShowToastNotification("Сегодня день рождения у:", BirthdaysTodayContainer.Text);
                isCreated = true;
            }
        }

        private void ChangeWindowState(WindowState windowState)
        {
            if (MyNavigationService.MainWindow == null) return;
            MyNavigationService.MainWindow.WindowState = windowState;
        }

        public void ShowToastNotification(string title, string message)
        {
            var im = new BitmapImage(new Uri("pack://application:,,,/ExternalResources/Images/Icons/Logo.png"));
            var content = new NotificationContent
            {
                Title = title,
                Message = message,
                Type = NotificationType.Notification,
                TrimType = NotificationTextTrimType.AttachIfMoreRows,
                RowsCount = 10,
                LeftButtonAction = () => ChangeWindowState(WindowState.Normal),
                LeftButtonContent = "Ок",
                Icon = im
            };

            var notificatioManager = new NotificationManager();
            SystemSounds.Beep.Play();
            notificatioManager.Show(content, expirationTime: TimeSpan.FromSeconds(10));
        }
    }
}
