namespace A0Dumper.Data.A0Items.Repository
{
    using System;
    using System.Collections.Generic;
    using A0Dumper.Data.A0Items.Entities;
    using A0Service;

    /// <summary>
    /// Предоставляем механизм получения данных обобщенных объектов А0.
    /// </summary>
    public class A0ItemRepo : IA0ItemRepo
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="A0ItemRepo"./>
        /// </summary>
        /// <param name="repo">Каталог обобщенных объектов А0.</param>
        public A0ItemRepo(IA0EstimateRepo repo)
        {
            this.Repo = repo;
        }

        /// <summary>
        /// Каталог обобщенных объектов А0.
        /// </summary>
        public IA0EstimateRepo Repo { get; }

        /// <summary>
        /// Осуществляет чтение каталога обобщенных объектов А0.
        /// </summary>
        /// <param name="parent">Родительский узел.</param>
        /// <returns>Список элементов содержащихся в сметном объекте.</returns>
        public IList<IA0Item> Read(IA0Item parent)
        {
            IA0ObjectIterator iterator;
            switch (parent.Kind)
            {
                case A0ItemKind.Unknown:
                    break;
                case A0ItemKind.Head:
                    // Получение списка элементов, содержищихся в головном узле "Проекты"
                    return this.GetTopLevelItems(this.Repo.ComplexID.HeadComplexGUID);
                case A0ItemKind.Complex:
                    if (parent is A0Section complexSection)
                    {
                        // Получение комплексов и проектов внутри глав или разделов
                        List<IA0Item> nestedComplexes = new List<IA0Item>();
                        IA0ObjectIterator complexIterator = this.Repo.ComplexID.Read3(complexSection.Root.Guid, complexSection.Id, null);
                        IList<IA0Item> complexes = this.GetEstimateObjects(complexIterator);
                        nestedComplexes.AddRange(complexes);
                        IA0ObjectIterator projectIterator = this.Repo.ProjID.Read3(complexSection.Root.Guid, complexSection.Id, null);
                        IList<IA0Item> projects = this.GetEstimateObjects(projectIterator);
                        nestedComplexes.AddRange(projects);
                        return nestedComplexes;
                    }

                    // Получение корневого узла дерева глав и разделов входящих в комплекс
                    IA0TreeNode treeHead = this.Repo.Complex.Load(parent.Guid, EAccessKind.akRead).Tree.Head;

                    // Получение сметных объектов или структуры глав и разделов, входящих в комплекс
                    return treeHead.Count == 0 ? this.GetTopLevelItems(parent.Guid) : this.GetSectionStructure(parent, treeHead);
                case A0ItemKind.Project:
                    IA0OSIDRepo osRepo = this.Repo.OSID;
                    if (parent is A0Section projectSection)
                    {
                        // Создание запроса на получение ОС, находящихся в главе или разделе
                        ISQLWhere where = osRepo.GetWhereRequest();
                        where.Node.And3("[OSTitle].[TotalNodeID]", "=", projectSection.Id.ToString());

                        // Получение итератора по отфильтрованной коллекции
                        iterator = osRepo.Read(projectSection.Root.Guid, null, where, null);
                        return this.GetEstimateObjects(iterator);
                    }

                    // Получение корневого узла дерева глав и разделов входящих в проект
                    treeHead = this.Repo.Proj.Load(parent.Guid, false).Tree.Head;
                    if (treeHead.Count == 0)
                    {
                        // Получение итератора по всей коллекции ОС входящих в проект
                        iterator = osRepo.Read(parent.Guid, null, null, null);
                        return this.GetEstimateObjects(iterator);
                    }

                    return this.GetSectionStructure(parent, treeHead);
                case A0ItemKind.OS:
                    IA0LSIDRepo lsRepo = this.Repo.LSID;

                    // Загрузка текущей ОС
                    IA0OS os = this.Repo.OS.Load(parent.Guid, false);

                    // Параметр необходимый для чтения в каталоге поиска ЛС
                    Guid projGuid = os.ID.ProjGUID;

                    // Создание запроса на получение ЛС, находящихся в текущей ОС
                    ISQLWhere lsWhere = lsRepo.GetWhereRequest();

                    // Приведение GUID ОС к строковому типу для запроса к БД
                    string osGuid = $"'{{{os.ID.GUID.ToString().ToUpper()}}}'";

                    // Добавление к запросу фильтрации по GUID текущей ОС
                    lsWhere.Node.And3("[LSTitle].[OSGUID]", "=", osGuid);
                    if (parent is A0Section osSection)
                    {
                        // Создание запроса на получение ЛС, находящихся в текущем разделе
                        lsWhere.Node.And3("[LSTitle].[TotalNodeID]", "=", osSection.Id.ToString());

                        // Получение итератора по отфильтрованной коллекции
                        iterator = lsRepo.Read(projGuid, null, lsWhere, null);
                        return this.GetEstimateObjects(iterator);
                    }

                    // Получение дерева разделов входящих в ОС
                    treeHead = os.Tree.Head;
                    if (treeHead.Count == 0)
                    {
                        // Получение итератора по всей коллекции ЛС входящих в ОС
                        iterator = lsRepo.Read(projGuid, null, lsWhere, null);
                        return this.GetEstimateObjects(iterator);
                    }

                    return this.GetSectionStructure(parent, treeHead);
                case A0ItemKind.LS:
                    break;
                case A0ItemKind.Act:
                    break;
                default:
                    break;
            }

            return new List<IA0Item>();
        }

