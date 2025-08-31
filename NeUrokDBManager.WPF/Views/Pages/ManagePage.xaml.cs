using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using NeUrokDBManager.Core.DTOs;
using NeUrokDBManager.Core.Entities;
using NeUrokDBManager.Core.Interfaces.Reposoitories;
using NeUrokDBManager.Infrastructure.Reposoitories;

namespace NeUrokDBManager.WPF.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ManagePage.xaml
    /// </summary>
    public partial class ManagePage : Page
    {

        private SolidColorBrush? _currentMarkerColor;
        private readonly IClientRepository? _clientRepository;
        private ObservableCollection<ClientDTO> _clients = new ObservableCollection<ClientDTO>();
        private Dictionary<int, System.Drawing.Color> _colors = new();

        public ManagePage()
        {
            InitializeComponent();
            if (MyNavigationService.DbContext != null)
                _clientRepository = new ClientRepository(MyNavigationService.DbContext);

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RegistrationDate.DisplayDate = DateTime.Now;
            RegistrationDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
            await SelectAll();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            await SelectAll();
        }

        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            await Search();
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_clientRepository is null) return;
            if (CheckFieldOnEmptiness(DeleteIdText) && CheckOnDigitality(DeleteIdText))
            {
                await _clientRepository.DeleteByUserIdAsync(int.Parse(DeleteIdText.Text));
                await SelectAll();
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MinMaxBtn_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void SetCurrentColor(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                var color = border.Background as SolidColorBrush;
                if (color != null)
                {
                    ResetMarkers();
                    _currentMarkerColor = color;
                    border.BorderBrush = new SolidColorBrush(Colors.Aqua);
                }
            }
        }

        private void NoneColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border bd)
            {
                ResetMarkers();
                _currentMarkerColor = null;
                bd.BorderBrush = new SolidColorBrush(Colors.Aqua);
            }

        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            await Add();
        }

        private async void DataGr_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (_currentMarkerColor is null) return;

            foreach (var cell in DataGr.SelectedCells)
            {
                var row = (DataGridRow)DataGr.ItemContainerGenerator.ContainerFromItem(cell.Item);
                if (row != null && !row.IsNewItem)
                {
                    row.Background = _currentMarkerColor;

                    var item = cell.Item;
                    if (_currentMarkerColor is null) return;
                    if (item is ClientDTO client && _clientRepository is not null)
                    {
                        System.Windows.Media.Color mediaColor = _currentMarkerColor.Color;

                        // Конвертируем в System.Drawing.Color
                        System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(
                            mediaColor.A,
                            mediaColor.R,
                            mediaColor.G,
                            mediaColor.B);
                        var col = new ClientColor { Color = drawingColor };
                        await _clientRepository.ChangeColorAsync(client.UserId, col);
                    }
                }
            }
        }

        private async void DataGr_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Получаем измененный объект ClientDTO
                var editedItem = e.Row.Item as ClientDTO;
                if (editedItem == null) return;

                // Получаем имя измененного свойства (столбца)
                var column = e.Column as DataGridBoundColumn;
                if (column == null) return;

                var bindingPath = (column.Binding as Binding)?.Path.Path;
                if (string.IsNullOrEmpty(bindingPath)) return;

                // Получаем новое значение из редактора ячейки
                var element = e.EditingElement as TextBox; // Для TextBox
                object newValue = element?.Text ?? new object();

                if (element == null)
                {
                    // Обработка других типов редакторов (CheckBox, DatePicker и т.д.)
                    if (e.EditingElement is DatePicker datePicker)
                    {
                        if (datePicker.SelectedDate != null)
                            newValue = datePicker.SelectedDate;
                    }
                    // Добавьте другие типы редакторов при необходимости
                }

                try
                {
                    // Преобразуем значение к нужному типу
                    var propertyType = typeof(ClientDTO).GetProperty(bindingPath)?.PropertyType;
                    if (propertyType != null && newValue != null)
                    {
                        if (propertyType == typeof(int?) && newValue is string str)
                        {
                            if (int.TryParse(str, out int intValue))
                                newValue = intValue;
                            else
                                newValue = null;
                        }
                        // Добавьте другие преобразования типов при необходимости
                    }

                    // Вызываем метод репозитория для обновления
                    if (_clientRepository is not null && newValue is not null)
                    {
                        await _clientRepository.UpdateAsync(
                            editedItem.UserId,
                            GetClientPropertySelector(bindingPath),
                            newValue
                        );
                    }

                    // Обновляем интерфейс (если нужно)
                    await SelectAll();
                    e.Cancel = true;
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    MessageBox.Show($"Ошибка при обновлении: {ex.Message}");
                    // Откатываем изменения в DataGrid
                    e.Cancel = true;
                }
            }
        }

        // Сопоставление свойств DTO с сущностью Client
        private Expression<Func<Client, object>> GetClientPropertySelector(string dtoPropertyName)
        {
            return dtoPropertyName switch
            {
                nameof(ClientDTO.StudentName) => c => c.StudentName,
                nameof(ClientDTO.Birthday) => c => c.Birthday,
                nameof(ClientDTO.RegistrationDate) => c => c.RegistrationDate,
                nameof(ClientDTO.Class) => c => c.Class,
                nameof(ClientDTO.Courses) => c => c.Courses,
                nameof(ClientDTO.ParentName) => c => c.ParentName,
                nameof(ClientDTO.PhoneNumber) => c => c.PhoneNumber,
                nameof(ClientDTO.AnotherPhoneNumber) => c => c.AnotherPhoneNumber,
                nameof(ClientDTO.Comments) => c => c.Comments,
                _ => throw new ArgumentException($"Неизвестное свойство: {dtoPropertyName}")
            };
        }

        private void ResetMarkers()
        {
            foreach (var marker in ColorMarkers.Children)
            {
                if (marker is Border border)
                    border.BorderBrush = new SolidColorBrush(Colors.Black);
            }
        }

        private bool CheckFields()
        {
            var checkings = new List<bool>
            {
                CheckFieldOnEmptiness(StudentFioText),
                CheckFieldOnEmptiness(FormText),
                CheckFieldOnEmptiness(ParentFioText),
                CheckFieldOnEmptiness(PhoneNumberText),
                CheckOnDigitality(FormText)
            };
            return checkings.All(c => c == true);
        }

        private bool CheckFieldOnEmptiness(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
            else
            {
                textBox.BorderBrush = new SolidColorBrush(Colors.Black);
                return true;
            }
        }

        private bool CheckOnDigitality(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text)) return true;
            if (int.TryParse(textBox.Text, out _))
            {
                textBox.BorderBrush = new SolidColorBrush(Colors.Black);
                return true;
            }
            else
            {
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
        }

        private bool CheckBirthdayDateSelected()
        {
            if (BirthdayDate.SelectedDate is not null)
            {
                BirthdayDate.BorderBrush = new SolidColorBrush(Colors.Transparent);
                return true;
            }
            else
            {
                BirthdayDate.BorderBrush = new SolidColorBrush(Colors.Red);
                return false;
            }
        }

        private async Task SelectAll()
        {
            if (_clientRepository != null)
            {
                var clients = await _clientRepository.GetAllAsync();
                _colors = await _clientRepository.GetColorsAsync();
                if (clients != null)
                {
                    var cls = clients.Select(s => new ClientDTO
                    {
                        UserId = s.UserId,
                        StudentName = s.StudentName,
                        Birthday = s.Birthday,
                        RegistrationDate = s.RegistrationDate,
                        AnotherPhoneNumber = s.AnotherPhoneNumber,
                        Class = s.Class,
                        Comments = s.Comments,
                        Courses = s.Courses,
                        ParentName = s.ParentName,
                        PhoneNumber = s.PhoneNumber
                    }).OrderBy(c => c.UserId).ToList();

                    _clients = new ObservableCollection<ClientDTO>(cls);
                    DataGr.ItemsSource = _clients;
                    SelectedCount.Text = $"Выведено: {DataGr.Items.Count}";
                    NextIdText.Text = NextIdText.Text = $"{DataGr.Items.Count + 1}";
                    //SetColors();
                }
            }
        }

        private async Task Add()
        {
            if (CheckFields())
            {
                AddBtn.Background.Opacity = 1f;
                if (_clientRepository is null) return;
                int newId = await _clientRepository.GetNextUserIdAsync();

                int.TryParse(FormText.Text, out int result);

                try
                {
                    Client newClient = new Client(newId,
                        StudentFioText.Text,
                        BirthdayDate.SelectedDate,
                        RegistrationDate.SelectedDate,
                        result,
                        CoursesText.Text,
                        ParentFioText.Text,
                        PhoneNumberText.Text,
                        AnotherPhoneNumberText.Text,
                        ComentsText.Text);

                    await _clientRepository.AddAsync(newClient);
                    await SelectAll();
                }
                catch { }
            }
            else
            {
                AddBtn.Background.Opacity = 0.5f;
            }
        }

        private async Task Search()
        {
            if (_clientRepository is null) return;
            _colors = await _clientRepository.GetColorsAsync();

            ClientSearchDTO searchDTO = new ClientSearchDTO
            {
                UserId = int.TryParse(IdText.Text, out int id) ? id : 0,
                StudentName = StudentFioText.Text,
                BirthdayDay = int.TryParse(BirthdayDateDay.Text, out int bDay) ? bDay : 0,
                BirthdayMonth = int.TryParse(BirthdayDateMonth.Text, out int bMonth) ? bMonth : 0,
                BirthdayYear = int.TryParse(BirthdayDateYear.Text, out int bYear) ? bYear : 0,
                RegistrationDateDay = int.TryParse(RegistrationDateDay.Text, out int rdDay) ? rdDay : 0,
                RegistrationDateMonth = int.TryParse(RegistrationDateMonth.Text, out int rdMonth) ? rdMonth : 0,
                RegistrationDateYear = int.TryParse(RegistrationDateYear.Text, out int rdYear) ? rdYear : 0,
                Class = int.TryParse(FormText.Text, out int form) ? form : null,
                ParentName = ParentFioText.Text,
                PhoneNumber = PhoneNumberText.Text,
                AnotherPhoneNumber = AnotherPhoneNumberText.Text,
                Courses = CoursesText.Text,
                Comments = ComentsText.Text
            };

            var result = await _clientRepository.SearchAsync(searchDTO);
            if (result is not null)
            {
                var cls = result.Select(s => new ClientDTO
                {
                    UserId = s.UserId,
                    StudentName = s.StudentName,
                    Birthday = s.Birthday,
                    RegistrationDate = s.RegistrationDate,
                    AnotherPhoneNumber = s.AnotherPhoneNumber,
                    Class = s.Class,
                    Comments = s.Comments,
                    Courses = s.Courses,
                    ParentName = s.ParentName,
                    PhoneNumber = s.PhoneNumber
                }).OrderBy(c => c.UserId).ToList();
                _clients = new ObservableCollection<ClientDTO>(cls);
                DataGr.ItemsSource = _clients;
                SelectedCount.Text = $"Выведено: {DataGr.Items.Count}";
                NextIdText.Text = $"{DataGr.Items.Count + 1}";
                SetColors();
            }
        }

        private void Clear()
        {
            IdText.Text = string.Empty;
            StudentFioText.Text = string.Empty;
            ParentFioText.Text = string.Empty;
            BirthdayDateDay.Text = string.Empty;
            BirthdayDateMonth.Text = string.Empty;
            BirthdayDateYear.Text = string.Empty;
            PhoneNumberText.Text = string.Empty;
            AnotherPhoneNumberText.Text = string.Empty;
            FormText.Text = string.Empty;
            RegistrationDateDay.Text = string.Empty;
            RegistrationDateMonth.Text = string.Empty;
            RegistrationDateYear.Text = string.Empty;
            CoursesText.Text = string.Empty;
            ComentsText.Text = string.Empty;
            RegistrationDate.SelectedDate = DateTime.Now;
            RegistrationDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
            BirthdayDate.SelectedDate = null;

        }

        private async void SetColors()
        {
            if (_clientRepository == null) { return; }
            var colors = await _clientRepository.GetColorsAsync();

            foreach (var color in colors)
            {
                var rowData = DataGr.ItemsSource
                    .Cast<ClientDTO>()
                    .FirstOrDefault(item => item.UserId == color.Key);

                if (rowData != null) // not null
                {
                    var row = (DataGridRow)DataGr.ItemContainerGenerator.ContainerFromItem(rowData);

                    if (row != null) // error: null
                    {
                        row.Background = ToMediaBrush(color.Value);
                    }
                }

            }
        }

        public static SolidColorBrush ToMediaBrush(System.Drawing.Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B));
        }

        private void DataGr_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row != null)
            {
                if (e.Row.Item is ClientDTO client &&
                    _colors.TryGetValue(client.UserId, out var c))
                {
                    SolidColorBrush brush = ToMediaBrush(c);
                    if (brush.CanFreeze) brush.Freeze();
                    e.Row.Background = brush;
                }
            }
        }
    }
}
