namespace A0Dumper.UI.CommonLib.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using A0Dumper.Smart.Tree.Entities;
    using A0Dumper.UI.CommonLib.Entities;

    /// <summary>
    /// Предоставляет спомогательные методы для работы с интерфейсом.
    /// </summary>
    public static class UIService
    {
        /// <summary>
        /// Получает критерий сортировки.
        /// </summary>
        /// <param name="buttonName">Надпись на кнопке выбора сортировки.</param>
        /// <returns>Критерий сортировки.</returns>
        public static SortCriterion GetSortCriterion(string buttonName)
        {
            switch (buttonName)
            {
                case "по имени":
                    return SortCriterion.Name;
                case "по шифру":
                    return SortCriterion.Mark;
                case "по дате":
                    return SortCriterion.Date;
                default:
                    return SortCriterion.None;
            }
        }

        /// <summary>
        /// Производит чтение содержимого глав выбранного узла.
        /// </summary>
        /// <param name="selectedNode">Выбранный узел.</param>
        /// <param name="rows">Список строк таблицы дочерних узлов.</param>
        /// <param name="tree">Дерево.</param>
        /// <returns>Операция выполняемая в отдельном потоке приложения.</returns>
        public static async Task ReadChapter(ITreeNode selectedNode, IList<TreeNodeRow> rows, ITree tree)
        {
            foreach (ITreeNode chapter in selectedNode.SubNodes)
            {
                // Запрос на получение дочерних узлов для нераскрытих ранее узлов
                if (!chapter.IsFilled)
                {
                    await tree.GetSubNodesAsync(chapter);
                }

                // Чтение всех разделов входящих в главы
                if (chapter.SubNodes.FirstOrDefault()?.Kind == TreeNodeKind.Section)
                {
                    await ReadChapter(chapter, rows, tree);
                }

                // Сохранение в список сметных объектов расположенных в главах или разделах
                else
                {
                    foreach (var subNode in chapter.SubNodes)
                    {
                        rows?.Add(new TreeNodeRow(subNode));
                    }
                }
            }
        }
    }
}