namespace A0Dumper.UI.WinForms
{
    using System;
    using System.Windows.Forms;
    using A0Dumper.Smart.Tree.Entities;

    /// <summary>
    /// Предоставляет изображение значка соответствующее узлу дерева.
    /// </summary>
    public static class IconManager
    {
        /// <summary>
        /// Список изображений значков для узлов дерева.
        /// </summary>
        private static ImageList imageList;

        /// <summary>
        /// Получает список изображений значков для узлов дерева.
        /// </summary>
        public static ImageList ImageList
        {
            get
            {
                if (imageList == null)
                {
                    imageList = new ImageList();
                    imageList.Images.Add("chapter", Properties.Resources.chapter);
                    imageList.Images.Add("head", Properties.Resources.head);
                    imageList.Images.Add("complex", Properties.Resources.complex);
                    imageList.Images.Add("project", Properties.Resources.project);
                    imageList.Images.Add("os", Properties.Resources.os);
                    imageList.Images.Add("ls", Properties.Resources.ls);
                }

                return imageList;
            }
        }

        /// <summary>
        /// Получает имя изображения по типу узла дерева.
        /// </summary>
        /// <param name="treeNode">Узел дерева.</param>
        /// <returns>Наименование изображения.</returns>
        public static string GetIcon(ITreeNode treeNode)
        {
            switch (treeNode.Kind)
            {
                case TreeNodeKind.Head:
                    return "head";
                case TreeNodeKind.Complex:
                    return "complex";
                case TreeNodeKind.Project:
                    return "project";
                case TreeNodeKind.OS:
                    return "os";
                case TreeNodeKind.LS:
                    return "ls";
                case TreeNodeKind.Section:
                    return "chapter";
                default:
                    throw new ApplicationException("Неизвестный тип узла");
            }
        }
    }
}