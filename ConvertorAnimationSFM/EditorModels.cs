using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using ConvertorAnimationSFMSimpleVersion;

namespace ConvertorAnimationSFM
{
    public partial class EditorModels : Form
    {
        private SQLiteConnection DB;
        private SQLiteCommand CMD;

        List<String> deleteFiles = new List<String>();

        public static CheckBox updateSelectNewBlendShape = new CheckBox();

        private void updateSelectNewBlendShape_CheckedChanged(object sender, EventArgs e)
        {
            if (BlendShapesTreeView.Nodes[0].Nodes.Count > 0)
            {
                BlendShapesTreeView.SelectedNode = BlendShapesTreeView.Nodes[0].LastNode;
            }

            updateSelectNewBlendShape.Checked = false;
        }

        public static CheckBox updateSelectBlendShape = new CheckBox();

        private void updateSelectBlendShape_CheckedChanged(object sender, EventArgs e)
        {
            if (BlendShapesTreeView.Nodes[0].Nodes.Count > 0)
            {
                BlendShapesTreeView.SelectedNode = BlendShapesTreeView.Nodes[0].Nodes[nameSelectItem.indexSelectBlendShape];
            }

            updateSelectBlendShape.Checked = false;
        }

        public static CheckBox updateBaseDataEditor = new CheckBox();
        private void updateBaseDataEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (updateBaseDataEditor.Checked == true)
            {
                //LoadDataBlenShapes();
                if (File.Exists(@"ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db"))
                {
                    BlendShapesTreeView.Nodes[0].Nodes.Clear();

                    DB = new SQLiteConnection("Data Source =" + @"ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "select * from BlendShapes";

                    SQLiteDataReader SQL = CMD.ExecuteReader();
                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            BlendShapesTreeView.Nodes[0].Nodes.Add(SQL["name"].ToString());
                        }
                    }
                    else
                    {
                        //statusNameLabel.Text = "Список сохраненых моделей пуст";
                    }

                    BlendShapesTreeView.Nodes[0].ExpandAll();

                    TreeNode[] findNode = BlendShapesTreeView.Nodes.Find(nameSelectItem.nameSelectBlendShape, true);
                    if (findNode.Length > 0) 
                    { 
                        BlendShapesTreeView.SelectedNode = findNode[0];
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                updateBaseDataEditor.Checked = false;
            }
        }

        public void LoadDataBlenShapes()
        {
            /*-----------------Загрузка данных из Базы Данных-------------------*/
            if (ModelsTreeView.SelectedNode != null) 
            {
                if (File.Exists(@"ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db"))
                {
                    BlendShapesTreeView.Nodes.Clear();
                    BlendShapesTreeView.Nodes.Add("BlendShapes");

                    DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "select * from BlendShapes";

                    SQLiteDataReader SQL = CMD.ExecuteReader();  
                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            BlendShapesTreeView.Nodes[0].Nodes.Add(SQL["name"].ToString());
                        }
                    }
                    else
                    {
                        //Text = "Список сохраненых моделей пуст";
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    BlendShapesTreeView.ExpandAll();

                    /*if (BlendShapesTreeView.Nodes[0].Nodes.Count == 0)
                    {
                        //downBlendShapesButton.Enabled = false;
                        contextMenuStrip.Items[2].Enabled = false;
                        contextMenuStrip.Items[4].Enabled = false;
                        contextMenuStrip.Items[5].Enabled = false;
                        contextMenuStrip.Items[7].Enabled = false;
                    }*/
                    /*else 
                    {
                        contextMenuStrip.Items[2].Enabled = true;
                        contextMenuStrip.Items[4].Enabled = true;
                        contextMenuStrip.Items[5].Enabled = true;
                        contextMenuStrip.Items[7].Enabled = true;
                    }*/
                }
            }
            else 
            {
                BlendShapesTreeView.Nodes.Clear();
                BlendShapesTreeView.Nodes.Add("BlendShapes");
            }
        }

