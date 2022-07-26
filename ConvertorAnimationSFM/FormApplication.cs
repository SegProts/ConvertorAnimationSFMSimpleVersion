using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertorAnimationSFM
{
    public partial class FormApplication : Form
    {
        private SQLiteConnection DB;
        private SQLiteCommand CMD;

        public static CheckBox updateBaseBlendShapesData = new CheckBox();
        private void updateBaseBlendShapesData_CheckedChanged(object sender, EventArgs e)
        {
            if (updateBaseBlendShapesData.Checked == true)
            {
                if (nameSelectItem.choiseFile)
                {
                    //Выбор модели созданной из файла анимации
                    if (nameSelectItem.blendShapeAnimatDone)
                    {
                        ModelListBox.SelectedIndex = ModelListBox.Items.Count - 1;
                    }
                    loadInformBlendShapesListBox();
                    updateBaseBlendShapesData.Checked = false;

                    //Обновления списка ModelList если запись о модели была сделана из файла анимации в реадкторе моделей
                    if (nameSelectItem.modelInFileAnamnation == true)
                    {
                        nameSelectItem.modelInFileAnamnation = false;
                        ModelListBox.Items.RemoveAt(0);
                    }
                }
                else 
                {
                    //Загрузка списка BlendShape в ListBox для выбранной модели
                    if (File.Exists(@"ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db"))
                    {
                        DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db; Version=3;");
                        DB.Open();
                        CMD = DB.CreateCommand();

                        CMD.CommandText = "SELECT * FROM BlendShapes";

                        ListBlendShapeModelListBox.Items.Clear();
                        //Загрузка совпадающих BlendShapes для ListBlendShapeModelListBox
                        ListBlendShapeModelListBox.Items.Add(" BlendShapes из моделе " + nameSelectItem.nameSelectMainModel + ": " + countBlendShape);
                        ListBlendShapeModelListBox.Items.Add(" ");


                        SQLiteDataReader SQL = CMD.ExecuteReader();
                        if (SQL.HasRows)
                        {
                            while (SQL.Read())
                            {
                                ListBlendShapeModelListBox.Items.Add(" " + SQL["name"].ToString());
                            }
                        }
                        else
                        {
                            //statusNameLabel.Text = "Список сохраненых моделей пуст";
                        }

                        ListBlendShapeModelListBox.Items.RemoveAt(0);
                        ListBlendShapeModelListBox.Items.Insert(0, " BlendShapes из моделе "
                            + nameSelectItem.nameSelectMainModel + ": " + (ListBlendShapeModelListBox.Items.Count - 1).ToString());

                        CMD.Dispose();
                        DB.Close();

                        updateBaseBlendShapesData.Checked = false;
                    }
                }
            }
        }

        public static CheckBox updateBaseData = new CheckBox();
        private void updateBaseData_CheckedChanged(object sender, EventArgs e)
        {
            if (updateBaseData.Checked == true)
            {
                if (File.Exists(@"ModelsResources\ModelsList.db"))
                {

                    DB = new SQLiteConnection(@"Data Source=ModelsResources\ModelsList.db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "select * from Models";

                    ModelListBox.Items.Clear();

                    ModelListBox.Items.Add("Добавить модель из файла анимации");
                    SQLiteDataReader SQL = CMD.ExecuteReader();
                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            ModelListBox.Items.Add(SQL["name"].ToString());
                        }
                    }
                    else
                    {
                        //statusNameLabel.Text = "Список сохраненых моделей пуст";
                    }

                    CMD.Dispose();
                    DB.Close();
                }

                FindProjectTextBox.Text = "";
                updateBaseData.Checked = false;
            }  
        }

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        public FormApplication()
        {
            InitializeComponent();
            updateBaseData.Checked = false;
            updateBaseData.CheckedChanged += updateBaseData_CheckedChanged;

            updateBaseBlendShapesData.Checked = false;
            updateBaseBlendShapesData.CheckedChanged += updateBaseBlendShapesData_CheckedChanged;

            //Инициализация класса обработки интерфейса
            InterfaceWork.initializedInerfeceWork(buttonModel, buttonImport, buttonExport);
            InterfaceWork.chancgeColorChouseButton(0); //При открытии всегда выбирается кнопка ПРОЕКТ

            //Назначаем кастомный рендер для контекстного меню
            topContextMenu.Renderer = new ToolStripProfessionalRenderer(new InterfaceWork.MyColorTable());
            ModelListBox.DrawMode = DrawMode.OwnerDrawFixed;
            ModelListBox.DrawItem += ModelListBox_DrawItem;

            /*-----------------Загрузка данных из Базы Данных в ListBox-------------------*/
            ModelListBox.Items.Add("Добавить модель из файла анимации");

            if (File.Exists(@"ModelsResources\ModelsList.db"))
            {
                ModelListBox.Items.Clear();

                DB = new SQLiteConnection(@"Data Source=ModelsResources\ModelsList.db; Version=3;");
                DB.Open();
                CMD = DB.CreateCommand();

                CMD.CommandText = "select * from Models";

                ModelListBox.Items.Add("Добавить модель из файла анимации");
                SQLiteDataReader SQL = CMD.ExecuteReader();
                if (SQL.HasRows)
                {
                    while (SQL.Read())
                    {
                        ModelListBox.Items.Add(SQL["name"].ToString());
                    }
                }
                else
                {
                    toolStripStatusLabel.Text = "Список сохраненых моделей пуст";
                }

                CMD.Dispose();
                DB.Close();
            }

            ListBlendShapeFileListBox.DrawMode = DrawMode.OwnerDrawFixed;
            ListBlendShapeFileListBox.DrawItem += ListBlendShapeFileListBox_DrawItem;

            ListBlendShapeModelListBox.DrawMode = DrawMode.OwnerDrawFixed;
            ListBlendShapeModelListBox.DrawItem += ListBlendShapeModelListBox_DrawItem;
        }

        //Описание кастомного рендера ModelListBox
        private void ModelListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (ModelListBox.Items.Count > 0) 
            {
                e.DrawBackground();
                Graphics g = e.Graphics;

                Brush brushCGreyColor = new SolidBrush(Color.FromArgb(47, 47, 47));

                Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
                              brushCGreyColor : new SolidBrush(e.BackColor);
                g.FillRectangle(brush, e.Bounds);
                e.Graphics.DrawString(ModelListBox.Items[e.Index].ToString(), e.Font,
                         new SolidBrush(InterfaceWork.whiteColor), e.Bounds, StringFormat.GenericDefault); //new SolidBrush(Color.FromArgb(227, 227, 227)
                e.DrawFocusRectangle();
            }
        }

        //Описание кастомного рендера ListBlendShapeFileListBox
        private void ListBlendShapeFileListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;

            Brush brushCGreyColor = new SolidBrush(Color.FromArgb(47, 47, 47));

            Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
                          brushCGreyColor : new SolidBrush(e.BackColor);
            g.FillRectangle(brush, e.Bounds);
            e.Graphics.DrawString(ListBlendShapeFileListBox.Items[e.Index].ToString(), e.Font,
                     new SolidBrush(InterfaceWork.whiteColor), e.Bounds, StringFormat.GenericDefault); //new SolidBrush(Color.FromArgb(227, 227, 227))
            e.DrawFocusRectangle();
        }

        //Описание кастомного рендера ListBlendShapeModelListBox
        private void ListBlendShapeModelListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;

            Brush brushCGreyColor = new SolidBrush(Color.FromArgb(47, 47, 47));

            Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
                          brushCGreyColor : new SolidBrush(e.BackColor);
            g.FillRectangle(brush, e.Bounds);
            e.Graphics.DrawString(ListBlendShapeModelListBox.Items[e.Index].ToString(), e.Font,
                     new SolidBrush(InterfaceWork.whiteColor), e.Bounds, StringFormat.GenericDefault); //Color.FromArgb(227, 227, 227)
            e.DrawFocusRectangle();
        }

        /*------------------------Обработка нажатий левого меню--------------------------------------*/
        private void ButtonProject_Click(object sender, EventArgs e)
        {
            InterfaceWork.chancgeColorChouseButton(0);

            ModelsPanel.Visible = true;
            ImportPanel.Visible = false;
            ExportPanel.Visible = false;

            toolStripStatusLabel.Text = " ";

            /*if (nameSelectItem.nameSelectMainModel != "")
            {
                //enableBtnImport = true;
                if (ModelListBox.SelectedIndex != -1) 
                {
                    //ModelListBox.SelectedIndex = ModelListBox.Items.IndexOf(nameSelectItem.nameSelectMainModel);
                }
            }*/
        }

        private void ButtonImport_Click(object sender, EventArgs e)
        {
            if (enableBtnImport)
            {
                InterfaceWork.chancgeColorChouseButton(1);

                ImportPanel.Visible = true;
                ModelsPanel.Visible = false;
                ExportPanel.Visible = false;

                toolStripStatusLabel.Text = " ";
                nameSelectItem.nameSelectBlendShape = "";

                if (nameSelectItem.choiseFile) 
                {
                    loadInformBlendShapesListBox();

                    if (errorLoadFileAnimation)
                    {
                        PathImportModelTextBox.Text = "";
                        errorLoadFileAnimation = false;
                    }
                }

                if (nameSelectItem.nameSelectMainModel == "Добавить модель из файла анимации") 
                {
                    ListBlendShapeModelListBox.Items.Clear();
                    ListBlendShapeModelListBox.Items.Add(" Добавить модель из файла анимации");
                }
            }
            else 
            {
                toolStripStatusLabel.Text = "  Модель не выбрана";
            }

            if (!nameSelectItem.errorExprotAnamnation && nameSelectItem.choiseFile)
            {
                toolStripStatusLabel.Text = " ";
            }
        }

        //Обработка нажатия кнопки Экспорт
        private void ButtonExport_Click(object sender, EventArgs e)
        {
            if (enableBtnExport)
            {
                InterfaceWork.chancgeColorChouseButton(2);

                ExportPanel.Visible = true;
                ModelsPanel.Visible = false;
                ImportPanel.Visible = false;

                toolStripStatusLabel.Text = " ";
                nameSelectItem.nameSelectBlendShape = "";


                //textBox2.Text = "Скрипт";
                //Перевод фокуса на 
                scriptTextBox.SelectionStart = scriptTextBox.Text.Length;

                /*----------------------------Вывод скрипта-------------------------*/
                List<String> s = Animation.GetScript(0);
                String script = "";
                foreach (String stroka in s)
                {
                    script += stroka + "\r\n";
                }

                //script += "bpy.ops.graph.select_all(action = 'SELECT')" + "\r\n";
                //script += "bpy.ops.graph.interpolation_type(type = 'LINEAR')";

                scriptTextBox.Text += script;
                Clipboard.SetText(script);
                toolStripStatusLabel.Text = "Скрипт скопирован в буфер обмена";
            }
        }

        /*------------------------Обработка нажатий контекстного меню--------------------------------*/
        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void РедакторМоделейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorModels editForm = new EditorModels();
            editForm.ShowDialog();
        }

        private void FindProjectTextBox_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(@"ModelsResources\ModelsList.db"))
            {

                DB = new SQLiteConnection(@"Data Source=ModelsResources\ModelsList.db; Version=3;");
                DB.Open();
                CMD = DB.CreateCommand();

                CMD.CommandText = "SELECT name FROM Models WHERE name LIKE '" + FindProjectTextBox.Text + "%';";

                if (ModelListBox.Items[0] == "Добавить модель из файла анимации") 
                {
                    ModelListBox.Items.Clear();
                    ModelListBox.Items.Add("Добавить модель из файла анимации");
                }
                else
                {
                    ModelListBox.Items.Clear();
                }
                
                SQLiteDataReader SQL = CMD.ExecuteReader();
                if (SQL.HasRows)
                {
                    while (SQL.Read())
                    {
                        ModelListBox.Items.Add(SQL["name"].ToString());
                    }
                }
                else
                {
                    //statusNameLabel.Text = "Список сохраненых моделей пуст";
                }

                CMD.Dispose();
                DB.Close();
            }
        }

        private void ListBlendShapeFileListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) 
            {
                if (ListBlendShapeFileListBox.SelectedItem.ToString() == " ") 
                {
                    ListBlendShapeFileListBox.SelectedIndex = prevIndexSelectFile;
                    ListBlendShapeModelListBox.SelectedIndex = prevIndexSelectModel;
                }
            }
        }

        private void ListBlendShapeModelListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (ListBlendShapeModelListBox.SelectedItem != null)
                {
                    if (ListBlendShapeModelListBox.SelectedItem.ToString() == " ")
                    {
                        ListBlendShapeFileListBox.SelectedIndex = prevIndexSelectFile;
                        ListBlendShapeModelListBox.SelectedIndex = prevIndexSelectModel;
                    }
                }
                
            }
        }

        bool sovpalItemSelect = false;
        int prevIndexSelectFile = -1;
        private void ListBlendShapeFileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBlendShapeFileListBox.SelectedItem != null) 
            {
                //Запоминаем индекс выделенного элемента
                prevIndexSelectFile = ListBlendShapeFileListBox.SelectedIndex;

                string nameSelectBlendShapeAnim = ListBlendShapeFileListBox.SelectedItem.ToString().Substring(1);

                //Проверка выбранный элемент ListBlendShapeFileListBox входит в список совпадающих BlendShapes
                if (listBlendShapeSovpalAnim.IndexOf(nameSelectBlendShapeAnim) != -1)
                {
                    sovpalItemSelect = true;
                    ListBlendShapeModelListBox.SelectedIndex = ListBlendShapeFileListBox.SelectedIndex;
                }
                else
                {
                    //Проверка выбранный элемент ListBlendShapeFileListBox содержит в себе "овпали"
                    if (ListBlendShapeFileListBox.SelectedItem.ToString().Contains("овпали")
                        || ListBlendShapeFileListBox.SelectedIndex == 0)
                    {
                        ListBlendShapeModelListBox.SelectedIndex = ListBlendShapeFileListBox.SelectedIndex;
                        sovpalItemSelect = true;
                    }
                    else
                    {
                        if (sovpalItemSelect)
                        {
                            sovpalItemSelect = false;
                            ListBlendShapeModelListBox.SelectedIndex = listBlendShapeSovpalAnim.Count + 7;
                        }
                        else
                        {
                            ListBlendShapeModelListBox.SelectedIndex = listBlendShapeSovpalAnim.Count + 7;
                        }
                    }
                }
            }
        }

        int prevIndexSelectModel = -1;
        //Переменная флаг когда файл уже открыт и пользователь выбирает первым щелчком 
        //несовпадающий элемент в ListBlendShapeModelListBox  
        //bool noSovpadItemModel = false;
        //int coontChangeNoSovpadItemModel = 0;
        private void ListBlendShapeModelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBlendShapeModelListBox.SelectedItem != null)
            {
                //Запоминаем индекс выделенного элемента
                prevIndexSelectModel = ListBlendShapeModelListBox.SelectedIndex;

                string nameSelectBlendShapeAnim = ListBlendShapeModelListBox.SelectedItem.ToString().Substring(1);

                //Проверка выбранный элемент ListBlendShapeFileListBox входит в список совпадающих BlendShapes
                if (listBlendShapeSovpalAnim.IndexOf(nameSelectBlendShapeAnim) != -1)
                {
                    sovpalItemSelect = true;
                    ListBlendShapeFileListBox.SelectedIndex = ListBlendShapeModelListBox.SelectedIndex;
                }
                else
                {
                    //Проверка выбранный элемент ListBlendShapeFileListBox содержит в себе "овпали"
                    if (ListBlendShapeModelListBox.SelectedItem.ToString().Contains("овпали")
                        || ListBlendShapeModelListBox.SelectedIndex == 0)
                    {
                        ListBlendShapeFileListBox.SelectedIndex = ListBlendShapeModelListBox.SelectedIndex;
                        sovpalItemSelect = true;
                    }
                    else
                    {
                        //Проверка совпал ли выделенный эдемент и выбран ли файл с анимацией
                        if (sovpalItemSelect && nameSelectItem.choiseFile)
                        {
                            sovpalItemSelect = false;
                            ListBlendShapeFileListBox.SelectedIndex = listBlendShapeSovpalAnim.Count + 7;
                        }
                        else 
                        {
                            if (!nameSelectItem.choiseFile)
                            {
                                ListBlendShapeFileListBox.SelectedItem = null;
                            }
                            /*else //Файл окрыт, значит несовпали элементы(смотри условие выше)
                            {
                                coontChangeNoSovpadItemModel++;
                                if (coontChangeNoSovpadItemModel < 2)
                                {
                                    noSovpadItemModel = true;
                                }
                                else 
                                {
                                    noSovpadItemModel = false;
                                }
                            }

                            if (noSovpadItemModel)
                            {
                                noSovpadItemModel = false;
                                ListBlendShapeFileListBox.SelectedIndex = listBlendShapeSovpalAnim.Count + 7; 
                            }*/
                        }
                    }
                }

            }

            if (ListBlendShapeModelListBox.SelectedIndex > 2)
            {
                if (!ListBlendShapeModelListBox.SelectedItem.ToString().Contains("овпали")
                    || !ListBlendShapeModelListBox.SelectedItem.ToString().Contains(" "))
                {
                    nameSelectItem.nameSelectMainBlendShape = ListBlendShapeModelListBox.SelectedItem.ToString();
                } 
            }
        }

        private void blendShapeComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Graphics g = e.Graphics;

                Brush brushGreyColor = new SolidBrush(Color.FromArgb(47, 47, 47)); //SystemColors.WindowFrame

                Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected) ?
                              brushGreyColor : new SolidBrush(e.BackColor);
                g.FillRectangle(brush, e.Bounds);
                e.Graphics.DrawString(typeEditorComboBox.Items[e.Index].ToString(), e.Font,
                    new SolidBrush(InterfaceWork.whiteColor), e.Bounds, StringFormat.GenericDefault); //new SolidBrush(InterfaceWork.whiteColor) e.ForeColor
                e.DrawFocusRectangle();
            }
        }

        private void typeEditorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            typeEditorComboBox.BackColor = SystemColors.WindowFrame;
            scriptTextBox.Focus();
        }
        private void FormApplication_Shown(object sender, EventArgs e)
        {
            ModelListBox.SelectedIndex = 1;
            //Главное окно приложения
            this.BackColor = InterfaceWork.backgroundFormColor;

            typeEditorComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            typeEditorComboBox.DrawItem += blendShapeComboBox_DrawItem;
            typeEditorComboBox.ForeColor = InterfaceWork.whiteColor;

            //Главные кнопки режимов
            buttonModel.ForeColor = InterfaceWork.whiteColor;
            buttonImport.ForeColor = InterfaceWork.whiteColor;
            buttonExport.ForeColor = InterfaceWork.whiteColor;

            //Панель модель


            //Панель импорт
            getAllContols(Controls[0]);

            foreach (Control item in listContols)
            {

                if (item is Label)
                {
                    item.ForeColor = InterfaceWork.whiteColor;
                }

                if (item is TextBox)
                {
                    item.ForeColor = InterfaceWork.whiteColor;
                    item.BackColor = InterfaceWork.backgroundTextColor;
                }

                if (item is Button)
                {
                    item.ForeColor = InterfaceWork.whiteColor;
                }

                if (item is ListBox)
                {
                    item.BackColor = InterfaceWork.backgroundTextColor;
                }

            }

            //Меню
            for (int i = 0; i < topContextMenu.Items.Count; i++)
            {
                topContextMenu.Items[i].ForeColor = InterfaceWork.whiteColor;
            }

            //Установка цвета для строки уведомлений
            toolStripStatusLabel.ForeColor = InterfaceWork.whiteColorPush;

            //Загрузка редактора по умолчанию для экспорта скрипта
            typeEditorComboBox.SelectedIndex = 0; //Blender

            //Временно
            //EditorModels editForm = new EditorModels();
            //editForm.ShowDialog();
        }

        List<Control> listContols = new List<Control>();

        public void getAllContols(Control parent)
        {
            foreach (Control item in parent.Controls)
            {

                listContols.Add(item);
                getAllContols(item);
            }
        }

        private void choiceFileAnimButton_Click(object sender, EventArgs e)
        {
            //Открыть диалоговое окно выбора файла анимации
            openImportFileDialog.ShowDialog();
        }

        //Количество BlendShapes модели
        int countBlendShape;
        //Список BlendShapes выранной модели
        List<String> listBlendShapesModel = new List<String>();

        List<string> fileLines = new List<string>();
        //Список BlendShapes и таймингов анимации из файла
        //List<BlenShapeNoteAnimation> listBlendShapeAnimation = new List<BlenShapeNoteAnimation>();

        //Список BlendShapes и таймингов анимации из файла
        List<double> importListTimes = new List<double>();
        List<double> imporListValues = new List<double>();

        private void openImportFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            toolStripStatusLabel.Text = " ";

            loadInformBlendShapesListBox();
        }

        //Обображение совпадающих и не совпадающих BlendShapes
        List<String> listBlendShapeSovpalAnim = new List<String>();
        List<String> listBlendShapeNoSovpalAnim = new List<String>();

        /*--------------------------------Загрузка BlendShapes из файла анимации---------------------------------*/
        public void loadInformBlendShapesListBox()
        {
            //Обработка события после выбора файла анимации
            if (openImportFileDialog.FileName != "")
            {
                //Был выбран файл анимации
                nameSelectItem.choiseFile = true;

                loadInformBlendShapesSelectModel();

                //Отчищаем InformFileAnimationTextBox от ифнормации прошлого файла анимации
                fileLines.Clear(); //Отчищаем список со старыми строками файла анимации
                //InformFileAnimationTextBox.Text = " Информация из файла анимации:\r\n\r\n";

                //Получаем путь к выбранному файлу анимации
                PathImportModelTextBox.Text = " " + openImportFileDialog.FileName;

                //Открываем и считаваем в строки файла анимации
                try
                {
                    using (StreamReader sr = new StreamReader(PathImportModelTextBox.Text, System.Text.Encoding.Default))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            fileLines.Add(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel.Text = ex.Message;
                }

                if (fileLines.Count != 0)
                {
                    if (fileLines[0] == "<!-- dmx encoding keyvalues2 1 format model 18 -->" 
                        || fileLines[0] == "<!-- dmx encoding keyvalues2 4 format model 22 -->")
                    {
                        //Загрузка имени файла анимации
                        int indexLastRazdel = openImportFileDialog.FileName.LastIndexOf(@"\") + 1;

                        InformFileAnimationTextBox.Text = " Информация из файла анимации: " + 
                            openImportFileDialog.FileName.Substring(indexLastRazdel) + "\r\n\r\n";
                        //ListBlendShapeModelListBox.Items.Insert(0, " " + );

                        //Загрузка имени модели из файла
                        indexLastRazdel = fileLines[14].LastIndexOf("/") + 1;
                        string nameModel = fileLines[14].Remove(fileLines[14].Length - 1);

                        nameModel = nameModel.Substring(indexLastRazdel);

                        if (nameSelectItem.nameInitModel != "")
                        {
                            if (nameSelectItem.nameInitModel != nameModel) 
                            {
                                ModelListBox.Items.Insert(0, "Добавить модель из файла анимации");
                            }
                        }
                        else
                        {
                            nameSelectItem.nameInitModel = nameModel;
                        }


                        //Загрузка Frame rate из файла
                        string frameRateAnimation = fileLines[17].Remove(fileLines[17].Length - 1); ;
                        int indexLastKovich = frameRateAnimation.LastIndexOf("\"") + 1;
                        frameRateAnimation = frameRateAnimation.Substring(indexLastKovich);

                        //Запоминаем фреймрейт загруженной анимации
                        nameSelectItem.frameRateAnimation = frameRateAnimation.Remove(frameRateAnimation.Length - 3, 3);

                        //Загрузка количество кадров в анимации из файла
                        string frameCountAnimation = fileLines[16].Remove(fileLines[16].Length - 1);
                        indexLastKovich = frameCountAnimation.LastIndexOf("\"") + 1;
                        frameCountAnimation = frameCountAnimation.Substring(indexLastKovich);


                        //Загружаем полученную информацию из файла в InformFileAnimationTextBox и ListBlendShapeFileListBox 
                        if (fileLines.Count > 0)
                        {
                            if (fileLines[0] == "<!-- dmx encoding keyvalues2 1 format model 18 -->")
                            {
                                InformFileAnimationTextBox.Text += ("  Источник анимации: Source Filmmaker");

                                InformFileAnimationTextBox.Text += ("\r\n  Модель: " + nameModel);

                                InformFileAnimationTextBox.Text += ("\r\n  Frame rate: " + frameRateAnimation); //"\r\n

                                InformFileAnimationTextBox.Text += ("\r\n  Количество кадров: " + frameCountAnimation);
                            }

                            if (fileLines[0] == "<!-- dmx encoding keyvalues2 4 format model 22 -->")
                            {
                                InformFileAnimationTextBox.Text += ("  Источник анимации: Source Filmmaker(Dota)");

                                InformFileAnimationTextBox.Text += ("\r\n  Модель: " + nameModel);

                                InformFileAnimationTextBox.Text += ("\r\n  Frame rate: " + frameRateAnimation);

                                InformFileAnimationTextBox.Text += ("\r\n  Количество кадров: " + frameCountAnimation);
                            }

                            /*------------Загрузка информации об BlendShapes и таймингах анимации из файла-------------*/
                            // Количество найденных flexWeight подряд
                            int n = 0;
                            int indexMinusZero = -1;

                            nameSelectItem.listBlendShapeAnimation.Clear();
                            importListTimes.Clear();
                            imporListValues.Clear();

                            //Имя BlendShape
                            string nameBlendShape = "";

                            ListBlendShapeFileListBox.Items.Clear();

                            for (int i = 0; i < fileLines.Count; i++)
                            {
                                if (fileLines[i].IndexOf("flexWeight") != -1)
                                {
                                    n++;

                                    if (n == 1)
                                    {
                                        nameBlendShape = fileLines[i - 1].Remove(fileLines[i - 1].Length - 1); ;
                                        indexLastKovich = nameBlendShape.LastIndexOf("\"") + 1;
                                        nameBlendShape = nameBlendShape.Substring(indexLastKovich);

                                        ListBlendShapeFileListBox.Items.Add("  " + nameBlendShape);
                                    }

                                    if (n == 2)
                                    {
                                        n = 0;

                                        while (fileLines[i].IndexOf("\"times\" \"time_array\"") == -1)
                                        {
                                            i++;
                                        }

                                        i += 2;

                                        //Если BlendShape вообще не анимирвовался, то в фале не будет вообще значений dhtvtyb b pyfxtybz
                                        if (fileLines[i].IndexOf("]") == -1)
                                        {
                                            while (getNumber(fileLines[i], 0) != -1)
                                            {
                                                importListTimes.Add(Math.Round(getNumber(fileLines[i], 0) - 5.0, 6));
                                                i++;
                                            }
                                            i += 6;
                                        }
                                        else 
                                        {
                                            importListTimes.Add(0.0);
                                            i++;

                                            importListTimes.Add(getNumber(frameCountAnimation, 6) / getNumber(frameRateAnimation, 6));
                                            i += 5;
                                        }

                                        //Если BlendShape вообще не анимировался, то в файле не будет вообще значений
                                        if (fileLines[i].IndexOf("]") == -1)
                                        {
                                            while (getNumber(fileLines[i], 1) != -1)
                                            {
                                                imporListValues.Add(getNumber(fileLines[i], 1));
                                                i++;
                                            }
                                        }
                                        else
                                        {
                                            imporListValues.Add(0.0);
                                            i++;

                                            imporListValues.Add(0.0);
                                            i++;
                                        }

                                        if (importListTimes[0] < 0) 
                                        {
                                            importListTimes[0] = 0;
                                        }
                                        /*while (getNumber(fileLines[i], 1) != -1)
                                        {
                                            imporListValues.Add(getNumber(fileLines[i], 1));
                                            i++;
                                        }

                                        //Убираем надбавку во времения на 5(5 = 0 с)
                                        if (importListTimes.Count == 2)
                                        {
                                            importListTimes[0] = 0;
                                        }
                                        else 
                                        {
                                            importListTimes.RemoveAt(0);
                                            imporListValues.RemoveAt(0);
                                        }

                                        i++;*/

                                        nameSelectItem.listBlendShapeAnimation.Add(new BlenShapeNoteAnimation(nameBlendShape, importListTimes, imporListValues));

                                        importListTimes.Clear();
                                        imporListValues.Clear();
                                    }

                                }
                            }
                        }

                        //Обображение совпадающих и не совпадающих BlendShapes
                        listBlendShapeSovpalAnim = new List<String>();
                        listBlendShapeNoSovpalAnim = new List<String>();

                        //Поиск несопадающих BlendShapes из файла анимации
                        foreach (BlenShapeNoteAnimation itemAnim in nameSelectItem.listBlendShapeAnimation)
                        {
                            bool sovpal = false;
                            foreach (String itemModel in listBlendShapesModel)
                            {
                                if (itemModel == itemAnim.name)
                                {
                                    sovpal = true;
                                    break;
                                }
                                else
                                {
                                    sovpal = false;
                                }
                            }

                            if (sovpal)
                            {
                                listBlendShapeSovpalAnim.Add(itemAnim.name);
                            }
                            else
                            {
                                if (listBlendShapeNoSovpalAnim.IndexOf(itemAnim.name) == -1)
                                {
                                    listBlendShapeNoSovpalAnim.Add(itemAnim.name);
                                }
                            }
                        }

                        //Если есть совпадающие BlendShapes, то вычисляем несовпадающие для ListBlendShapeModelListBox
                        if (listBlendShapeSovpalAnim.Count > 0)
                        {
                            //Поиск несопадающих BlendShapes из файла модели
                            foreach (String itemModel in listBlendShapesModel.ToArray())
                            {
                                foreach (String itemAnimSovpad in listBlendShapeSovpalAnim)
                                {
                                    if (itemModel == itemAnimSovpad)
                                    {
                                        listBlendShapesModel.Remove(itemModel);
                                    }
                                }
                            }
                        }

                        //Загрузка в listBox совпадающих BlendShapes
                        ListBlendShapeFileListBox.Items.Clear();
                        ListBlendShapeModelListBox.Items.Clear();

                        //Загрузка совпадающих BlendShapes для ListBlendShapeFileListBox
                        ListBlendShapeFileListBox.Items.Add(" BlendShapes из файла анимации: " + (nameSelectItem.listBlendShapeAnimation.Count).ToString());
                        ListBlendShapeFileListBox.Items.Add(" ");
                        ListBlendShapeFileListBox.Items.Add(" Совпали: " + (listBlendShapeSovpalAnim.Count).ToString());
                        ListBlendShapeFileListBox.Items.Add(" ");

                        foreach (String item in listBlendShapeSovpalAnim)
                        {
                            ListBlendShapeFileListBox.Items.Add(" " + item);
                        }

                        //Загрузка совпадающих BlendShapes для ListBlendShapeModelListBox
                        if (nameSelectItem.nameSelectMainModel == "Добавить модель из файла анимации")
                        {
                            ListBlendShapeModelListBox.Items.Clear();
                            ListBlendShapeModelListBox.Items.Add(" Добавить модель из файла анимации");
                        }
                        else 
                        {
                            ListBlendShapeModelListBox.Items.Add(" BlendShapes из моделе " + nameSelectItem.nameSelectMainModel + ": " + countBlendShape);
                            ListBlendShapeModelListBox.Items.Add(" ");
                            ListBlendShapeModelListBox.Items.Add(" Совпали: " + (listBlendShapeSovpalAnim.Count).ToString());
                            ListBlendShapeModelListBox.Items.Add(" ");
                        }

                        foreach (String item in listBlendShapeSovpalAnim)
                        {
                            ListBlendShapeModelListBox.Items.Add(" " + item);
                        }

                        //Загрузка не совпадающих BlendShapes для ListBlendShapeFileListBox
                        ListBlendShapeFileListBox.Items.Add(" ");
                        ListBlendShapeFileListBox.Items.Add(" Не совпали: " + (listBlendShapeNoSovpalAnim.Count).ToString());
                        ListBlendShapeFileListBox.Items.Add(" ");

                        foreach (String item in listBlendShapeNoSovpalAnim)
                        {
                            ListBlendShapeFileListBox.Items.Add(" " + item);
                        }

                        //Загрузка не совпадающих BlendShapes для ListBlendShapeModelListBox
                        if (nameSelectItem.nameSelectMainModel != "Добавить модель из файла анимации")
                        {
                            ListBlendShapeModelListBox.Items.Add(" ");
                            ListBlendShapeModelListBox.Items.Add(" Не совпали: " + (listBlendShapesModel.Count).ToString());
                            ListBlendShapeModelListBox.Items.Add(" ");
                        }

                        foreach (String item in listBlendShapesModel.ToArray())
                        {
                            ListBlendShapeModelListBox.Items.Add(" " + item);
                        }

                        //Обработка ситуации когда не все BlendShapes выбранной модели соотвествуют BlendShapes файла анимации
                        if (listBlendShapeNoSovpalAnim.Count != 0)
                        {
                            nameSelectItem.errorExprotAnamnation = true;
                            
                            //Информируем пользователя о возможности создания записи об модели из файла анимации
                            if (ModelListBox.SelectedIndex == 0)
                            {
                                toolStripStatusLabel.Text = "  Модель " + nameSelectItem.nameSelectMainModel + " не содержит необходимых BlendShapes.";
                            }
                            else 
                            {
                                toolStripStatusLabel.Text = "  Можете создать модель из файла анимации.";
                            } 

                            //Если запись о моделе из анимации была создана, то указываем это, чтобы пользователь мог зайти в модели и выбрать ее
                            //А так же убрать пункт "Создать из файла анимации" при повторном открытии редактора с одним и тем же открытым файлом анимации
                            if (nameSelectItem.modelInFileAnamnation)
                            {
                                toolStripStatusLabel.Text += " Модель " + nameSelectItem.nameModelInAnimation + " из файла анимации уже создана.";
                            }
                        }

                        //Если BlendShapes из файла анимации совпадают с BlendShapes выбранной модели  
                        if(listBlendShapeSovpalAnim.Count != 0)
                        {
                            toolStripStatusLabel.Text = "  Анимация может быть экспортирована";
                            //Включаем возможность перейти к Экспорту
                            enableBtnExport = true;
                        }

                        //Если BlendShapes из файла анимации не совпадают с BlendShapes выбранной модели 
                        if (listBlendShapeSovpalAnim.Count == listBlendShapesModel.Count) 
                        {
                            //Выключаем возможность перейти к Экспорту
                            enableBtnExport = false;
                        }
                    }
                    else
                    {
                        //Возникла ошибка загрузки файла анимации
                        errorLoadFileAnimation = true;

                        if (errorLoadFileAnimation) 
                        {
                            toolStripStatusLabel.Text = "  Выбранный файл не содержит в себе информацию об анимации.";
                        }

                        InformFileAnimationTextBox.Text = " Информация из файла анимации: ";
                        ListBlendShapeFileListBox.Items.Clear();
                        ListBlendShapeFileListBox.Items.Add(" BlendShapes из файла анимации:");
 
                        //Загрузка списка BlendShape в ListBox для выбранной модели
                        loadInformBlendShapesSelectModel();
                    }
                }
                else
                {
                    //Возникла ошибка загрузки файла анимации
                    errorLoadFileAnimation = true;

                    toolStripStatusLabel.Text = "  Выбранный файл не содержит в себе информацию об анимации.";

                    InformFileAnimationTextBox.Text = " Информация из файла анимации: ";
                    ListBlendShapeFileListBox.Items.Clear();
                    ListBlendShapeFileListBox.Items.Add(" BlendShapes из файла анимации:");

                    //Загрузка списка BlendShape в ListBox для выбранной модели
                    loadInformBlendShapesSelectModel();
                }

                //Сбрасываем переменую флаг о создании записи о моделе из файла анимации для новой загруженной анимации
                //nameSelectItem.modelInFileAnamnation = false;
            }
        }

        //Ошибка загрузки файла анимации
        bool errorLoadFileAnimation = false;

        private void loadInformBlendShapesSelectModel()
        {
            //Загрузка списка BlendShape в ListBox для выбранной модели
            if (nameSelectItem.nameSelectModel != "")
            {
                if (File.Exists(@"ModelsResources\BlendShapes\" + nameSelectItem.nameSelectMainModel + ".db"))
                {
                    DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + nameSelectItem.nameSelectMainModel + ".db; Version=3;");
                    DB.Open();
                    CMD = DB.CreateCommand();

                    CMD.CommandText = "SELECT * FROM BlendShapes";
                    //name, type, nameSFM, nameEditorLeft, nameEditorRight

                    listBlendShapesModel.Clear();

                    ListBlendShapeModelListBox.Items.Clear();

                    SQLiteDataReader SQL = CMD.ExecuteReader();
                    countBlendShape = 0;
                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            countBlendShape++;
                            listBlendShapesModel.Add(SQL["name"].ToString());
                            ListBlendShapeModelListBox.Items.Add(" " + SQL["name"].ToString());
                        }
                    }
                    else
                    {
                        //statusNameLabel.Text = "Список сохраненых моделей пуст";
                    }

                    ListBlendShapeModelListBox.Items.Insert(0, " BlendShapes из моделе " + nameSelectItem.nameSelectMainModel + ": " + countBlendShape);
                    ListBlendShapeModelListBox.Items.Insert(1, " ");

                    CMD.Dispose();
                    DB.Close();
                }
            }
        }

        public double getNumber(string stroka, int typeValue) 
        {
            string result = "";
            for (int i = 0; i < stroka.Length; i++) 
            {
                if (Char.IsNumber(stroka[i]) || stroka[i] == '.') 
                {
                    result += stroka[i];
                }
            }

            //float asd = (float) Convert.ToDouble("41.00027357629127");
            if (result != "") 
            {
                //return float.Parse(result, System.Globalization.CultureInfo.InvariantCulture);
                //0 - значения времени, 1 - значение BlendShape
                if (typeValue == 0)
                {
                    return Math.Round(Double.Parse(result, CultureInfo.InvariantCulture), 6);
                    //return Math.Round(float.Parse(result, CultureInfo.InvariantCulture), 6);
                }
                else
                {
                    return Double.Parse(result, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            else 
            {
                return -1;
            }
        }

        bool enableBtnImport = false;
        bool enableBtnExport = false;

        
        private void ModelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModelListBox.SelectedItem != null)
            {
                enableBtnImport = true;
                nameSelectItem.nameSelectMainModel = ModelListBox.SelectedItem.ToString();

                //Загрузка списка BlendShape в ListBox для выбранной модели
                loadInformBlendShapesSelectModel();
            }
            else 
            {
                enableBtnImport = false;
                nameSelectItem.nameSelectMainModel = "";
            }

            toolStripStatusLabel.Text = " "; //Сброс статуса ошибки
        }

        /*--------------------------------------Сохранения скрипта----------------------------------*/
        private void button1_Click(object sender, EventArgs e)
        {
            int index = -1;
            
            foreach (BlenShapeNoteAnimation item in nameSelectItem.listBlendShapeAnimation) 
            {
                index++;
                if (item.name == "lipPressor") { break; }
                //lipCornerDepressorAndSharpLipPuller
                //upperLipsTowardAndPart
                //lowerLipsTowardAndPart
            }

            List<String> list = new List<String>();

            foreach (float item in nameSelectItem.listBlendShapeAnimation[index].listTimes)
            {
                list.Add(item + ";");
            }

            int n = -1;
            foreach (float item in nameSelectItem.listBlendShapeAnimation[index].listValues)
            {
                n++;
                list[n] +=  item + ";";
            }

            scriptTextBox.Text = "t;value;" + "\r\n";
            foreach (String item in list)
            {
                scriptTextBox.Text += item + "\r\n";
            }

            Clipboard.SetText(scriptTextBox.Text);

            //saveFileDialog.ShowDialog();
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string filename = saveFileDialog.FileName;
            System.IO.File.WriteAllText(filename, scriptTextBox.Text);
            //toolStripStatusLabel.Text = " Скрипт " + filename + " успешно экспортирован для "
            //    + typeEditorComboBox.Items[typeEditorComboBox.SelectedIndex];
        }
    }
}
