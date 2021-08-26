namespace A0Dumper.Data.A0Items.Repository
{
    using System.Collections.Generic;
    using A0Dumper.Data.A0Items.Entities;
    using A0Service;

    /// <summary>
    /// Предоставляет механизм получения данных обобщенных объектов А0.
    /// </summary>
    public interface IA0ItemRepo
    {
        /// <summary>
        /// Каталог обобщенных объектов А0.
        /// </summary>
        IA0EstimateRepo Repo { get; }

        /// <summary>
        /// Осуществляет чтение каталога обобщенных объектов А0.
        /// </summary>
        /// <param name="parent">Родительский узел.</param>
        /// <returns></returns>
        IList<IA0Item> Read(IA0Item parent);
    }
}