        public EditorModels()
        {
            InitializeComponent();

            BlendShapesTreeView.Nodes[0].ExpandAll();

            updateSelectNewBlendShape.Checked = false;
            updateSelectNewBlendShape.CheckedChanged += updateSelectNewBlendShape_CheckedChanged;

            updateSelectBlendShape.Checked = false;
            updateSelectBlendShape.CheckedChanged += updateSelectBlendShape_CheckedChanged;

            updateBaseDataEditor.Checked = false;
            updateBaseDataEditor.CheckedChanged += updateBaseDataEditor_CheckedChanged;

            //Создание сокета для подключения к БД SQLite
            DB = new SQLiteConnection();

            //Создание папок для хранения баз данных, если они еще не созданы
            if (!Directory.Exists(@"ModelsResources"))
            {
                Directory.CreateDirectory(@"ModelsResources");
            }

            if (!Directory.Exists(@"ModelsResources\BlendShapes"))
            {
                Directory.CreateDirectory(@"ModelsResources\BlendShapes");
            }

            if (!File.Exists(@"ModelsResources\ModelsList.db"))
            {
                try
                {
                    DB = new SQLiteConnection("Data Source=ModelsResources/ModelsList.db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "CREATE TABLE Models (№ INTEGER PRIMARY KEY AUTOINCREMENT, name STRING NOT NULL UNIQUE);";
                    CMD.ExecuteNonQuery();

                }
                catch (SQLiteException ex)
                {
                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    MessageBox.Show("Ошибка создания базы данных: " + ex.Message);
                }

                CMD.Dispose();
                DB.Close();
                DB.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            //Назначаем рендер для контекстного меню
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new InterfaceWork.MyColorTable());
            contextMenuStrip.Renderer = new ToolStripProfessionalRenderer(new InterfaceWork.MyColorTable2());

            //Разернуть все treeView
            BlendShapesTreeView.ExpandAll();
            ModelsTreeView.ExpandAll();
        }

        //Предотвращение выбора корневого элемента Модели
        private void ModelsTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Text.Contains("Модели"))
            {
                e.Cancel = true;
            }
        }

        /*----------------------------------ОБРАБОТКА СОБЫТИЙ----------------------------------*/
        /*----------------------------------------ФОРМА----------------------------------------*/
        private void EditorModels_Load(object sender, EventArgs e)
        {
            //Центрирование положения окна
            this.CenterToParent();
          
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            /*-----------------Загрузка данных из Базы Данных-------------------*/
            if (File.Exists(@"ModelsResources\ModelsList.db"))
            {

                DB = new SQLiteConnection(@"Data Source=ModelsResources/ModelsList.db; Version=3;");
                DB.Open();
                CMD = DB.CreateCommand();

                CMD.CommandText = "select * from Models";

                List<String> nameModels = new List<String>();

                SQLiteDataReader SQL = CMD.ExecuteReader();
                if (SQL.HasRows)
                {
                    while (SQL.Read())
                    {
                        nameModels.Add(SQL["name"].ToString());
                    }
                }
                else
                {
                    statusNameLabel.Text = "Список сохраненых моделей пуст";
                }

                if (nameModels.Count > 0)
                {
                    foreach (String itemName in nameModels)
                    {
                        ModelsTreeView.Nodes[0].Nodes.Add(itemName);
                    }
                    ModelsTreeView.ExpandAll();
                }

                CMD.Dispose();
                DB.Close();
                DB.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            //Добавления пункта меню Создать из файла анимации
            //if (nameSelectItem.errorExprotAnamnation && !nameSelectItem.modelInFileAnamnation)
            if (nameSelectItem.nameSelectMainModel == "Добавить модель из файла анимации" 
                && nameSelectItem.choiseFile //Выбран ли файл анимации
                && nameSelectItem.errorExprotAnamnation //Возникла ли ошибка чтения данных об анимации при загрузке файла
                && !nameSelectItem.blendShapeAnimatDone) //Проверка создавалась ли уже запись об BlendShapes выбранной анимации
            {
                //Делаем пункт "Создать из файла анимации" видимим
                contextMenuStrip.Items[0].Visible = true;
            }
        }

        /*---------------------------------КОНТЕКСТНОЕ МЕНЮ------------------------------------*/
        //Обратботка контекстного меню для treeView
        int selectTreeView = 0;

        bool beginRenameModel = false;
        String pastNameModel = "";

        String pastNameBlendShape = "";

        TreeNode mySelectedNode = new TreeNode("1");

        private void ПереименоватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mySelectedNode != null) //&& mySelectedNode.Parent != null - если выбранный элемент является корневым
            {
                switch (selectTreeView)
                {
                    case 1:
                        beginRenameModel = true;

                        ModelsTreeView.SelectedNode = mySelectedNode;
                        ModelsTreeView.LabelEdit = true; //Включить возможность редактировать элементы в treeView
                        
                        pastNameModel = ModelsTreeView.SelectedNode.Text;

                        ModelsTreeView.SelectedNode.BeginEdit();
                        break;
                    case 2:
                        beginRenameModel = true;

                        BlendShapesTreeView.SelectedNode = mySelectedNode;
                        BlendShapesTreeView.LabelEdit = true; //Включить возможность редактировать элементы в treeView

                        pastNameBlendShape = BlendShapesTreeView.SelectedNode.Text;

                        BlendShapesTreeView.SelectedNode.BeginEdit();
                        break;
                }
            }
            else
            {
                //MessageBox.Show("No tree node selected or selected node is a root node.\n" +
                //   "Editing of root nodes is not allowed.", "Invalid selection");
            }

            LoadDataBlenShapes();
        }

        //Обработка нажатия пункта сконтекстного меню Создать
        private void СоздатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (selectTreeView)
            {
                case 1:
                    AddModel();
                    break;
                case 2:
                    AddBlendShape();
                    break;
            }
        }

        //Обработка нажатия пункта сконтекстного меню Удалить
        private void УдалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (selectTreeView)
            {
                case 1:
                    DeleteModel();
                    break;
                case 2:
                    DeleteBlendShape();
                    break;
            }
        }

        //Обработка нажатия пункта контекстного меню Переместить вверх
        private void ПереместитьВверхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (selectTreeView)
            {
                case 1:
                    MoveUpModel();
                    break;
                case 2:
                    MoveUpBlendShape();
                    break;
            }
        }

        //Обработка нажатия пункта сконтекстного меню Переместить вниз
        private void ВнизToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (selectTreeView)
            {
                case 1:
                    MoveDownModel();
                    break;
                case 2:
                    MoveDownBlendShape();
                    break;
            }
        }

        /*------------------------------------------Обработка нажатие кнопок--------------------------------------------------*/

        //Нажатие кнопки добавить файл(модель)
        private void AddModelButton_Click_1(object sender, EventArgs e)
        {
            AddModel();
        }

        //Нажатие кнопки удалить файл(модель) подкатегории
        private void DeleteModelButton_Click_1(object sender, EventArgs e)
        {
            DeleteModel();
        }

        private void UpModelButton_Click_1(object sender, EventArgs e)
        {
            MoveUpModel();
        }

        private void DownModelButton_Click_1(object sender, EventArgs e)
        {
            MoveDownModel();
        }

        /*--------------------------Нажатие кнопок BlendShapes подкатегории------------------------------*/
        //Нажатие кнопки добвить BlendShape модели
        private void AddBlendShapesButton_Click_1(object sender, EventArgs e)
        {
            AddBlendShape();
        }

        //Нажатие кнопки удалить BlendShape модели
        private void DeleteBlendShapesButton_Click_1(object sender, EventArgs e)
        {
            DeleteBlendShape();
        }

        //Нажатие кнопки вверх BlendShape модели
        private void UpBlendShapesButton_Click_1(object sender, EventArgs e)
        {
            MoveUpBlendShape();
        }

        //Нажатие кнопки вниз BlendShape модели
        private void DownBlendShapesButton_Click_1(object sender, EventArgs e)
        {
            MoveDownBlendShape();
        }


        /*--------------------------------Функции работы с элементами treeView----------------------------------*/
        /*----------------------------------------Файлы категорий(Модели)--------------------------------------*/
        private void ModelsTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            String pastSelectedNodeText = e.Node.Text;
            statusNameLabel.Text = "";

            this.BeginInvoke((MethodInvoker)delegate
            {
                //statusNameLabel.Text = "Успешно";
                //Проверка пустое ли название ввел пользователь
                if (e.Node.Text != "Новая")
                {
                    if (e.Node.Text != "")
                    {
                        if (e.Node.Text[0] != ' ')
                        {
                            //Операции по переименованию сущевствующей записи
                            if (beginRenameModel)
                            {
                                if (pastNameModel != e.Node.Text)
                                {
                                    File.Copy(@"ModelsResources\BlendShapes\" + pastNameModel + ".db",
                                        @"ModelsResources\BlendShapes\" + e.Node.Text + ".db", true);

                                    deleteFiles.Add(@"ModelsResources\BlendShapes\" + pastNameModel + ".db");

                                    nameSelectItem.nameSelectModel = e.Node.Text;

                                    //Запись имени новой модели в БД где хранятся всех модели
                                    DB = new SQLiteConnection(@"Data Source = ModelsResources/ModelsList.db; Version=3;");
                                    DB.Open();
                                    CMD = DB.CreateCommand();

                                    //CMD.CommandText = "INSERT INTO Models('name') VALUES(@newName)";

                                    CMD.CommandText = "UPDATE Models SET name = @newName WHERE name = @pastName";
                                    CMD.Parameters.Add("@newName", System.Data.DbType.String).Value = e.Node.Text; //ModelsTreeView.SelectedNode.Text
                                    CMD.Parameters.Add("@pastName", System.Data.DbType.String).Value = pastNameModel;

                                    CMD.ExecuteNonQuery();

                                    CMD.Dispose();
                                    DB.Close();
                                    DB.Dispose();

                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();

                                    pastNameModel = "";
                                    beginRenameModel = false;

                                    LoadDataBlenShapes();
                                    FormApplication.updateBaseData.Checked = true;
                                }
                                else 
                                {
                                    pastNameModel = "";
                                    beginRenameModel = false;
                                }
                            }
                            //Операции по созданию записей
                            else 
                            {
                                //Копирование записи о моделе было начато
                                if (copyModelBegin)
                                {
                                    //Копироание файла записи о моделе
                                    File.Copy(@"ModelsResources\BlendShapes\" + prevSelectNode.Text + ".db",
                                        @"ModelsResources\BlendShapes\" + prevSelectNode.Text + "(copy).db");

                                    copyModelBegin = false;
                                }
                                else 
                                {
                                    //Проверка есть ли ошибка несоотвествия BlendShapes загруженной анимации и выбранной модели
                                    if (!nameSelectItem.errorExprotAnamnation)
                                    {
                                        //Запись имени новой модели в БД где хранятся всех модели
                                        DB = new SQLiteConnection(@"Data Source = ModelsResources/ModelsList.db; Version=3;");
                                        DB.Open();
                                        CMD = DB.CreateCommand();

                                        CMD.CommandText = "insert into Models(name) values(@nameModel)";
                                        CMD.Parameters.Add("@nameModel", System.Data.DbType.String).Value = e.Node.Text;

                                        try
                                        {
                                            CMD.ExecuteNonQuery();

                                            CMD.Dispose();
                                            DB.Close();
                                            DB.Dispose();

                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();

                                            //Создание файла базы данных новой категории
                                            SQLiteConnection.CreateFile(@"ModelsResources\BlendShapes\" + e.Node.Text + ".db");

                                            try
                                            {
                                                DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + e.Node.Text + ".db; Version=3;");
                                                DB.Open();
                                                CMD = DB.CreateCommand();

                                                CMD.CommandText = "CREATE TABLE BlendShapes (№ INTEGER PRIMARY KEY AUTOINCREMENT, name STRING NOT NULL UNIQUE, type INTEGER NOT NULL, nameSFM STRING  NOT NULL, nameEditorLeft  STRING NOT NULL, nameEditorRight STRING  UNIQUE);";
                                                CMD.ExecuteNonQuery();

                                                CMD.Dispose();
                                                DB.Close();
                                                DB.Dispose();

                                                GC.Collect();
                                                GC.WaitForPendingFinalizers();

                                                //Загрузка в BlendShapeTreeView данных о BlendShapes выбранной модели
                                                LoadDataBlenShapes();

                                                //Были внесенны новые данные, следует обновить ListBox в главном окне
                                                FormApplication.updateBaseData.Checked = true;
                                            }
                                            catch (SQLiteException ex)
                                            {
                                                CMD.Dispose();
                                                DB.Close();
                                                DB.Dispose();

                                                GC.Collect();
                                                GC.WaitForPendingFinalizers();
                                                MessageBox.Show("Error: " + ex.Message);
                                            }
                                        }
                                        catch (System.Data.SQLite.SQLiteException ex)
                                        {
                                            if (ex.Message == "constraint failed\r\nUNIQUE constraint failed: Models.name")
                                            {
                                                statusNameLabel.Text = "Модель " + e.Node.Text + " уже сущевствует";
                                                ModelsTreeView.Nodes[0].Nodes.Remove(ModelsTreeView.Nodes[0].LastNode);
                                                /*if (mySelectNode.Text == ModelsTreeView.SelectedNode.Text)
                                                {
                                                    statusNameLabel.Text = "";
                                                }
                                                else
                                                {
                                                    statusNameLabel.Text = "Модель " + e.Node.Text + " уже сущевствует";
                                                }*/
                                            }
                                        }

                                        CMD.Dispose();
                                        DB.Dispose();

                                        GC.Collect();
                                        GC.WaitForPendingFinalizers();
                                    }
                                    //Создание записи о модели из файла анимации
                                    else
                                    {
                                        //Запись новой модели из фала анимации в БД
                                        DB = new SQLiteConnection(@"Data Source = ModelsResources/ModelsList.db; Version=3;");
                                        DB.Open();
                                        CMD = DB.CreateCommand();

                                        CMD.CommandText = "insert into Models(name) values(@nameModel)";
                                        CMD.Parameters.Add("@nameModel", System.Data.DbType.String).Value = e.Node.Text;

                                        try
                                        {
                                            CMD.ExecuteNonQuery();

                                            CMD.Dispose();
                                            DB.Close();
                                            DB.Dispose();

                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();

                                            //Создание файла БД о новой моделе
                                            SQLiteConnection.CreateFile(@"ModelsResources\BlendShapes\" + e.Node.Text + ".db");
                                            
                                            //Запоминаем имя записи созданной из фала анимации
                                            nameSelectItem.nameModelInAnimation = e.Node.Text;

                                            try
                                            {
                                                DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + e.Node.Text + ".db; Version=3;");
                                                DB.Open();
                                                CMD = DB.CreateCommand();

                                                CMD.CommandText = "CREATE TABLE BlendShapes (№ INTEGER PRIMARY KEY AUTOINCREMENT, name STRING NOT NULL UNIQUE, type INTEGER NOT NULL, nameSFM STRING  NOT NULL, nameEditorLeft  STRING NOT NULL, nameEditorRight STRING  UNIQUE);";
                                                CMD.ExecuteNonQuery();

                                                //Была создана запись об BlendShapes для выбранного файла анимации 
                                                nameSelectItem.blendShapeAnimatDone = true;

                                                CMD.Dispose();
                                                DB.Close();
                                                DB.Dispose();

                                                GC.Collect();
                                                GC.WaitForPendingFinalizers();

                                                //Загрузка в BlendShapeTreeView данных о BlendShapes выбранной модели
                                                LoadDataBlenShapes();

                                                //Были внесенны новые данные, следует обновить ListBox в главном окне
                                                FormApplication.updateBaseData.Checked = true;
                                            }
                                            catch (SQLiteException ex)
                                            {

                                                if (ex.Message == "constraint failed\r\nUNIQUE constraint failed: Models.name")
                                                {
                                                    statusNameLabel.Text = "Модель " + e.Node.Text + " уже сущевствует";
                                                    ModelsTreeView.Nodes[0].Nodes.Remove(ModelsTreeView.Nodes[0].LastNode);
                                                }
                                            }


                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();

                                            DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + e.Node.Text + ".db; Version=3;");

                                            DB.Open();
                                            CMD = DB.CreateCommand();

                                            BlendShapesTreeView.Nodes[0].Nodes.Clear();

                                            foreach (BlenShapeNoteAnimation item in nameSelectItem.listBlendShapeAnimation)
                                            {
                                                CMD.CommandText = "INSERT INTO BlendShapes('name', 'type', 'nameSFM', 'nameEditorLeft') " +
                                                    "VALUES(@name, @type, @nameSFM, @nameEditorLeft)";
                                                CMD.Parameters.Add("@name", System.Data.DbType.String).Value = item.name;
                                                CMD.Parameters.Add("@type", System.Data.DbType.Int32).Value = 0;
                                                CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = item.name;
                                                CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = item.name; //"Ввежите новое значение"

                                                try
                                                {
                                                    CMD.ExecuteNonQuery();
                                                    BlendShapesTreeView.Nodes[0].Nodes.Add(item.name);
                                                }

                                                catch (System.Data.SQLite.SQLiteException ex)
                                                {
                                                    CMD.Dispose();
                                                    DB.Close();
                                                    DB.Dispose();

                                                    GC.Collect();
                                                    GC.WaitForPendingFinalizers();

                                                    if (ex.Message == "constraint failed\r\nUNIQUE constraint failed: BlendShapes.name")
                                                    {
                                                        statusNameLabel.Text = "Введенное имя совпадает с уже существующим";
                                                        //DialogResult result = MessageBox.Show("Введенное имя совпадает с уже существующим",
                                                        //    "Ошибка ввода имени BlendShape", MessageBoxButtons.OK);
                                                    }
                                                }
                                            }

                                            CMD.Dispose();
                                            DB.Close();
                                            DB.Dispose();

                                            GC.Collect();
                                            GC.WaitForPendingFinalizers();

                                            BlendShapesTreeView.ExpandAll();
                                            //Сбрасываем флаг ошибки несооствествия BlendShapes выбранной модели и загруженной анимации
                                            nameSelectItem.errorExprotAnamnation = false;

                                            //Была создана запись о моделе из фала анимации
                                            nameSelectItem.modelInFileAnamnation = true;

                                            //Делаем пункт контесктного меню Создать из файла анимации невидимим
                                            contextMenuStrip.Items[0].Visible = false;

                                            //Были внесенны новые данные, следует обновить ListBox в главном окне программы
                                            FormApplication.updateBaseBlendShapesData.Checked = true;
                                        }
                                        catch (System.Data.SQLite.SQLiteException ex)
                                        {
                                            if (ex.Message == "constraint failed\r\nUNIQUE constraint failed: Models.name")
                                            {
                                                statusNameLabel.Text = "Модель " + e.Node.Text + " уже сущевствует";
                                                ModelsTreeView.Nodes[0].Nodes.Remove(ModelsTreeView.Nodes[0].LastNode);
                                            }
                                        }
                                    }
                                }
                            } 
                        }
                        else
                        {
                            statusNameLabel.Text = "Введеноное имя начинается с пробела";
                            ModelsTreeView.LabelEdit = true;
                            ModelsTreeView.SelectedNode.Text = pastSelectedNodeText;
                            ModelsTreeView.SelectedNode.BeginEdit();
                        }
                    }
                    else
                    {
                        statusNameLabel.Text = "Введено пустое имя";
                        ModelsTreeView.LabelEdit = true;
                        ModelsTreeView.SelectedNode.Text = pastSelectedNodeText;
                        ModelsTreeView.SelectedNode.BeginEdit();
                    }
                }
                else
                {
                    statusNameLabel.Text = "Введите уникальное имя модели";
                    ModelsTreeView.LabelEdit = true;
                    ModelsTreeView.SelectedNode.Text = pastSelectedNodeText;
                    ModelsTreeView.Nodes[0].LastNode.Remove();
                }
            });
        }

        public void AddModel()
        {
            TreeNode newNode = new TreeNode("Новая");
            ModelsTreeView.Nodes[0].Nodes.Add(newNode);
            ModelsTreeView.LabelEdit = true;
            ModelsTreeView.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        public void DeleteBlendShape()
        {
            if (BlendShapesTreeView.SelectedNode != null && BlendShapesTreeView.SelectedNode != ModelsTreeView.Nodes[0])
            {
                TreeNode prevedNode = BlendShapesTreeView.SelectedNode.PrevNode;
                int indexPrevedNode = 0;
                if (prevedNode != null) { indexPrevedNode = BlendShapesTreeView.SelectedNode.PrevNode.Index; }

                string caption = "Удаление BlendShape";
                string message = "Вы уверены, что хотите удалить запись о BlendShape - " + BlendShapesTreeView.SelectedNode.Text + "?";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    //Запись имени новой модели в БД где хранятся всех модели
                    DB = new SQLiteConnection("Data Source=ModelsResources/BlendShapes/" + ModelsTreeView.SelectedNode.Text + ".db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "DELETE FROM BlendShapes WHERE name = '" + BlendShapesTreeView.SelectedNode.Text + "'";
                    CMD.ExecuteNonQuery();

                    CMD.CommandText = "DELETE FROM sqlite_sequence WHERE name = 'BlendShapes'";
                    CMD.ExecuteNonQuery();

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    statusNameLabel.Text = "Запись о BlenShape - " + BlendShapesTreeView.SelectedNode.Text + " успешно удалена";

                    LoadDataBlenShapes();

                    //Были внесенны новые данные, следует обновить ListBox в главном окне
                    updateBaseDataEditor.Checked = true;
                    FormApplication.updateBaseBlendShapesData.Checked = true;
                    FormApplication.updateBaseData.Checked = true;

                    if (BlendShapesTreeView.Nodes[0].Nodes.Count > 0) 
                    {
                        if (prevedNode == null)
                        {
                            BlendShapesTreeView.SelectedNode = BlendShapesTreeView.Nodes[0].Nodes[0];
                        }
                        else 
                        {
                            BlendShapesTreeView.SelectedNode = BlendShapesTreeView.Nodes[0].Nodes[indexPrevedNode]; ;
                        }
                    }
                }
            }

        }

        public void MoveDownModel()
        {
            if (ModelsTreeView.SelectedNode != null)
            {
                TreeNode node = ModelsTreeView.SelectedNode;
                TreeNode parent = node.Parent;

                if (parent != null)
                {
                    int index = parent.Nodes.IndexOf(node);
                    if (index < parent.Nodes.Count - 1)
                    {
                        parent.Nodes.RemoveAt(index);
                        parent.Nodes.Insert(index + 1, node);

                        // bw : add this line to restore the originally selected node as selected
                        node.TreeView.SelectedNode = node;
                    }

                    //Обновлении записи в БД
                    DB = new SQLiteConnection("Data Source =" + @"ModelsResources/ModelsList.db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "delete from Models";
                    CMD.ExecuteNonQuery();

                    CMD.CommandText = "delete from sqlite_sequence where name = 'Models'";
                    CMD.ExecuteNonQuery();

                    foreach (TreeNode item in ModelsTreeView.Nodes[0].Nodes)
                    {
                        CMD.CommandText = "insert into Models(name) values(@nameModel)";
                        CMD.Parameters.Add("@nameModel", System.Data.DbType.String).Value = item.Text;
                        CMD.ExecuteNonQuery();
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers(); ;

                    //Были внесенны новые данные, следует обновить ListBox в главном окне
                    FormApplication.updateBaseData.Checked = true;
                }
            }
        }

        private void ModelsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ModelsTreeView.SelectedNode = e.Node;
                selectTreeView = 1;
            }
            if (e.Button == MouseButtons.Left) 
            { 
                selectTreeView = 1; 
            }

            /*if (e.Node == null)
            {
                copyModelButton.Enabled = false;
                deleteModelButton.Enabled = false;
                upModelButton.Enabled = false;
                upModelButton.Enabled = false;
            }
            else
            {
                copyModelButton.Enabled = true;
                deleteModelButton.Enabled = true;
                upModelButton.Enabled = true;
                upModelButton.Enabled = true;

                statusNameLabel.Text = " ";
            }*/
        }

        TreeNode mySelectNode;
        TreeNode prevSelectNode = new TreeNode();

        private void ModelsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (ModelsTreeView.SelectedNode.Parent != null) 
            { 
                copyModelButton.Enabled = true;
                deleteModelButton.Enabled = true;
            }
            else 
            { 
                copyModelButton.Enabled = false;
                deleteModelButton.Enabled = true;
            }

            if (ModelsTreeView.SelectedNode != null)
            {
                //Если выбрана последняя запись BlendShape, то убираем доступность кнопок перемещения записи вниз
                if (ModelsTreeView.SelectedNode == ModelsTreeView.Nodes[0].LastNode)
                {
                    downModelButton.Enabled = false;
                    contextMenuStrip.Items[7].Enabled = false;
                }
                else
                {
                    downModelButton.Enabled = true;
                    contextMenuStrip.Items[7].Enabled = true;
                }

                if (ModelsTreeView.SelectedNode == ModelsTreeView.Nodes[0].FirstNode)
                {
                    upModelButton.Enabled = false;
                    contextMenuStrip.Items[6].Enabled = false;
                }
                else
                {
                    upModelButton.Enabled = true;
                    contextMenuStrip.Items[6].Enabled = true;
                }

                addBlendShapesButton.Enabled = true;
            }
            else 
            {
                addBlendShapesButton.Enabled = false;
            }

            prevSelectNode = mySelectNode;
            mySelectNode = ModelsTreeView.SelectedNode;

            nameSelectItem.nameSelectModel = ModelsTreeView.SelectedNode.Text;
            nameSelectItem.nameSelectBlendShape = "";

            LoadDataBlenShapes();
        }


        private void BlendShapesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (BlendShapesTreeView.SelectedNode.Parent != null)
            {
                changeBlendShapesButton.Enabled = true;
                deleteBlendShapesButton.Enabled = true;
            }
            else
            {
                changeBlendShapesButton.Enabled = false;
                deleteBlendShapesButton.Enabled = true;
            }

            if (BlendShapesTreeView.SelectedNode != null)
            {
                //Если выбрана последняя запись BlendShape, то убираем доступность кнопок перемещения записи вниз
                if (BlendShapesTreeView.SelectedNode == BlendShapesTreeView.Nodes[0].LastNode)
                {
                    downBlendShapesButton.Enabled = false;
                    contextMenuStrip.Items[7].Enabled = false;
                }
                else
                {
                    downBlendShapesButton.Enabled = true;
                    contextMenuStrip.Items[7].Enabled = true;
                }

                if (BlendShapesTreeView.SelectedNode == BlendShapesTreeView.Nodes[0].FirstNode)
                {
                    upBlendShapesButton.Enabled = false;
                    contextMenuStrip.Items[6].Enabled = false;
                }
                else
                {
                    upBlendShapesButton.Enabled = true;
                    contextMenuStrip.Items[6].Enabled = true;
                }

                nameSelectItem.nameSelectBlendShape = BlendShapesTreeView.SelectedNode.Text;
            }

            //this.Text = BlendShapesTreeView.SelectedNode.Text;
            mySelectNode = BlendShapesTreeView.SelectedNode;
        }

        /*----------------------------------------BlendShapes--------------------------------------*/
                private void BlendShapesTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            String pastSelectedNodeText = e.Node.Text;
            statusNameLabel.Text = "";
        }

        private void changeBlendShapesButton_Click(object sender, EventArgs e)
        {
            UpdateBlenShape();
        }

        public void UpdateBlenShape()
        {
            if (BlendShapesTreeView.Nodes[0].Nodes.Count > 0) 
            {

                //Обновлении записи в БД
                DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + ModelsTreeView.SelectedNode.Text + ".db; Version = 3;");
                DB.Open();
                CMD = DB.CreateCommand();

                //CMD.CommandText = "select * from BlendShapes";
                CMD.CommandText = "SELECT* FROM BlendShapes WHERE name LIKE @nameBlendShape";
                CMD.Parameters.Add("@nameBlendShape", System.Data.DbType.String).Value = BlendShapesTreeView.SelectedNode.Text;
                CMD.ExecuteNonQuery();

                nameSelectItem.selectBlendShape = null;

                SQLiteDataReader SQL = CMD.ExecuteReader();

                if (SQL.HasRows)
                {
                    while (SQL.Read())
                    {
                        nameSelectItem.selectBlendShape = new BlenShapeNote(SQL["name"].ToString(), Convert.ToInt32(SQL["type"].ToString()),
                            SQL["nameSFM"].ToString(), SQL["nameEditorLeft"].ToString(), SQL["nameEditorRight"].ToString());
                    }
                }

                CMD.Dispose();
                DB.Close();
                DB.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                nameSelectItem.nameSelectBlendShape = BlendShapesTreeView.SelectedNode.Text;
                //nameSelectItem.nameSelectModel = ModelsTreeView.SelectedNode.Text;

                nameSelectItem.typeInitWindows = 1;

                CreateBlendShape createBlendShape = new CreateBlendShape();
                createBlendShape.Text = "Редактирование BlendShape - " + nameSelectItem.nameSelectBlendShape;

                if (nameSelectItem.selectBlendShape.type == 1)
                { 
                    createBlendShape.Size = new Size(552, 460); 
                }

                createBlendShape.ShowDialog();
            }
        }

        public void AddBlendShape()
        {
            nameSelectItem.nameSelectModel = ModelsTreeView.SelectedNode.Text;
            nameSelectItem.typeInitWindows = 0;

            CreateBlendShape createBlendShape = new CreateBlendShape();
            createBlendShape.Text = "Добавление нового BlendShape";

            createBlendShape.ShowDialog();
        }

        public void DeleteModel()
        {
            if (ModelsTreeView.SelectedNode != null && ModelsTreeView.SelectedNode != ModelsTreeView.Nodes[0])
            {

                string caption = "Удаление модели";
                string message = "Вы уверены, что хотите удалить запись о моделе " + ModelsTreeView.SelectedNode.Text +
                    ". Вместе с ней удаляться все записи о ее BlendShapes?";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    
                    //Запись имени новой модели в БД где хранятся всех модели
                    DB = new SQLiteConnection("Data Source=ModelsResources/ModelsList.db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "DELETE FROM Models WHERE name = '" + ModelsTreeView.SelectedNode.Text + "';";
                    CMD.ExecuteNonQuery();

                    CMD.CommandText = "DELETE FROM sqlite_sequence WHERE name = 'Models'";
                    CMD.ExecuteNonQuery();

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    statusNameLabel.Text = "Запись о моделе " + ModelsTreeView.SelectedNode.Text + " успешно удалена";

                    //Добавляем удаляймую запись в список для удаления
                    CMD.Dispose();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    File.Delete(@"ModelsResources\BlendShapes\" + ModelsTreeView.SelectedNode.Text + ".db");

                    foreach (TreeNode item in ModelsTreeView.Nodes[0].Nodes)
                    {
                        if (item == ModelsTreeView.SelectedNode)
                        {
                            ModelsTreeView.Nodes[0].Nodes.Remove(item);
                        }
                    }

                    LoadDataBlenShapes();

                    //Были внесенны новые данные, следует обновить ListBox в главном окне
                    FormApplication.updateBaseData.Checked = true;
                }
            }
        }

        public void MoveUpBlendShape()
        {
            if (BlendShapesTreeView.SelectedNode != null)
            {
                TreeNode node = BlendShapesTreeView.SelectedNode;
                TreeNode parent = node.Parent;

                if (parent != null)
                {
                    //Обновлении записи в БД
                    DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + ModelsTreeView.SelectedNode.Text + ".db; Version = 3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "select * from BlendShapes";

                    List<BlenShapeNote> listBlendShapes = new List<BlenShapeNote>();

                    SQLiteDataReader SQL = CMD.ExecuteReader();

                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            listBlendShapes.Add(new BlenShapeNote(SQL["name"].ToString(), Convert.ToInt32(SQL["type"].ToString()),
                                SQL["nameSFM"].ToString(), SQL["nameEditorLeft"].ToString(), SQL["nameEditorRight"].ToString()));
                        }
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + ModelsTreeView.SelectedNode.Text + ".db; Version = 3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "delete from BlendShapes";
                    CMD.ExecuteNonQuery();

                    CMD.CommandText = "delete from sqlite_sequence where name = 'BlendShapes'";
                    CMD.ExecuteNonQuery();

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    BlenShapeNote prevNote;
                    int nodeIndex = 0;
                    while (nodeIndex <= listBlendShapes.Count)
                    {
                        if (listBlendShapes[nodeIndex].name == BlendShapesTreeView.SelectedNode.Text)
                        {
                            prevNote = listBlendShapes[nodeIndex - 1];
                            listBlendShapes[nodeIndex - 1] = listBlendShapes[nodeIndex];
                            listBlendShapes[nodeIndex] = prevNote;

                            break;
                        }
                        else
                        {
                            nodeIndex++;
                        }
                    }

                    BlendShapesTreeView.Nodes[0].Nodes.Clear();
                    DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + ModelsTreeView.SelectedNode.Text + ".db; Version = 3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    foreach (BlenShapeNote item in listBlendShapes)
                    {
                        BlendShapesTreeView.Nodes[0].Nodes.Add(item.name);

                        try
                        {
                            if (item.type == 0)
                            {
                                CMD.CommandText = "INSERT INTO BlendShapes('name', 'type', 'nameSFM', 'nameEditorLeft') " +
                                    "VALUES(@name, @type, @nameSFM, @nameEditorLeft)";
                                CMD.Parameters.Add("@name", System.Data.DbType.String).Value = item.name;
                                CMD.Parameters.Add("@type", System.Data.DbType.Int32).Value = item.type;
                                CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = item.nameSFM;
                                CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = item.nameEditorLeft;
                            }
                            else
                            {
                                CMD.CommandText = "INSERT INTO BlendShapes('name', 'type', 'nameSFM', 'nameEditorLeft', 'nameEditorRight') " +
                                    "VALUES(@name, @type, @nameSFM, @nameEditorLeft, @nameEditorRight)";
                                CMD.Parameters.Add("@name", System.Data.DbType.String).Value = item.name;
                                CMD.Parameters.Add("@type", System.Data.DbType.Int32).Value = item.type;
                                CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = item.nameSFM;
                                CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = item.nameEditorLeft;
                                CMD.Parameters.Add("@nameEditorRight", System.Data.DbType.String).Value = item.nameEditorRight;
                            }

                            CMD.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    //Были внесенны новые данные, следует обновить ListBox в главном окне
                    //LoadDataBlenShapes();

                    BlendShapesTreeView.SelectedNode = BlendShapesTreeView.Nodes[0].Nodes[nodeIndex - 1];
                    BlendShapesTreeView.ExpandAll();

                    FormApplication.updateBaseBlendShapesData.Checked = true;
                }
            }
        }

        public void MoveDownBlendShape()
        {
            if (BlendShapesTreeView.SelectedNode != null)
            {
                TreeNode node = BlendShapesTreeView.SelectedNode;
                TreeNode parent = node.Parent;

                if (parent != null)
                {
                    //Обновлении записи в БД
                    DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + ModelsTreeView.SelectedNode.Text + ".db; Version = 3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "select * from BlendShapes";

                    List<BlenShapeNote> listBlendShapes = new List<BlenShapeNote>();

                    SQLiteDataReader SQL = CMD.ExecuteReader();

                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            listBlendShapes.Add(new BlenShapeNote(SQL["name"].ToString(), Convert.ToInt32(SQL["type"].ToString()),
                                SQL["nameSFM"].ToString(), SQL["nameEditorLeft"].ToString(), SQL["nameEditorRight"].ToString()));
                        }
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    DB = new SQLiteConnection(@"Data Source = ModelsResources/BlendShapes/" + ModelsTreeView.SelectedNode.Text + ".db; Version = 3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "delete from BlendShapes";
                    CMD.ExecuteNonQuery();

                    CMD.CommandText = "delete from sqlite_sequence where name = 'BlendShapes'";
                    CMD.ExecuteNonQuery();

                    BlenShapeNote prevNote;
                    int nodeIndex = 0;
                    while (nodeIndex <= listBlendShapes.Count)
                    {

                        if (listBlendShapes[nodeIndex].name == BlendShapesTreeView.SelectedNode.Text)
                        {
                            prevNote = listBlendShapes[nodeIndex + 1];
                            listBlendShapes[nodeIndex + 1] = listBlendShapes[nodeIndex];
                            listBlendShapes[nodeIndex] = prevNote;

                            break;
                        }
                        else
                        {
                            nodeIndex++;
                        }
                    }

                    BlendShapesTreeView.Nodes[0].Nodes.Clear();

                    foreach (BlenShapeNote item in listBlendShapes)
                    {
                        BlendShapesTreeView.Nodes[0].Nodes.Add(item.name);

                        try
                        {
                            if (item.type == 0)
                            {
                                CMD.CommandText = "INSERT INTO BlendShapes('name', 'type', 'nameSFM', 'nameEditorLeft') " +
                                    "VALUES(@name, @type, @nameSFM, @nameEditorLeft)";
                                CMD.Parameters.Add("@name", System.Data.DbType.String).Value = item.name;
                                CMD.Parameters.Add("@type", System.Data.DbType.Int32).Value = item.type;
                                CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = item.nameSFM;
                                CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = item.nameEditorLeft;
                            }
                            else
                            {
                                CMD.CommandText = "INSERT INTO BlendShapes('name', 'type', 'nameSFM', 'nameEditorLeft', 'nameEditorRight') " +
                                    "VALUES(@name, @type, @nameSFM, @nameEditorLeft, @nameEditorRight)";
                                CMD.Parameters.Add("@name", System.Data.DbType.String).Value = item.name;
                                CMD.Parameters.Add("@type", System.Data.DbType.Int32).Value = item.type;
                                CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = item.nameSFM;
                                CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = item.nameEditorLeft;
                                CMD.Parameters.Add("@nameEditorRight", System.Data.DbType.String).Value = item.nameEditorRight;
                            }

                            CMD.ExecuteNonQuery();
                        }
                        catch (SQLiteException ex)
                        {
                            CMD.Dispose();
                            DB.Close();
                            DB.Dispose();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    //Были внесенны новые данные, следует обновить ListBox в главном окне
                    LoadDataBlenShapes();

                    BlendShapesTreeView.SelectedNode = BlendShapesTreeView.Nodes[0].Nodes[nodeIndex + 1];

                    //Были внесенны новые данные, следует обновить ListBox в главном окне
                    FormApplication.updateBaseBlendShapesData.Checked = true;
                }
            }
        }

        private void BlendShapesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                BlendShapesTreeView.SelectedNode = e.Node;
                selectTreeView = 2;
            }

            if (e.Button == MouseButtons.Left) 
            { 
                selectTreeView = 2; 
            }

            /*if (BlendShapesTreeView.SelectedNode == null) 
            {
                changeBlendShapesButton.Enabled = false;
                deleteBlendShapesButton.Enabled = false;
                upBlendShapesButton.Enabled = false;
                downBlendShapesButton.Enabled = false;
            } 
            else 
            {
                changeBlendShapesButton.Enabled = true;
                deleteBlendShapesButton.Enabled = true;

                nameSelectItem.nameSelectBlendShape = BlendShapesTreeView.SelectedNode.Text;
                nameSelectItem.indexSelectBlendShape = BlendShapesTreeView.SelectedNode.Index;
            }*/
        }

        private void AddBlendShapesButton_Click(object sender, EventArgs e)
        {
            AddBlendShape();
        }

        private void ModelsTreeView_Click(object sender, EventArgs e)
        {
            statusNameLabel.Text = ""; //Сюрасывает текуший статус при клике
        }

        public void MoveUpModel()
        {
            if (ModelsTreeView.SelectedNode != null)
            {
                TreeNode node = ModelsTreeView.SelectedNode;
                TreeNode parent = node.Parent;

                if (parent != null)
                {
                    int index = parent.Nodes.IndexOf(node);
                    if (index > 0)
                    {
                        parent.Nodes.RemoveAt(index);
                        parent.Nodes.Insert(index - 1, node);

                        // bw : add this line to restore the originally selected node as selected
                        node.TreeView.SelectedNode = node;
                    }

                    //Обновлении записи в БД
                    DB = new SQLiteConnection("Data Source =" + @"ModelsResources/ModelsList.db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "delete from Models";
                    CMD.ExecuteNonQuery();

                    CMD.CommandText = "delete from sqlite_sequence where name = 'Models'";
                    CMD.ExecuteNonQuery();

                    foreach (TreeNode item in ModelsTreeView.Nodes[0].Nodes)
                    {
                        CMD.CommandText = "insert into Models(name) values(@nameModel)";
                        CMD.Parameters.Add("@nameModel", System.Data.DbType.String).Value = item.Text;
                        CMD.ExecuteNonQuery();
                    }

                    CMD.Dispose();
                    DB.Close();
                    DB.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    //Были внесенны новые данные, следует обновить ListBox в главном окне
                    FormApplication.updateBaseData.Checked = true;
                }
            }
        }

        private void upBlendShapesButton_Click(object sender, EventArgs e)
        {
            MoveUpBlendShape();
        }

        private void downBlendShapesButton_Click(object sender, EventArgs e)
        {
            MoveDownBlendShape();
        }

        private void deleteBlendShapesButton_Click(object sender, EventArgs e)
        {
            DeleteBlendShape();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateBlenShape();
        }

        private void BlendShapesTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Text.Contains("BlendShapes"))
            {
                e.Cancel = true;
            }
        }

        private void ModelsTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            selectTreeView = 1;
        }

        private void BlendShapesTreeView_MouseMove(object sender, MouseEventArgs e)
        {
            selectTreeView = 2;
            /*if (BlendShapesTreeView.SelectedNode == null)
            {
                copyModelButton.Enabled = false;
                changeBlendShapesButton.Enabled = false;
                deleteBlendShapesButton.Enabled = false;
                //upBlendShapesButton.Enabled = false;
                //downBlendShapesButton.Enabled = false;
            }
            else
            {
                copyModelButton.Enabled = false;
                //changeBlendShapesButton.Enabled = true;
                //deleteBlendShapesButton.Enabled = true;
            }*/
        }

        private void BlendShapesTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModelsTreeView.SelectedNode == null)
            {
                BlendShapesTreeView.ContextMenuStrip = null;
            }
            else 
            {
                BlendShapesTreeView.ContextMenuStrip = contextMenuStrip;
            }
        }

        private void EditorModels_Shown(object sender, EventArgs e)
        {
            /*-----------Изменениек палитры цветов----------*/
            this.BackColor = InterfaceWork.backgroundFormColor;
            panelBorderWindow.BackColor = InterfaceWork.backgroundFormColor;
            statusStrip.BackColor = InterfaceWork.backgroundFormColor;
            menuStrip.BackColor = InterfaceWork.backgroundFormColor;

            ModelsTreeView.ForeColor = InterfaceWork.whiteColor;
            ModelsTreeView.BackColor = InterfaceWork.backgroundTextColor;

            BlendShapesTreeView.ForeColor = InterfaceWork.whiteColor;
            BlendShapesTreeView.BackColor = InterfaceWork.backgroundTextColor;

            statusNameLabel.ForeColor = InterfaceWork.whiteColorPush;

            //Меню
            for (int i = 0; i < menuStrip.Items.Count; i++)
            {
                menuStrip.Items[i].ForeColor = InterfaceWork.whiteColor;
            }

            for (int i = 0; i < contextMenuStrip.Items.Count; i++)
            {
                contextMenuStrip.Items[i].ForeColor = InterfaceWork.whiteColor;
            }
        }

        private void EditorModels_FormClosed(object sender, FormClosedEventArgs e)
        {
            CMD.Dispose();
            DB.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (deleteFiles.Count > 0) 
            {
                foreach (String item in deleteFiles) 
                {
                    File.Delete(item);
                }
            }
        }

        private void ModelsTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (ModelsTreeView.SelectedNode != null) 
            {
                if (e.KeyCode == Keys.F2)
                {
                    ModelsTreeView.LabelEdit = true;
                    ModelsTreeView.SelectedNode.BeginEdit();
                }

                if (e.KeyCode == Keys.M)
                {
                    AddModel();
                }

                if (e.KeyCode == Keys.Delete)
                {
                    DeleteModel();
                }
            }
        }

        private void BlendShapesTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (BlendShapesTreeView.SelectedNode != null)
            {
                if (e.KeyCode == Keys.F2)
                {
                    UpdateBlenShape();
                }

                if (e.KeyCode == Keys.Delete) 
                {
                    DeleteBlendShape();
                }

                if (e.KeyCode == Keys.B)
                {
                    AddBlendShape();
                }
            }
        }

        bool copyModelBegin = false;
        public void copyModel() 
        {
            if (copyModelButton.Enabled == true) 
            {
                TreeNode newNode = new TreeNode(ModelsTreeView.SelectedNode.Text + "(copy)");
                ModelsTreeView.Nodes[0].Nodes.Add(newNode);
                ModelsTreeView.LabelEdit = true;
                ModelsTreeView.SelectedNode = newNode;
                copyModelBegin = true;

                newNode.BeginEdit();
            }
        }

        private void copyModelButton_Click(object sender, EventArgs e)
        {
            copyModel();
        }

        private void дублироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyModel();
        }

        //Создание записи о модели из файла анимации
        private void создатьИзФалаАнимацииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode newNode = new TreeNode("Новая");
            ModelsTreeView.Nodes[0].Nodes.Add(newNode);
            ModelsTreeView.LabelEdit = true;
            ModelsTreeView.SelectedNode = newNode;
            newNode.BeginEdit();
        }

        /*---------Изменение размеров TreeView при растягивание (в роли рамки по бокам выступает ListBox)--------*/
        private void borderWhiteModelListBox_Resize(object sender, EventArgs e)
        {
            ModelsTreeView.Size = new Size(borderWhiteModelListBox.Size.Width - 5, 
                borderWhiteModelListBox.Size.Height - 5); ;
        }

        private void borderWhiteBlendShapeListBox_Resize(object sender, EventArgs e)
        {
            BlendShapesTreeView.Size = new Size(borderWhiteBlendShapeListBox.Size.Width - 5,
                borderWhiteBlendShapeListBox.Size.Height - 5);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (contextMenuStrip.SourceControl == ModelsTreeView)
            {
                this.Text = "Вызван из ModelsTreeView"; 

                if (selectTreeView == 1)
                {
                    if (ModelsTreeView.SelectedNode == null)
                    {
                        contextMenuStrip.Items[1].Visible = true;
                        contextMenuStrip.Items[3].Visible = true;
                        contextMenuStrip.Items[4].Visible = true;
                        contextMenuStrip.Items[5].Visible = true;

                        contextMenuStrip.Items[2].Visible = false;

                        contextMenuStrip.Items[1].Enabled = true;
                        contextMenuStrip.Items[3].Enabled = false;
                        contextMenuStrip.Items[4].Enabled = false;
                        contextMenuStrip.Items[5].Enabled = false;
                    }
                    else 
                    {
                        contextMenuStrip.Items[1].Visible = true;
                        contextMenuStrip.Items[3].Visible = true;
                        contextMenuStrip.Items[4].Visible = true;
                        contextMenuStrip.Items[5].Visible = true;

                        contextMenuStrip.Items[2].Visible = false;

                        contextMenuStrip.Items[1].Enabled = true;
                        contextMenuStrip.Items[3].Enabled = true;
                        contextMenuStrip.Items[4].Enabled = true;
                        contextMenuStrip.Items[5].Enabled = true;

                        contextMenuStrip.Items[2].Enabled = true;
                    }
                }
            }

            if (contextMenuStrip.SourceControl == BlendShapesTreeView)
            {
                this.Text = "Вызван из BlendShapesTreeView";

                if (selectTreeView == 2)
                {
                    if (BlendShapesTreeView.SelectedNode == null)
                    {
                        contextMenuStrip.Items[1].Visible = true;
                        contextMenuStrip.Items[2].Visible = true;
                        contextMenuStrip.Items[5].Visible = true;

                        contextMenuStrip.Items[3].Visible = false;
                        contextMenuStrip.Items[4].Visible = false;

                        contextMenuStrip.Items[1].Enabled = true;
                        contextMenuStrip.Items[2].Enabled = false;
                        contextMenuStrip.Items[4].Enabled = false;
                        contextMenuStrip.Items[5].Enabled = false;

                        contextMenuStrip.Items[6].Enabled = false;
                        contextMenuStrip.Items[7].Enabled = false;
                    }
                    else
                    {
                        contextMenuStrip.Items[1].Visible = true;
                        contextMenuStrip.Items[2].Visible = true;
                        contextMenuStrip.Items[5].Visible = true;

                        contextMenuStrip.Items[4].Visible = false;
                        contextMenuStrip.Items[3].Visible = false;

                        contextMenuStrip.Items[1].Enabled = true;
                        contextMenuStrip.Items[2].Enabled = true;
                        contextMenuStrip.Items[4].Enabled = true;
                        contextMenuStrip.Items[5].Enabled = true;

                        contextMenuStrip.Items[3].Enabled = true;
                    }
                }
            }





            /*
             


             * 
             * */

            /*
             * 
            contextMenuStrip.Items[3].Enabled = true;
            contextMenuStrip.Items[4].Enabled = true;
            contextMenuStrip.Items[5].Enabled = true;

            //Если в ModelsTreeView выбран первый элемент, то блокирем кнопки "Поднять вверх"
            if (ModelsTreeView.SelectedNode == ModelsTreeView.Nodes[0].FirstNode)
            {
                upModelButton.Enabled = false;
                contextMenuStrip.Items[6].Enabled = false;
            }
            else
            {
                upModelButton.Enabled = true;
                contextMenuStrip.Items[6].Enabled = true;
            }

            //Если в ModelsTreeView выбран первый элемент, то блокирем кнопки "Опустить вниз"
            if (ModelsTreeView.SelectedNode == ModelsTreeView.Nodes[0].LastNode)
            {
                downModelButton.Enabled = false;
                contextMenuStrip.Items[7].Enabled = false;
            }
            else
            {
                downModelButton.Enabled = true;
                contextMenuStrip.Items[7].Enabled = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                selectTreeView = 1;

                contextMenuStrip.Items[3].Visible = true;
                contextMenuStrip.Items[2].Visible = false;
                contextMenuStrip.Items[4].Visible = true;

                contextMenuStrip.Show(ModelsTreeView, e.Location);
            }

                        if (e.Button == MouseButtons.Right) 
            {
                if (selectTreeView == 2)
                {
                    if (BlendShapesTreeView.SelectedNode == null)
                    {
                        BlendShapesTreeView.ContextMenuStrip = null;
                    }
                    else
                    {
                        BlendShapesTreeView.ContextMenuStrip = contextMenuStrip;
                    }

                }
            }
             */
        }
    }
}
