namespace A0Dumper.Data.A0Items.Service
{
    using System;
    using System.Runtime.InteropServices;
    using A0Service;

    /// <summary>
    /// Обеспечивает соединение с базой данных через API A0.
    /// </summary>
    public class ConnectionService : IDisposable
    {
        /// <summary>
        /// COM-объект API A0.
        /// </summary>
        private API a0;

        /// <summary>
        /// Истинно, если неуправляемые ресурсы были освобождены; иначе ложно.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Получает каталог обобщенных объектов А0.
        /// </summary>
        public IA0EstimateRepo Repo => this.a0?.Estimate.Repo;

        /// <summary>
        /// Устанавливает соединение с БД А0.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        public void ConnectToA0(string login, string password)
        {
            if (this.a0 == null)
            {
                this.a0 = new API();
            }

            // Инициализация параметров для установки соединения
            ConnectionSettings cs = new ConnectionSettings();

            // Установка соединения с БД A0
            EConnectReturnCode returnCode = this.a0.Connect3(cs.ConnStr, login, password);
            if (returnCode != EConnectReturnCode.crcSuccess)
            {
                throw new ApplicationException($"Не удалось установить соединение с БД А0. Код возврата {returnCode}");
            }
        }

        /// <summary>
        /// Выполняет освобождение неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Подавление финализации
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Выполняет освобождение неуправляемых ресурсов.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (this.a0 != null)
                {
                    if (disposing)
                    {
                        this.a0.Disconnect();
                    }

                    Marshal.ReleaseComObject(this.a0);
                    this.a0 = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Выполняет освобождение неуправляемых ресурсов перед удалением объекта из памяти.
        /// </summary>
        ~ConnectionService()
        {
            this.Dispose(false);
        }
    }

    /// <summary>
    /// Представляет настройки соединения с базой данных.
    /// </summary>
    internal class ConnectionSettings
    {
        /// <summary>
        /// Получает OLEDB строку соединения.
        /// </summary>
        internal string ConnStr => Properties.Settings.Default.ConnStr;

        /// <summary>
        /// Получает имя пользователя в системе А0.
        /// </summary>
        internal string UserName => Properties.Settings.Default.UserName;

        /// <summary>
        /// Получает пароль пользователя в системе А0.
        /// </summary>
        internal string Password => Properties.Settings.Default.Password;
    }
}