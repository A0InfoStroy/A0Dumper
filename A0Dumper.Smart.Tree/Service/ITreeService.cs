namespace A0Dumper.Smart.Tree.Service
{
    using A0Dumper.Data.A0Items.Repository;
    using A0Dumper.Smart.Tree.Entities;

    /// <summary>
    /// Предоставляет механизм по формированию древовидной структуры.
    /// </summary>
    public interface ITreeService
    {
        /// <summary>
        /// Каталог обобщенных объектов A0.
        /// </summary>
        IA0ItemRepo Repo { get; }

        /// <summary>
        /// Получает структуру-дерево.
        /// </summary>
        /// <returns>Древовидная структура сметных объектов.</returns>
        ITree GetTree();
    }
}