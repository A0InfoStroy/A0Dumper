namespace A0Dumper.UI.WinForms
{
    using A0Dumper.Data.A0Items.Service;
    using A0Dumper.UI.CommonLib.Settings;
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Обеспечивает логику взаимодействия с формой ввода логина и пароля.
    /// </summary>
    public partial class AuthenticationForm : Form
    {
        /// <summary>
        /// Объект обеспечивающий соединение с БД А0
        /// </summary>
        public ConnectionService ConnectionService { get; private set; }

        /// <summary>
        /// Получает настройки окна приложения.
        /// </summary>
        public IWindowSettings Settings { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <seealso cref="AuthenticationForm"./>
        /// </summary>
        public AuthenticationForm(IWindowSettings settings)
        {
            InitializeComponent();
            // Создание службы для подключения к API А0
            this.ConnectionService = new ConnectionService();
            // Создание настроек окна приложения
            this.Settings = settings;
            this.CenterToScreen();
        }

        /// <summary>
        /// Записывает последний введенный логин в реестр.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Settings.Login = this.loginTextBox.Text;
        }

        /// <summary>
        /// Загружает из реестра сохраненный логин и передает его в поле ввода формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticationForm_Load(object sender, EventArgs e)
        {
            this.loginTextBox.Text = this.Settings.Login;
            this.loginTextBox.SelectAll();         
        }

        /// <summary>
        /// Выполняет попытку соединения с БД A0 по введенным логину и паролю.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, EventArgs e)
        {         
            try
            {
                if (this.Settings.Login == null)
                {
                    this.Settings.Login = this.loginTextBox.Text;
                }
                this.ConnectionService.ConnectToA0(this.loginTextBox.Text, this.passwordBox.Text);
            }
            catch (ApplicationException)
            {
                MessageBox.Show("Проверьте корректность логина и пароля.", "Ошибка подключения к системе А0", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = DialogResult.None;
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Сигнализирует о выходе пользователя из приложения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }        
    }
}