namespace A0Dumper.UI.CommonLib.Entities
{
    using System.Globalization;
    using A0Dumper.Data.A0Items.Entities;
    using A0Dumper.Smart.Tree.Entities;

    /// <summary>
    /// Представляет строку в таблице, отображающую параметры узла дерева.
    /// </summary>
    public class TreeNodeRow
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="TreeNodeRow"./>
        /// </summary>
        /// <param name="treeNode">Узел дерева для строкового представления.</param>
        public TreeNodeRow(ITreeNode treeNode)
        {
            this.Name = treeNode.Caption;
            if (treeNode.Source is A0Item item)
            {
                this.Mark = item.Mark;
                this.DayMonth = item.CreationDate.ToString("dd MMMM", CultureInfo.CurrentCulture);
                this.Year = item.CreationDate.Year;
            }
        }

        /// <summary>
        /// Получает шифр сметного объекта.
        /// </summary>
        public string Mark { get; }

        /// <summary>
        /// Получает наименование сметного объекта.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Получает день и месяц создания сметного объекта.
        /// </summary>
        public string DayMonth { get; }

        /// <summary>
        /// Получает год создания сметног объекта.
        /// </summary>
        public int Year { get; }
    }
}