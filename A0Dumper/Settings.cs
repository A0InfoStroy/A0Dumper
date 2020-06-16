// $Date: 2020-03-20 14:04:17 +0300 (Пт, 20 мар 2020) $
// $Revision: 51 $
// $Author: agalkin $


namespace A0Dumper
{
    /// <summary>
    /// Настройки соединения
    /// </summary>
    class ConnectionSettings
    {
        /// <summary>
        /// OLEDB строка соединения
        /// </summary>
        public string ConnStr { get { return Properties.Settings.Default.ConnStr; } }
        /// <summary>
        /// Имя пользователя в системе А0
        /// </summary>
        public string UserName { get { return Properties.Settings.Default.UserName; } }
        /// <summary>
        /// Пароль пользователя в системе А0
        /// </summary>
        public string Password { get { return Properties.Settings.Default.Password; } }
    }
}
