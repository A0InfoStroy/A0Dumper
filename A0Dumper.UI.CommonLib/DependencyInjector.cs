namespace A0Dumper.UI.CommonLib
{
    using A0Dumper.Data.A0Items.Repository;
    using A0Dumper.Smart.Tree.Service;
    using A0Dumper.UI.CommonLib.Settings;
    using A0Service;
    using Ninject;

    /// <summary>
    /// Предоставляет методы для регистрации зависимостей.
    /// </summary>
    public class DependencyInjector
    {
        /// <summary>
        /// Создает реализацию интерфейса IWindowSettings.
        /// </summary>
        /// <returns>Экзепмляр класса <seealso cref="RegistryStorage".</returns>
        public IWindowSettings CreateWindowSettings()
        {
            using (IKernel ninjectKernel = new StandardKernel())
            {
                ninjectKernel.Bind<IWindowSettings>().To<RegistryStorage>();
                return ninjectKernel.Get<IWindowSettings>();
            }
        }

        /// <summary>
        /// Создает реализацию интерфейса ITreeService.
        /// </summary>
        /// <param name="argName">Имя аргумента.</param>
        /// <param name="repo">Каталог обобщенных объектов A0.</param>
        /// <returns>Экзепмляр класса <seealso cref="TreeService"</returns>
        public ITreeService CreateTreeService(string argName, IA0EstimateRepo repo)
        {
            using (IKernel ninjectKernel = new StandardKernel())
            {
                ninjectKernel.Bind<IA0ItemRepo>().To<A0ItemRepo>().WithConstructorArgument(argName, repo);
                ninjectKernel.Bind<ITreeService>().To<TreeService>();
                return ninjectKernel.Get<ITreeService>();
            }
        }
    }
}