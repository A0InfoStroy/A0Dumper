namespace A0Dumper.Data.A0Items.Entities
{
    using System;

    /// <summary>
    /// Представляет абстракцию сметного объекта в А0.
    /// </summary>
    public class A0Item : IA0Item
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="A0Item"./>
        /// </summary>
        /// <param name="kind">Тип сметного объекта.</param>
        public A0Item(A0ItemKind kind)
        {
            this.Kind = kind;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="A0Item"./>
        /// </summary>
        /// <param name="name">Наименование сметного объекта.</param>
        /// <param name="kind">Тип сметного объекта.</param>
        /// <param name="guid">Уникальный идентификатор сметного объекта.</param>
        /// <param name="mark">Шифр сметного объекта.</param>
        /// <param name="date">Дата создания сметного объекта.</param>
        public A0Item(string name, A0ItemKind kind, Guid guid, string mark, DateTime date)
            : this(kind)
        {
            this.Name = name;
            this.Guid = guid;
            this.Mark = mark;
            this.CreationDate = date;
        }

        /// <summary>
        /// Получает наименование сметного объекта.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Получает тип сметного объекта.
        /// </summary>
        public A0ItemKind Kind { get; }

        /// <summary>
        /// Получает уникальный идентификатор сметного объекта.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Получает шифр сметного объекта.
        /// </summary>
        public string Mark { get; }

        /// <summary>
        /// Получает дату создания сметного объекта.
        /// </summary>
        public DateTime CreationDate { get; }
    }
}