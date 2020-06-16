namespace A0Dumper.Data.A0Items.Entities
{
    using System;

    /// <summary>
    /// Описывает параметры объекта в А0.
    /// </summary>
    public interface IA0Item
    {
        /// <summary>
        /// Получает наименование объекта в А0.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Получает тип сметного объекта в А0.
        /// </summary>
        A0ItemKind Kind { get; }

        /// <summary>
        /// Получает уникальный идентификатор объекта в А0.
        /// </summary>
        Guid Guid { get; }
    }

    /// <summary>
    /// Типы объектов в А0.
    /// </summary>
    public enum A0ItemKind
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
    }
}