        /// <summary>
        /// Получает сметные объекты, расположенные на уровне комплекса или входящие в его состав.
        /// </summary>
        /// <param name="guid">Уникальный идентификатор сметного объекта.</param>
        /// <returns>Коллекция сметных объектов, входящих в состав объекта с идентификатором<paramref name="guid"/>.</returns>
        private IList<IA0Item> GetTopLevelItems(Guid guid)
        {
            List<IA0Item> topLevelItems = new List<IA0Item>();

            // Получение комплексов
            IA0ObjectIterator iterator = this.Repo.ComplexID.Read2(guid, null);
            IList<IA0Item> complexes = this.GetEstimateObjects(iterator);
            topLevelItems.AddRange(complexes);

            // Получение проектов, находящихся на одном уровне с комплексами
            iterator = this.Repo.ProjID.Read2(guid, null);
            IList<IA0Item> projects = this.GetEstimateObjects(iterator);
            topLevelItems.AddRange(projects);
            return topLevelItems;
        }

        /// <summary>
        /// Получает вложенные элементы сметного объекта.
        /// </summary>
        /// <param name="iterator">Итератор сметного объекта A0.</param>
        /// <returns>Список сметных объектов.<seealso cref="EstimateObjectNode"/></returns>
        private IList<IA0Item> GetEstimateObjects(IA0ObjectIterator iterator)
        {
            IList<IA0Item> estimateObjects = new List<IA0Item>();
            while (iterator.Next())
            {
                // Текущий элемент итератора
                IA0Object item = iterator.Item;
                estimateObjects.Add(new A0Item(item.Name, (A0ItemKind)item.Kind, item.ID.GUID, item.Mark, item.CreateMoment()));
            }

            return estimateObjects;
        }

        /// <summary>
        /// Получает структуру глав и разделов входящих в сметный объект.
        /// </summary>
        /// <param name="parent">Родительский сметный объект.</param>
        /// <param name="tree">Древовидная структура разделов входящих в сметный объект.</param>
        /// <returns>Список глав и вложенных разделов содержащихся в родительском сметном объекте.</returns>
        private IList<IA0Item> GetSectionStructure(IA0Item parent, IA0TreeNode tree)
        {
            IList<IA0Item> chapters = new List<IA0Item>(tree.Count);
            for (int i = 0; i < tree.Count; i++)
            {
                IA0TreeNode chapter = tree.Item[i];
                A0Section section = new A0Section(chapter.Name, chapter.ID, parent);
                chapters.Add(section);
                this.FillSectionTree(chapter, section);
            }

            return chapters;
        }

        /// <summary>
        /// Заполняет список дочерних узлов раздела при наличии вложенных в него подразделов.
        /// </summary>
        /// <param name="node">Сметный объект в А0, содержащий вложенные элементы.</param>
        /// <param name="section">Глава или раздел.</param>
        private void FillSectionTree(IA0TreeNode node, A0Section section)
        {
            if (node.Count == 0)
            {
                return;
            }

            for (int i = 0; i < node.Count; i++)
            {
                A0Section innerDir = new A0Section(node.Item[i].Name, node.Item[i].ID, section.Root);
                section.SubSections.Add(innerDir);
                this.FillSectionTree(node.Item[i], innerDir);
            }
        }
    }
}