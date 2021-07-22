using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Drawing;
using TreeView = System.Windows.Forms.TreeView;
using MessageBox = System.Windows.MessageBox;
using Color = System.Drawing.Color;

namespace WpfFormsApp
{
    public partial class MainWindow : Window
    {
        TreeView treeView1 = new TreeView();
        MainRepository repository;
        public MainWindow()
        {
            InitializeComponent();
            HostTreeView.Child = treeView1;
            treeView1.LostFocus += treeView1_LostFocus;
            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.BeforeExpand += TreeView1_BeforeExpand;
            repository = new MainRepository(CE_Context.GetInstance("CoreEntityConnection"));
            LoadRoots();
            
        }
        #region Методы отображения дерева
        void LoadRoots() //метод прогружает корни дерева (группы без отцов)
        {
            List<CE_Tgroup> Tgroups = repository.Tgroups.GetTgroupsInList();
            List<CE_Trelation> Trelations = repository.Trelations.GetTrelationInList();
            foreach (var group in Tgroups)
            {
                if (Trelations.Find(x => x.Id_child == group.Id) == null)
                {
                    var nodeToAdd = group.ConvertToTreeNode();
                    nodeToAdd.Nodes.Add(GetTechNode());
                    treeView1.Nodes.Add(nodeToAdd);
                }
            }
        }
        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        { //перед раскрытием узла удаляет техническую группу, и прогружает всех детей раскрываемой группы
            string type = e.Node.Name.Split('|')[1];
            if (e.Node.FirstNode.Name == GetTechNode().Name)
            {
                e.Node.Nodes.Clear();
                if (type == "Group")
                    LoadChildrenNodes(e.Node);
            }    
        }
        void LoadChildrenNodes(TreeNode node)
        {   //отображает всех детей группы
            int id = int.Parse(node.Name.Split('|')[0]);
            List<CE_Tgroup> Tgroups = repository.Tgroups.GetTgroupsInList();
            List<CE_Trelation> Trelations = repository.Trelations.GetTrelationInList().FindAll(x=>x.Id_parent == id);
            List<CE_Tproperty> Tproperties = repository.Tproperties.GetTproperyInList().FindAll(x => x.Group_id == id);
            if (Trelations == null) return;
            if (Trelations != null)
            {
                foreach(var rel in Trelations)
                {
                    var nodeToAdd = Tgroups.Find(x => x.Id == rel.Id_child).ConvertToTreeNode();
                    nodeToAdd.Nodes.Add(GetTechNode());
                    node.Nodes.Add(nodeToAdd);
                }
            }
            if(Tproperties != null)
            {
                foreach (var prop in Tproperties)
                {
                    node.Nodes.Add(prop.ConvertToTreeNode()); 
                }
            }
        }
        #endregion
        #region Служебные методы и общие обработчики событий
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) => repository.Disconnect();
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) //снимает выделение с узла дерева при нажатии на esc
        {
            if (e.Key == Key.Escape)
            {
                DeSelectNode();
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }
        TreeNode GetTechNode()
        {
            return new TreeNode()
            {
                Name = "TechnicalGroup",
                Text = "TechnicalGroup"
            };
        } //возвращает технический узел
        int GetSelectedTreeNodeId() => int.Parse(treeView1.SelectedNode.Name.Split('|')[0]); //возвращает Id выделенного узла
        string GetSelectedTreeNodeType() => treeView1.SelectedNode.Name.Split('|')[1]; //возвращает Name выделенного узла
        string GetTpropertyValue(int id) => repository.Tproperties.GetTpropertyById(id).Value; //возвращает описание свойства по Id
        #endregion
        #region Обработчики событий меню
        private void AddTgroupMenuButton_Click(object sender, RoutedEventArgs e) //скрывает форму добавления свойства, и влючает форму добавления группы
        {
            if (AddTpropertyGroupBox.Visibility == Visibility.Visible)
            {
                AddTpropertyGroupBox.Visibility = Visibility.Hidden;
                AddTgroupGroupBox.Visibility = Visibility.Visible;
            }
        }
        private void AddTpropertyMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddTgroupGroupBox.Visibility == Visibility.Visible)
            {
                AddTgroupGroupBox.Visibility = Visibility.Hidden;
                AddTpropertyGroupBox.Visibility = Visibility.Visible;
            }
        } //скрывает форму добавления группы и включает форму добавления свойства
        private void EditButton_Click(object sender, RoutedEventArgs e) //включает форму редактирования узла, преждевременно скрывая другую форму редактирования если тип редактируемого объекта поменялся и загружая в TextBox'ы редактируемые данные
        {
            string type = GetSelectedTreeNodeType();
            if (type == "Group")
            {
                if (EditTgroupGroupBox.Visibility == Visibility.Hidden)
                {
                    EditTpropertyGroupBox.Visibility = Visibility.Hidden;
                    EditTgroupGroupBox.Visibility = Visibility.Visible;
                }
                EditTgroupNameTextBox.Text = treeView1.SelectedNode.Text;
                EditTgroupGroupBox.IsEnabled = true;
                SelectNode();
                EditTgroupNameTextBox.Focus();
            }
            if (type == "Property")
            {
                if (EditTpropertyGroupBox.Visibility == Visibility.Hidden)
                {
                    EditTgroupGroupBox.Visibility = Visibility.Hidden;
                    EditTpropertyGroupBox.Visibility = Visibility.Visible;
                }
                EditTpropertyNameTextBox.Text = treeView1.SelectedNode.Text;
                EditTpropertyValueTextBox.Text = GetTpropertyValue(GetSelectedTreeNodeId());
                EditTpropertyGroupBox.IsEnabled = true;
                SelectNode();
                EditTpropertyNameTextBox.Focus();
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e) //удаляет юзел и соответствующий элемент, и все зависящие от него элементы, снимает выделение
        {
            if (treeView1.SelectedNode != null)
            {
                int id = GetSelectedTreeNodeId();
                string type = GetSelectedTreeNodeType();
                if (type == "Group")
                    repository.Tgroups.DeleteTgroupEntity(id);
                if (type == "Property")
                    repository.Tproperties.DeleteTpropertyEntity(id);
                treeView1.Nodes.Remove(treeView1.SelectedNode);
                treeView1.SelectedNode = null;
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }
        #endregion
        #region Обработчик собтиый выделения treeView и узлов
        /// <summary>
        /// при выделении узла кнопки "удалить" и "редактировать" становятся доступными
        /// при снятии выделения соответственнно наоборот
        /// </summary>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                DeleteButton.IsEnabled = true;
                EditButton.IsEnabled = true;
            }
        }
        private void treeView1_LostFocus(object sender, EventArgs e)
        {
            if (!treeView1.Focused)
            {
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
            }
        }
        /// <summary>
        /// покраска и стирание покраски выделенного узла
        /// </summary>
        void SelectNode()
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.BackColor = Color.SteelBlue;
                treeView1.SelectedNode.ForeColor = Color.White;
            }
        }
        void DeSelectNode()
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.BackColor = Color.White;
                treeView1.SelectedNode.ForeColor = Color.Black;
                treeView1.SelectedNode = null;
            }
        }
        #endregion
        #region Обработчики событий фокуса на GroupBox'ах
        private void AddTgroupGroupBox_GotFocus(object sender, RoutedEventArgs e) => SelectNode();
        private void AddTpropertyGroupBox_GotFocus(object sender, RoutedEventArgs e) => SelectNode();
        private void EditTgroupGroupBox_GotFocus(object sender, RoutedEventArgs e) => SelectNode();
        private void EditTpropertyGroupBox_GotFocus(object sender, RoutedEventArgs e) => SelectNode();
        private void AddTgroupGroupBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!AddTgroupGroupBox.IsMouseOver)
                DeSelectNode();
        }
        private void AddTpropertyGroupBox_LostFocus(object sender, RoutedEventArgs e)
        {
             if(!AddTpropertyGroupBox.IsMouseOver)
                 DeSelectNode();   
        }
        private void EditTpropertyGroupBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!EditTpropertyGroupBox.IsMouseOver)
            {
                EditTpropertyCancelButton_Click(null, null);
                EditTpropertyGroupBox.IsEnabled = false;
                DeSelectNode();
            }
        
        }
        private void EditTgroupGroupBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!EditTgroupGroupBox.IsMouseOver)
            {
                EditTgroupCancelButton_Click(null, null);
                EditTgroupGroupBox.IsEnabled = false;
                DeSelectNode();
            }
        }
        #endregion
        #region Обработчики событий ...SaveButton_Click
        private void AddTgroupSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddTgroupNameTextBox.Text != "")
            {
                if (treeView1.SelectedNode == null)
                {
                    repository.Tgroups.CreateTgroupEntity(AddTgroupNameTextBox.Text);
                    treeView1.Nodes.Add(repository.Tgroups.GetTgroupsInList().Last().ConvertToTreeNode());
                }
                else
                {
                    string type = GetSelectedTreeNodeType();
                    if (type == "Property")
                    {
                        MessageBox.Show("Нельзя добавить группу к свойству, выберите группу или снимите выделение с узла");
                        return;
                    }
                    if (type == "Group")
                    {
                        repository.Tgroups.CreateTgroupEntity(AddTgroupNameTextBox.Text);
                        repository.Trelations.CreateTrelationEntity(GetSelectedTreeNodeId(), repository.Tgroups.GetTgroupsInList().Last().Id);
                        treeView1.SelectedNode.Nodes.Add(repository.Tgroups.GetTgroupsInList().Last().ConvertToTreeNode());
                    }
                }
                AddTgroupCancelButton_Click(null, null);
            }
            else
                MessageBox.Show("Введите значение");
        }  //добавляет группу с учетом выделенного узла
        private void AddTpropertySaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (AddTpropertyNameTextBox.Text != "" && AddTpropertyValueTextBox.Text != "" && GetSelectedTreeNodeType() == "Group")
                {
                    repository.Tproperties.CreateTpropertyEntity(AddTpropertyNameTextBox.Text, AddTpropertyValueTextBox.Text, GetSelectedTreeNodeId());
                    treeView1.SelectedNode.Nodes.Add(repository.Tproperties.GetTproperyInList().Last().ConvertToTreeNode());
                }
                if(AddTpropertyNameTextBox.Text == "" || AddTpropertyValueTextBox.Text == "")
                    MessageBox.Show("Введите значения");
                if(GetSelectedTreeNodeType() == "Property")
                    MessageBox.Show("Нельзя добавить свойство к свойству");
                AddTpropertyCancelButton_Click(null, null);
            }
            else
                MessageBox.Show("Выберите группу, к которой надо добавить свойство");
        } //добавляет свойство с учетом выделенного узла
        private void EditTgroupSaveButton_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(treeView1.SelectedNode.Name.Split('|')[0]);
            if (EditTgroupNameTextBox.Text != "")
            {
                repository.Tgroups.UpdateTgroupEntity(id, EditTgroupNameTextBox.Text);
                treeView1.SelectedNode.Text = EditTgroupNameTextBox.Text; 
                EditTgroupNameTextBox.Text = "";
                EditTgroupGroupBox.IsEnabled = false;
            }
            else
                MessageBox.Show("Введите значение");
        } //сохраняет изменения в группе
        private void EditTpropertySaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditTpropertyNameTextBox.Text != "" && EditTpropertyValueTextBox.Text != "")
            {
                repository.Tproperties.UpdateTpropertyEntity(GetSelectedTreeNodeId(), EditTpropertyNameTextBox.Text, EditTpropertyValueTextBox.Text);
                treeView1.SelectedNode.Text = EditTpropertyNameTextBox.Text; 
                EditTpropertyCancelButton_Click(null, null);
                EditTpropertyGroupBox.IsEnabled = false;
            }
            else
                MessageBox.Show("Введите значения");
        } //сохраняет изменения в свойстве
        #endregion 
        #region Обработчики событий ...CancelButton_Click
        /// <summary>
        /// удаляют текст из TextBox'ов
        /// </summary>
        private void AddTgroupCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddTgroupNameTextBox.Text != "")
                AddTgroupNameTextBox.Text = "";
        }
        private void AddTpropertyCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddTpropertyNameTextBox.Text != "" || AddTpropertyValueTextBox.Text != "")
            {
                AddTpropertyNameTextBox.Text = "";
                AddTpropertyValueTextBox.Text = "";
            }
        }
        private void EditTgroupCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditTgroupNameTextBox.Text != "")
                EditTgroupNameTextBox.Text = "";
        }
        private void EditTpropertyCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditTpropertyNameTextBox.Text != "" || EditTpropertyValueTextBox.Text != "")
            {
                EditTpropertyNameTextBox.Text = "";
                EditTpropertyValueTextBox.Text = "";
            }
        }
        #endregion


    }
}
