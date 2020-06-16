namespace A0Dumper.Smart.Tree.Entities
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using A0Dumper.Data.A0Items.Entities;

    /// <summary>
    /// Представляет сметный объект в древовидной структуре А0.
    /// </summary>
    public class TreeNode : ITreeNode
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="TreeNode"./>
        /// </summary>
        /// <param name="caption">Отображаемый текст узла.</param>
        public TreeNode(string caption)
        {
            this.Caption = caption;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="TreeNode"./>
        /// </summary>
        /// <param name="source">Ссылка на абстракцию объекта АО, которую представляет узел.</param>
        public TreeNode(IA0Item source)
        {
            this.Caption = source.Name;
            this.Source = source;
            this.SubNodes = new ObservableCollection<ITreeNode>();
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="TreeNode"./>
        /// </summary>
        /// <param name="caption">Отображаемый текст узла.</param>
        /// <param name="source">Ссылка на абстракцию объекта АО, которую представляет узел.</param>
        public TreeNode(string caption, IA0Item source)
            : this(source)
        {
            this.Caption = caption;
        }

        /// <summary>
        /// Получает отображаемый текст узла.
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// Получает или устанавливает список дочерних узлов, входящих в состав родительского узла.
        /// </summary>
        public IList<ITreeNode> SubNodes { get; set; }

        /// <summary>
        /// Получает ссылку на абстракцию объекта А0, представляемую узлом.
        /// </summary>
        public IA0Item Source { get; }

        /// <summary>
        /// Получает тип узла.
        /// </summary>
        public TreeNodeKind Kind => this.Source is A0Section ? TreeNodeKind.Section : (TreeNodeKind)this.Source?.Kind;

        /// <summary>
        /// Получает или устанавливает признак заполненности списка дочерних узлов.
        /// </summary>
        public bool IsFilled { get; set; }
    }
}