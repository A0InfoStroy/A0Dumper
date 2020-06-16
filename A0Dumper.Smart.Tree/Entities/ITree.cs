namespace A0Dumper.Smart.Tree.Entities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using A0Dumper.Data.A0Items.Entities;

    /// <summary>
    /// Описывает свойства структуры-дерева.
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// Получает корневой узел дерева.
        /// </summary>
        ITreeNode Head { get; }

        /// <summary>
        /// Получает или устанавливает критерий сортировки узлов дерева.
        /// </summary>
        SortCriterion SortKey { get; set; }

        /// <summary>
        /// Получает или устанавливает признак сортировки в обратном порядке.
        /// </summary>
        bool Descending { get; set; }

        /// <summary>
        /// Заполняет список дочерних узлов родительского узла.
        /// </summary>
        /// <param name="parent">Родительский узел.</param>
        /// <returns>Операция выполняемая в отдельном потоке приложения.</returns>
        Task GetSubNodesAsync(ITreeNode parent);
    }

    /// <summary>
    /// Описывает свойства узла в древовидной структуре.
    /// </summary>
    public interface ITreeNode
    {
        /// <summary>
        /// Получает отображаемый текст узла.
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// Получает или устанавливает список дочерних узлов, входящих в состав родительского узла.
        /// </summary>
        IList<ITreeNode> SubNodes { get; set; }

        /// <summary>
        /// Получает или устанавливает признак заполненности списка дочерних узлов.
        /// </summary>
        bool IsFilled { get; set; }

        /// <summary>
        /// Получает ссылку на абстракцию объекта А0, представляемую узлом.
        /// </summary>
        IA0Item Source { get; }

        /// <summary>
        /// Получает тип узла.
        /// </summary>
        TreeNodeKind Kind { get; }
    }

    /// <summary>
    /// Типы узлов в дереве.
    /// </summary>
    public enum TreeNodeKind
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Комплекс
        /// </summary>
        Complex = 1,

        /// <summary>
        /// Проект
        /// </summary>
        Project = 2,

        /// <summary>
        /// Объектная смета
        /// </summary>
        OS = 3,

        /// <summary>
        /// Локальная смета
        /// </summary>
        LS = 4,

        /// <summary>
        /// Акт
        /// </summary>
        Act = 5,

        /// <summary>
        /// Корневой узел
        /// </summary>
        Head = 6,

        /// <summary>
        /// Раздел или глава
        /// </summary>
        Section = 7,
    }

    /// <summary>
    /// Критерии сортировки узлов дерева.
    /// </summary>
    public enum SortCriterion
    {
        /// <summary>
        /// Критерий сортировки не задан
        /// </summary>
        None = 0,

        /// <summary>
        /// Сортировка по наименованию сметного объекта
        /// </summary>
        Name = 1,

        /// <summary>
        /// Сортировка по шифру сметного объекта
        /// </summary>
        Mark = 2,

        /// <summary>
        /// Сортировка по дате создания сметного объекта
        /// </summary>
        Date = 3,
    }
}