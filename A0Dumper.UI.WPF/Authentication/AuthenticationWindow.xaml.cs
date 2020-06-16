namespace A0Dumper.UI.WPF
{
    using System;
    using System.Windows;
    using A0Dumper.Data.A0Items.Service;
    using A0Dumper.UI.CommonLib.Settings;

    /// <summary>
    /// Логика взаимодействия для AuthenticationWidow.xaml.
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="AuthenticationWindow"./>
        /// </summary>
        /// /// <param name="settings">Настройки окна приложения.</param>
        public AuthenticationWindow(IWindowSettings settings)
        {
            this.InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Создание службы для подключения к API А0
            this.ConnectionService = new ConnectionService();

            // Создание настроек окна приложения
            this.Settings = settings;
        }

        /// <summary>
        /// Объект обеспечивающий соединение с БД А0.
        /// </summary>
        public ConnectionService ConnectionService { get; private set; }

        /// <summary>
        /// Получает настройки окна приложения.
        /// </summary>
        public IWindowSettings Settings { get; }

        /// <summary>
        /// Выполняет попытку соединения с БД A0 по введенным логину и паролю.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Settings.Login == null)
                {
                    this.Settings.Login = this.loginTextBox.Text;
                }

                this.ConnectionService.ConnectToA0(this.loginTextBox.Text, this.passwordBox.Password);
            }
            catch (ApplicationException)
            {
                MessageBox.Show("Проверьте корректность логина и пароля.", "Ошибка подключения к системе А0", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
                return;
            }
            this.DialogResult = true;
        }

        /// <summary>
        /// Сигнализирует о выходе пользователя из приложения.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Сохраняет последний введенный логин.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void AuthWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Settings.Login = this.loginTextBox.Text;
        }

        /// <summary>
        /// Отображает на форме последний введенный логин.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void AuthWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            this.passwordBox.Focus();
        }
    }
}