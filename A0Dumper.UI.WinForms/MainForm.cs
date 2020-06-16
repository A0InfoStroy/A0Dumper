namespace A0Dumper.UI.WinForms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using A0Dumper.Data.A0Items.Service;
    using A0Dumper.Smart.Tree.Entities;
    using A0Dumper.Smart.Tree.Service;
    using A0Dumper.UI.CommonLib;
    using A0Dumper.UI.CommonLib.Entities;
    using A0Dumper.UI.CommonLib.Service;
    using TreeNode = System.Windows.Forms.TreeNode;

    /// <summary>
    /// Обеспечивает логику взаимодействия с основной формой.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Ссылка на объект обеспечивающий соединение с БД А0.
        /// </summary>
        private ConnectionService connectionService;

        /// <summary>
        /// Ссылка на дерево.
        /// </summary>
        private ITree tree;

        /// <summary>
        /// Инициализирует новый экземпляр класса.<seealso cref="MainForm"./>
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.CenterToScreen();
        }

        /// <summary>
        /// Запускает форму ввода логина и пароля.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            DependencyInjector dependencyInjector = new DependencyInjector();
            AuthenticationForm authWindow = new AuthenticationForm(dependencyInjector.CreateWindowSettings());
            if (authWindow.ShowDialog() == DialogResult.OK)
            {
                // Создание службы в случае успешной аутентификации
                this.connectionService = authWindow.ConnectionService;
                A0Service.IA0EstimateRepo repo = this.connectionService.Repo;
                ITreeService treeService = dependencyInjector.CreateTreeService("repo", repo);

                // Создание дерева с корневым узлом "Проекты"
                this.tree = treeService.GetTree();
            }
            else
            {
                // Закрытие приложения при нажатие "Отмены" или закрытия формы аутентификации
                this.Close();
            }
        }

        /// <summary>
        /// Показывает на форме корневой узел при выборе опции "Bывод сметных объектов".
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void ListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Определение значков для узлов дерева
            this.treeView.ImageList = IconManager.ImageList;

            // Определение значка для корневого узла дерева
            string icon = IconManager.GetIcon(this.tree.Head);

            // Создание элемента формы и связывание с корневым узлом дерева
            TreeNode head = new TreeNode(this.tree.Head.Caption) { Tag = this.tree.Head, ImageKey = icon, SelectedImageKey = icon };

            // Предварительное заполнение элемента формы для возможности инициировать событие раскрытия
            head.Nodes.Add(string.Empty);

            // Отображение корневого узла дерева на форме
            this.treeView.Nodes.Add(head);

            // Разрешение доступа к настройкам дерева после его создания
            this.viewToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// Показывает на форме подузлы входящие в состав узла при его раскрытии.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private async void TreeView_AfterExpandAsync(object sender, TreeViewEventArgs e)
        {
            // Получение данных о раскрытом узле
            if (!(e.Node.Tag is ITreeNode expandedNode))
            {
                return;
            }

            // Операции для незаполненных узлов, раскрываемых первый раз
            if (!expandedNode.IsFilled)
            {
                // Получение коллекции подузлов в отдельном потоке приложения
                await this.tree.GetSubNodesAsync(expandedNode);
            }

            this.CreateTreeViewItems(e, expandedNode);
        }

        /// <summary>
        /// Создает дочерние узлы для элемента формы.
        /// </summary>
        /// <param name="e">Аргументы события.</param>
        /// <param name="displayedNode">Отображаемый узел.</param>
        private void CreateTreeViewItems(TreeViewEventArgs e, ITreeNode displayedNode)
        {
            // Очистка раскрытого элемента формы после предварительного заполнения
            e.Node.Nodes.Clear();
            foreach (ITreeNode subNode in displayedNode.SubNodes)
            {
                // Определение значка узла дерева в зависимости от его типа
                string icon = IconManager.GetIcon(subNode);

                // Создание элемента формы и связывание с узлом дерева
                TreeNode treeNode = new TreeNode(subNode.Caption) { Tag = subNode, ImageKey = icon, SelectedImageKey = icon };

                // Предварительное заполнение подузлов для возможности последующего раскрытия
                treeNode.Nodes.Add(string.Empty);

                // Отображение дочерних узлов на форме
                e.Node.Nodes.Add(treeNode);
            }
        }

        /// <summary>
        /// Передает дереву критерий по которому сортируются дочерние узлы.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void OrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                this.CheckMenuItem(this.orderToolStripMenuItem, menuItem);
                this.tree.SortKey = UIService.GetSortCriterion(menuItem.Text);
            }
        }

        /// <summary>
        /// Передает дереву информацию о порядке сортировки.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void DisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                this.CheckMenuItem(this.displayToolStripMenuItem, item);
                this.tree.Descending = item == this.descendingToolStripMenuItem;
            }
        }

        /// <summary>
        /// Сбрасывает статусы всех элементов меню кроме выбранного.
        /// </summary>
        /// <param name="parent">Меню содерждащее выбираемые элементы.</param>
        /// <param name="checkedItem">Выбранный элемент меню.</param>
        private void CheckMenuItem(ToolStripMenuItem parent, ToolStripMenuItem checkedItem)
        {
            foreach (ToolStripItem item in parent.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.Checked = menuItem == checkedItem;
                }
            }
        }

        /// <summary>
        /// Показывает таблицу сметных объектов входящих в выбранный узел.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private async void TreeView_AfterSelectAsync(object sender, TreeViewEventArgs e)
        {
            // Получение данных о раскрытом узле
            if (!(e.Node.Tag is ITreeNode selectedNode))
            {
                return;
            }

            // Запрос на получение дочерних узлов для нераскрытих ранее узлов
            if (!selectedNode.IsFilled)
            {
                await this.tree.GetSubNodesAsync(selectedNode);
                this.CreateTreeViewItems(e, selectedNode);
            }

            IList<TreeNodeRow> rows;

            // Проверка содержит ли выбранный узел главы
            if (selectedNode.SubNodes.FirstOrDefault()?.Kind == TreeNodeKind.Section)
            {
                // Чтение содержимого глав и сохранение результатов в список
                rows = new List<TreeNodeRow>();
                await UIService.ReadChapter(selectedNode, rows, this.tree);
            }
            else
            {
                // Проекция списка дочерних узлов в список строк таблицы сметных объектов
                rows = selectedNode.SubNodes.Select(x => new TreeNodeRow(x)).ToList();
            }

            // Связывание списка дочерних узлов с таблицей на форме
            var bindingList = new BindingList<TreeNodeRow>(rows);
            this.nodeDataGridView.DataSource = bindingList;

            // Форматирование ширины столбцов таблицы
            this.nodeDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.nodeDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.nodeDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.nodeDataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }
    }
}