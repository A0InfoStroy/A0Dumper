namespace A0Dumper.Data.A0Items.Entities
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Представляет абстракцию главы или раздела в А0.
    /// </summary>
    public class A0Section : IA0Item
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="A0Section"./>
        /// </summary>
        /// <param name="name">Наименование главы или раздела.</param>
        /// <param name="id">Идентификатор главы или раздела.</param>
        /// <param name="root">Ссылка на сметный объект содержащий данный экземпляр.</param>
        public A0Section(string name, int id, IA0Item root)
        {
            this.Name = name;
            this.Id = id;
            this.Root = root;
            this.SubSections = new List<A0Section>();
        }

        /// <summary>
        /// Получает наименование главы или раздела.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Получает тип сметного объекта содержащего данный экземпляр.
        /// </summary>
        public A0ItemKind Kind => this.Root?.Kind ?? A0ItemKind.Unknown;

        /// <summary>
        /// Получает уникальный идентификатор сметного объекта содержащего данный экземпляр.
        /// </summary>
        public Guid Guid => this.Root?.Guid ?? Guid.Empty;

        /// <summary>
        /// Получает список подразделов в разделе.
        /// </summary>
        public IList<A0Section> SubSections { get; }

        /// <summary>
        /// Получает ссылку на сметный объект содержащий данный экземпляр.
        /// </summary>
        public IA0Item Root { get; }

        /// <summary>
        /// Получает идентификатор главы или раздела.
        /// </summary>
        public int Id { get; }
    }
}