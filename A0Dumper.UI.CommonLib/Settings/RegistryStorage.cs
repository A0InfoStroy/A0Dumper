namespace A0Dumper.UI.CommonLib.Settings
{
    using Microsoft.Win32;

    /// <summary>
    /// Обеспечивает хранение имени пользователя в реестре.
    /// </summary>
    public class RegistryStorage : IWindowSettings
    {
        /// <summary>
        /// Получает или устанавливает имя пользователя.
        /// </summary>
        public string Login { get => this.LoadFromRegistry(); set => this.SaveInRegistry(value); }

        /// <summary>
        /// Записывает имя пользователя в реестр.
        /// </summary>
        /// <param name="login">Имя пользователя.</param>
        private void SaveInRegistry(string login)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\A0DumperLogin");
            key.SetValue("Login", login);
            key.Close();
        }

        /// <summary>
        /// Загружает из реестра сохраненное имя пользователя.
        /// </summary>
        /// <returns>Имя пользователя.</returns>
        private string LoadFromRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\A0DumperLogin");
            if (key != null)
            {
                return key.GetValue("Login").ToString();
            }

            return null;
        }
    }
}