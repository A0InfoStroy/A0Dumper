namespace A0Dumper.Smart.Tree.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using A0Dumper.Data.A0Items.Entities;
    using A0Dumper.Data.A0Items.Repository;

    /// <summary>
    /// Представляет структуру-дерево.
    /// </summary>
    public class Tree : ITree
    {
        /// <summary>
        /// Каталог обобщенных объектов A0.
        /// </summary>
        private readonly IA0ItemRepo repo;

        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="Tree"./>
        /// </summary>
        /// <param name="head">Корневой узел.</param>
        /// <param name="repo">Каталог обобщенных объектов A0.</param>
        public Tree(ITreeNode head, IA0ItemRepo repo)
        {
            this.Head = head;
            this.repo = repo;
        }

        /// <summary>
        /// Получает корневой узел дерева.
        /// </summary>
        public ITreeNode Head { get; }

        /// <summary>
        /// Получает или устанавливает критерий сортировки узлов дерева.
        /// </summary>
        public SortCriterion SortKey { get; set; }

        /// <summary>
        /// Получает или устанавливает признак сортировки в обратном порядке.
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// Заполняет список дочерних узлов родительского узла.
        /// </summary>
        /// <param name="parent">Родительский узел.</param>
        /// <returns>Операция выполняемая в отдельном потоке приложения.</returns>
        public async Task GetSubNodesAsync(ITreeNode parent)
        {
            // Получение списка дочерних узлов в отдельном потоке приложения
            IList<IA0Item> subNodes = await Task.Run(() => this.repo.Read(parent.Source));

            // Получение первого элемента списка с целью определения типа дочерних узлов
            IA0Item first = subNodes.FirstOrDefault();

            // Заполнение родительского узла главами
            if (first is A0Section)
            {
                foreach (IA0Item subNode in subNodes)
                {
                    // Создание дочернего узла дерева
                    TreeNode node = new TreeNode(subNode);

                    // Заполнение списка родительского узла
                    parent.SubNodes.Add(node);

                    // Заполнение глав разделами
                    this.FillInnerStructure((A0Section)subNode, node);
                }
            }

            // Заполнение родительского узла сметными объектами
            else
            {
                switch (this.SortKey)
                {
                    case SortCriterion.None:
                        // Загрузка узлов без сортировки
                        subNodes.Select(x => new TreeNode(x)).ToList<ITreeNode>().ForEach(x => parent.SubNodes.Add(x));
                        break;
                    case SortCriterion.Name:
                        // Загрузка узлов отсортированных по наименованию
                        this.GetSortedNodes(subNodes, x => x.Name).ForEach(x => parent.SubNodes.Add(x));
                        break;
                    case SortCriterion.Mark:
                        // Загрузка узлов отсортированных по шифру сметного объекта
                        this.GetSortedNodes(subNodes, x => x.Mark).ForEach(x => parent.SubNodes.Add(x));
                        break;
                    case SortCriterion.Date:
                        // Загрузка узлов отсортированных по дате создания сметного объекта
                        this.GetSortedNodes(subNodes, x => x.CreationDate).ForEach(x => parent.SubNodes.Add(x));
                        break;
                }
            }

            // Переключение флага сигнализирующее о получении родительским узлом дочерних
            parent.IsFilled = true;
        }

        /// <summary>
        /// Получает список отсортированных узлов дерева.
        /// </summary>
        /// <param name="subNodes">Список сметных объектов А0, преобразуемых в узлы дерева.</param>
        /// <param name="sortKey">Функция определяющая ключ сортировки.</param>
        /// <returns>Список узлов дерева.</returns>
        private List<ITreeNode> GetSortedNodes(IList<IA0Item> subNodes, Func<A0Item, IComparable> sortKey)
        {
            return subNodes.Cast<A0Item>()
                            .SortNodes(sortKey, this.Descending)
                            .Select(x => new TreeNode(x))
                            .ToList<ITreeNode>();
        }

        /// <summary>
        /// Заполняет главу родительского узла вложенными разделами.
        /// </summary>
        /// <param name="chapter">Глава сметного объекта А0.</param>
        /// <param name="parent">Узел дерева отображающий главу.</param>
        private void FillInnerStructure(A0Section chapter, ITreeNode parent)
        {
            if (chapter.SubSections.Count == 0)
            {
                return;
            }

            foreach (var subSection in chapter.SubSections)
            {
                TreeNode child = new TreeNode(subSection);
                parent.SubNodes.Add(child);

                // Рекурсивный вызов для заполнения следующих уровней вложенности
                this.FillInnerStructure(subSection, child);
            }

            // Переключение флага сигнализирующее о получении главой вложенных разделов
            parent.IsFilled = true;
        }
    }

    /// <summary>
    /// Содержит вспомогательные методы расширения для построения дерева.
    /// </summary>
    public static class TreeExtensions
    {
        /// <summary>
        /// Сортирует коллекцию сметных объектов.
        /// </summary>
        /// <param name="a0Items">Исходная коллекция сметных объектов.</param>
        /// <param name="sortKey">Функция определяющая ключ сортировки.</param>
        /// <param name="descending">Признак сортировки по убыванию.</param>
        /// <returns>Коллекция сметных объектов.</returns>
        public static IEnumerable<A0Item> SortNodes(this IEnumerable<A0Item> a0Items, Func<A0Item, IComparable> sortKey, bool descending)
        {
            if (descending)
            {
                return a0Items.OrderByDescending(sortKey);
            }

            return a0Items.OrderBy(sortKey);
        }
    }
}