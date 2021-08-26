namespace A0Dumper.Smart.Tree.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Представляет сметный объект в древовидной структуре А0.
    /// </summary>
    public class EstimateObjectNode : ITreeNode
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <seealso cref="EstimateObjectNode"./>
        /// </summary>
        /// <param name="caption">Отображаемый текст узла</param>
        public EstimateObjectNode(string caption)
        {
            this.Caption = caption;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <seealso cref="EstimateObjectNode"./>
        /// </summary>
        /// <param name="caption">Отображаемый текст узла</param>
        /// <param name="kind">Тип узла</param>
        public EstimateObjectNode(string caption, TreeNodeKind kind) : this(caption)
        {
            this.Kind = kind;
            this.SubNodes = new ObservableCollection<ITreeNode>();
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <seealso cref="EstimateObjectNode"./>
        /// </summary>
        /// <param name="caption">Отображаемый текст узла</param>
        /// <param name="kind">Тип узла</param>
        /// <param name="guid">Уникальный идентификатор узла</param>
        public EstimateObjectNode(string caption, TreeNodeKind kind, Guid guid) : this(caption, kind)
        {
            this.Guid = guid;
        }

        /// <summary>
        /// Получает отображаемый текст узла.
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// Получает тип сметного объекта, представленного узлом.
        /// </summary>
        public TreeNodeKind Kind { get; }

        /// <summary>
        /// Получает уникальный идентификатор сметного объекта, представленного узлом.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Получает список дочерних узлов, входящих в состав родительского узла.
        /// </summary>
        public IList<ITreeNode> SubNodes { get; }

        /// <summary>
        /// Получает или устанавливает признак заполненности списка дочерних узлов.
        /// </summary>
        public bool IsFilled { get; set; }
    }
}