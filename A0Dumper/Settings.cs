// $Date: 2019-10-15 14:18:33 +0300 (Вт, 15 окт 2019) $
// $Revision: 7 $
// $Author: vbutov $


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
