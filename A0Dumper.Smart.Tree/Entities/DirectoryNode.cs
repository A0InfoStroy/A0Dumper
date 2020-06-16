namespace A0Dumper.Smart.Tree.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Представляет папку в древовидной структуре A0
    /// </summary>
    public class DirectoryNode : ITreeNode
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <seealso cref="DirectoryNode"./>
        /// </summary>
        /// <param name="caption">Отображаемый текст узла</param>
        /// <param name="id">Идентификатор папки</param>
        /// <param name="root">Ссылка на сметный объект, который содержит данный экземпляр</param>
        public DirectoryNode(string caption, int id, ITreeNode root)
        {
            this.Caption = caption;
            this.Id = id;
            this.Root = root;
            this.SubNodes = new ObservableCollection<ITreeNode>();
        }

        /// <summary>
        /// Получает отображаемый текст узла.
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// Получает тип сметного объекта в котором находится папка.
        /// </summary>
        public TreeNodeKind Kind => Root?.Kind ?? TreeNodeKind.Unknown;

        /// <summary>
        /// Получает уникальный идентификатор сметного объекта в котором находится папка.
        /// </summary>
        public Guid Guid => Root?.Guid ?? Guid.Empty;

        /// <summary>
        /// Получает список дочерних узлов, входящих в состав родительского узла.
        /// </summary>
        public IList<ITreeNode> SubNodes { get; }

        /// <summary>
        /// Получает или устанавливает признак заполненности списка дочерних узлов.
        /// </summary>
        public bool IsFilled { get; set; }

        /// <summary>
        /// Получает идентификатор папки. 
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Получает ссылку на сметный объект, в котором находится папка.
        /// </summary>
        public ITreeNode Root { get;}
    }
}