namespace A0Dumper.UI.WPF
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using A0Dumper.Data.A0Items.Service;
    using A0Dumper.Smart.Tree.Entities;
    using A0Dumper.Smart.Tree.Service;
    using A0Dumper.UI.CommonLib;
    using A0Dumper.UI.CommonLib.Entities;
    using A0Dumper.UI.CommonLib.Service;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
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
        /// Инициализирует новый экземпляр класса.<seealso cref="MainWindow"./>
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Запускает форму ввода логина и пароля.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void MainForm_Loaded(object sender, RoutedEventArgs e)
        {
            DependencyInjector dependencyInjector = new DependencyInjector();
            AuthenticationWindow authWindow = new AuthenticationWindow(dependencyInjector.CreateWindowSettings());
            if (authWindow.ShowDialog() == true)
            {
                // Создание службы в случае успешной аутентификации
                this.connectionService = authWindow.ConnectionService;
                A0Service.IA0EstimateRepo repo = this.connectionService.Repo;
                ITreeService treeService = dependencyInjector.CreateTreeService("repo", repo);

                // Создание дерева с корневым узлом
                this.tree = treeService.GetTree();
            }
            else
            {
                // Закрытие приложения при нажатие "Отмены" или закрытия формы аутентификации
                this.Close();
            }
        }

        /// <summary>
        /// Показывает на форме дерево сметных объектов.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void ListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Отображение корневого узла дерева на форме
            this.treeViewMenu.Items.Add(this.tree.Head);

            // Автоматическое раскрытие корневого узла
            TreeViewItem root = this.treeViewMenu.ItemContainerGenerator.ContainerFromItem(this.tree.Head) as TreeViewItem;
            root.IsExpanded = true;
        }

        /// <summary>
        /// Показывает на форме подузлы входящие в состав узла при его раскрытии.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private async void TreeViewItem_ExpandedAsync(object sender, RoutedEventArgs e)
        {
            // Получение данных о раскрытом узле
            if (!(e.OriginalSource is TreeViewItem source) || !(source.Header is ITreeNode expandedNode))
            {
                return;
            }

            // Операции для незаполненных узлов, раскрываемых первый раз
            if (!expandedNode.IsFilled)
            {
                // Получение коллекции подузлов в отдельном потоке приложения
                await this.tree.GetSubNodesAsync(expandedNode);
            }
        }

        /// <summary>
        /// Передает дереву критерий по которому сортируются дочерние узлы.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void OrderMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem)
            {
                this.tree.SortKey = UIService.GetSortCriterion(menuItem.Header.ToString());
            }
        }

        /// <summary>
        /// Передает дереву информацию о порядке сортировки.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void DisplayMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            this.tree.Descending = e.OriginalSource is MenuItem mi && mi == this.descendingMenuItem;
        }

        /// <summary>
        /// Показывает таблицу сметных объектов входящих в выбранный узел.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private async void TreeViewMenu_SelectedAsync(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is TreeViewItem source) || !(source.Header is ITreeNode selectedNode))
            {
                return;
            }

            // Запрос на получение дочерних узлов для нераскрытих ранее узлов
            if (!selectedNode.IsFilled)
            {
                await this.tree.GetSubNodesAsync(selectedNode);
            }

            // Проверка содержит ли выбранный узел главы
            IList<TreeNodeRow> rows;
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

            this.nodeContentDataGrid.ItemsSource = rows;
        }

        /// <summary>
        /// Выполняет очистку неуравляемых ресурсов при закрытии окна приложения.
        /// </summary>
        /// <param name="sender">Объект инициировавший событие.</param>
        /// <param name="e">Аргументы события.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.connectionService?.Dispose();
        }
    }
}