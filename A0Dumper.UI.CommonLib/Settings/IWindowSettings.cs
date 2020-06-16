namespace A0Dumper.UI.CommonLib.Settings
{
    /// <summary>
    /// Представляет настройки окна приложения.
    /// </summary>
    public interface IWindowSettings
    {
        /// <summary>
        /// Получает или устанавливает имя пользователя.
        /// </summary>
        string Login { get; set; }
    }
}
