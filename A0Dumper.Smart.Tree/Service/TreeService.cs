namespace A0Dumper.Smart.Tree.Service
{
    using A0Dumper.Data.A0Items.Entities;
    using A0Dumper.Data.A0Items.Repository;
    using A0Dumper.Smart.Tree.Entities;

    /// <summary>
    /// Предоставляет механизм по формированию древовидной структуры.
    /// </summary>
    public class TreeService : ITreeService
    {
        /// <summary>
        /// Каталог обобщенных объектов A0.
        /// </summary>
        public IA0ItemRepo Repo { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="TreeService"./>
        /// </summary>
        /// <param name="repo">Объект обеспечивающий доступ к каталогам A0.</param>
        public TreeService(IA0ItemRepo repo)
        {
            this.Repo = repo;
        }

        /// <summary>
        /// Получает структуру-дерево с одним корневым узлом.
        /// </summary>
        /// <returns>Древовидная структура сметных объектов.</returns>
        public ITree GetTree()
        {
            // Создание корневого узла
            ITreeNode head = new TreeNode("Проекты", new A0Item(A0ItemKind.Head));
            return new Tree(head, this.Repo);
        }
    }
}