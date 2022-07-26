using ConvertorAnimationSFM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertorAnimationSFMSimpleVersion
{
    public partial class CreateBlendShape : Form
    {
        private SQLiteConnection DB;
        private SQLiteCommand CMD;

        public bool changedType = false;

        public bool enableUdtade = false;
        public bool enableUdtateFormBlendShapes = false;
        public bool enableUdateSelectNewBLendShape = false;

        public CreateBlendShape()
        {
            InitializeComponent();

            if (nameSelectItem.typeInitWindows == 1)
            {
                createButton.Text = "ИЗМЕНИТЬ";

                if (nameSelectItem.selectBlendShape.type == 0)
                {
                    typeBlendShapeComboBox.SelectedIndex = 0; //Выбор одинарного BlendShape при загрузке
                    sfmOneTextBox.Text = nameSelectItem.selectBlendShape.nameSFM;
                    editorOnePanelTextBox.Text = nameSelectItem.selectBlendShape.nameEditorLeft;
                }
                else
                {
                    OneBlendPanel.Visible = false;
                    TwoBlendPanel.Visible = true;

                    typeBlendShapeComboBox.SelectedIndex = 1; //Выбор двойного BlendShape при загрузке
                    sfmTwoTextBox.Text = nameSelectItem.selectBlendShape.name;
                    editorLeftTextBox.Text = nameSelectItem.selectBlendShape.nameEditorLeft;
                    editorRightTextBox.Text = nameSelectItem.selectBlendShape.nameEditorRight;
                }
            }
            else
            {
                typeBlendShapeComboBox.SelectedIndex = 0;
                createButton.Text = "СОЗДАТЬ";
            }

            /*------------------Инициализация кастомного рендера для интерфейса------------------*/
            typeBlendShapeComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            typeBlendShapeComboBox.DrawItem += blendShapeComboBox_DrawItem;
        }

        /*---------------------------------КАСТОМНЫЙ РЕНДЕР--------------------------------*/
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
                e.Graphics.DrawString(typeBlendShapeComboBox.Items[e.Index].ToString(), e.Font,
                    new SolidBrush(InterfaceWork.whiteColor), e.Bounds, StringFormat.GenericDefault); //new SolidBrush(InterfaceWork.whiteColor) e.ForeColor
                e.DrawFocusRectangle();
            }
        }

        private void BtnCreateBlendShap_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void typeBlendShapeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (typeBlendShapeComboBox.SelectedIndex == 0)
            {
                OneBlendPanel.Visible = true;
                TwoBlendPanel.Visible = false;

                this.Size = new Size(552, 396);

                sfmOneTextBox.Text = sfmTwoTextBox.Text;
                editorOnePanelTextBox.Text = editorLeftTextBox.Text;

                changedType = true;

                sfmOneTextBox.SelectionStart = sfmOneTextBox.Text.Length;
                sfmOneTextBox.Focus();
            }
            else
            {
                OneBlendPanel.Visible = false;
                TwoBlendPanel.Visible = true;

                this.Size = new Size(552, 460);

                sfmTwoTextBox.Text = sfmOneTextBox.Text;
                editorLeftTextBox.Text = editorOnePanelTextBox.Text;

                sfmTwoTextBox.SelectionStart = sfmTwoTextBox.Text.Length;
                sfmTwoTextBox.Focus();
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            //Обновление уже сущевствующей запирси о BlenShape
            if (nameSelectItem.typeInitWindows == 1)
            {
                UdpadeBlendShape();
            }
            else //Создание записи о новой BlendShape
            {
                AddBlendShape();
            }
        }

        bool closeWindow = false;
        public void UdpadeBlendShape()
        {
            //Запись имени новой модели в БД где хранятся всех модели
            DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db; Version=3;");
            DB.Open();
            CMD = DB.CreateCommand();

            if (typeBlendShapeComboBox.SelectedIndex == 0)
            {
                if (sfmOneTextBox.Text != "" && editorOnePanelTextBox.Text != "")
                {
                    if (sfmOneTextBox.Text[0] != ' ' && editorOnePanelTextBox.Text[0] != ' ')
                    {
                        CMD.CommandText = "UPDATE BlendShapes SET name = @name, type = @type, nameSFM = @nameSFM," +
                        " nameEditorLeft = @nameEditorLeft  WHERE name = @nameBlendShape";

                        CMD.Parameters.Add("@name", System.Data.DbType.String).Value = sfmOneTextBox.Text;
                        CMD.Parameters.Add("@type", System.Data.DbType.String).Value = typeBlendShapeComboBox.SelectedIndex;
                        CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = sfmOneTextBox.Text;
                        CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = editorOnePanelTextBox.Text;
                        CMD.Parameters.Add("@nameBlendShape", System.Data.DbType.String).Value = nameSelectItem.nameSelectBlendShape;

                        closeWindow = true;
                    }
                    else
                    {
                        statusNameLabel.Text = "Введеноное имя начинается с пробела";
                    }
                }
                else 
                {
                    statusNameLabel.Text = "Введено пустое имя";
                } 
            }
            else
            {
                if (sfmTwoTextBox.Text != "" && editorLeftTextBox.Text != "" && editorRightTextBox.Text != "")
                {
                    if (sfmTwoTextBox.Text[0] != ' ' && editorLeftTextBox.Text[0] != ' ' && editorRightTextBox.Text[0] != ' ')
                    {
                        CMD.CommandText = "UPDATE BlendShapes SET name = @name, type = @type, nameSFM = @nameSFM," +
                            " nameEditorLeft = @nameEditorLeft, nameEditorRight = @nameEditorRight  WHERE name = @nameBlendShape";

                        CMD.Parameters.Add("@name", System.Data.DbType.String).Value = sfmTwoTextBox.Text;
                        CMD.Parameters.Add("@type", System.Data.DbType.String).Value = typeBlendShapeComboBox.SelectedIndex;
                        CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = sfmTwoTextBox.Text;
                        CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = editorLeftTextBox.Text;
                        CMD.Parameters.Add("@nameEditorRight", System.Data.DbType.String).Value = editorRightTextBox.Text;
                        CMD.Parameters.Add("@nameBlendShape", System.Data.DbType.String).Value = nameSelectItem.nameSelectBlendShape;

                        closeWindow = true;
                    }
                    else
                    {
                        statusNameLabel.Text = "Введеноное имя начинается с пробела";
                    }
                }
                else
                {
                    statusNameLabel.Text = "Введено пустое имя";
                }
            }

            if (closeWindow)
            {
                
                try
                {
                    CMD.ExecuteNonQuery();
                }
                catch (System.Data.SQLite.SQLiteException ex) 
                {
                    if (ex.Message == "constraint failed\r\nUNIQUE constraint failed: BlendShapes.name") 
                    {
                        DialogResult result = MessageBox.Show("Введенное имя совпадает с уже существующим", 
                            "Ошибка ввода имени BlendShape", MessageBoxButtons.OK);
                    }
                }

                if (changedType)
                {
                    CMD.CommandText = "UPDATE BlendShapes SET nameEditorRight = @nameEditorRight  WHERE name = @nameBlendShape";
                    CMD.Parameters.Add("@nameEditorRight", System.Data.DbType.String).Value = null;
                    CMD.Parameters.Add("@nameBlendShape", System.Data.DbType.String).Value = nameSelectItem.nameSelectBlendShape;
                }

                CMD.ExecuteNonQuery();

                CMD.Dispose();
                DB.Close();
                DB.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                // = true; // Нужно обновить отображение списка BlendShapes в окне EditorModels
                //enableUdtateFormBlendShapes = true; //Обновить отображение списка BlendShapes в окне FormApplication

                //Были внесенны новые данные, следует обновить ListBox в окне Редактора
                EditorModels.updateBaseDataEditor.Checked = true;

                //Были внесенны новые данные, следует обновить ListBox в главном окне программы
                FormApplication.updateBaseBlendShapesData.Checked = true;

                if (typeBlendShapeComboBox.SelectedIndex == 0) 
                {
                    //nameSelectItem.nameSelectBlendShape = ;
                }
                else
                {
                    //nameSelectItem.nameSelectBlendShape = ;
                }

                EditorModels.updateSelectBlendShape.Checked = true;

                this.Close();
            }
        }

        public void AddBlendShape()
        {
            //Если выбран одиночный тип BlendShape
            if (typeBlendShapeComboBox.SelectedIndex == 0)
            {
                statusNameLabel.Text = "Успешно";
                //Проверка пустое ли название ввел пользователь
                if (sfmOneTextBox.Text != "" && editorOnePanelTextBox.Text != "")
                {
                    if (sfmOneTextBox.Text[0] != ' ' && editorOnePanelTextBox.Text[0] != ' ')
                    { 
                        try
                        {
                            DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db; Version=3;");

                            DB.Open();
                            CMD = DB.CreateCommand();

                            CMD.CommandText = "INSERT INTO BlendShapes('name', 'type', 'nameSFM', 'nameEditorLeft') " +
                                "VALUES(@name, @type, @nameSFM, @nameEditorLeft)";
                            CMD.Parameters.Add("@name", System.Data.DbType.String).Value = sfmOneTextBox.Text;
                            CMD.Parameters.Add("@type", System.Data.DbType.Int32).Value = typeBlendShapeComboBox.SelectedIndex;
                            CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = sfmOneTextBox.Text;
                            CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = editorOnePanelTextBox.Text;

                            CMD.ExecuteNonQuery();

                            CMD.Dispose();
                            DB.Close();
                            DB.Dispose();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            enableUdtade = true; // Нужно обновить отображение списка BlendShapes в окне EditorModels
                            enableUdtateFormBlendShapes = true; //Обновить отображение списка BlendShapes в окне FormApplication
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
                                DialogResult result = MessageBox.Show("Введенное имя совпадает с уже существующим",
                                    "Ошибка ввода имени BlendShape", MessageBoxButtons.OK);
                            }
                        }
                    }
                    else
                    {
                        statusNameLabel.Text = "Введеноное имя начинается с пробела";
                    }

                    enableUdateSelectNewBLendShape = true;
                    this.Close();
                }
                else
                {
                    statusNameLabel.Text = "Введено пустое имя";
                }
            }
            else //Если выбран двойной тип BlendShape
            {
                statusNameLabel.Text = "Успешно";
                //Проверка пустое ли название ввел пользователь
                if (sfmTwoTextBox.Text != "" && editorLeftTextBox.Text != "" && editorRightTextBox.Text != "")
                {
                    if (sfmTwoTextBox.Text[0] != ' ' && editorLeftTextBox.Text[0] != ' ' && editorRightTextBox.Text[0] != ' ')
                    {
                        try
                        {
                            DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + nameSelectItem.nameSelectModel + ".db; Version=3;");

                            DB.Open();
                            CMD = DB.CreateCommand();

                            CMD.CommandText = "INSERT INTO BlendShapes('name', 'type', 'nameSFM', 'nameEditorLeft', 'nameEditorRight')"
                                + "VALUES(@name, @type, @nameSFM, @nameEditorLeft, @nameEditorRight)";
                            CMD.Parameters.Add("@name", System.Data.DbType.String).Value = sfmTwoTextBox.Text;
                            CMD.Parameters.Add("@type", System.Data.DbType.Int32).Value = typeBlendShapeComboBox.SelectedIndex;
                            CMD.Parameters.Add("@nameSFM", System.Data.DbType.String).Value = sfmTwoTextBox.Text;
                            CMD.Parameters.Add("@nameEditorLeft", System.Data.DbType.String).Value = editorLeftTextBox.Text;
                            CMD.Parameters.Add("@nameEditorRight", System.Data.DbType.String).Value = editorRightTextBox.Text;

                            CMD.ExecuteNonQuery();

                            CMD.Dispose();
                            DB.Close();
                            DB.Dispose();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            enableUdtade = true; // Нужно обновить отображение списка BlendShapes в окне EditorModels
                            enableUdtateFormBlendShapes = true; //Обновить отображение списка BlendShapes в окне FormApplication
                        }
                        catch (System.Data.SQLite.SQLiteException ex)
                        {
                            CMD.Dispose();
                            DB.Close();
                            DB.Dispose();

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                    else
                    {
                        statusNameLabel.Text = "Введеноное имя начинается с пробела";
                    }

                    enableUdateSelectNewBLendShape = true;
                    this.Close();
                }
                else
                {
                    statusNameLabel.Text = "Введено пустое имя";
                }
            }
        }

        private void CreateBlendShape_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (enableUdtade == true)
            {
                //Были внесенны новые данные, следует обновить ListBox в окне Редактора
                EditorModels.updateBaseDataEditor.Checked = true;
            }
            if (enableUdtateFormBlendShapes == true)
            {
                //Были внесенны новые данные, следует обновить ListBox в главном окне программы
                FormApplication.updateBaseBlendShapesData.Checked = true;
            }

            if (enableUdateSelectNewBLendShape == true) 
            {
                EditorModels.updateSelectNewBlendShape.Checked = true;
            }

            //nameSelectItem.selectBlendShape = null;
        }

        private void sfmTwoTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 0)
            {
                AddBlendShape();
            }

            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 1)
            {
                UdpadeBlendShape();
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void editorLeftTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 0)
            {
                AddBlendShape();
            }

            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 1)
            {
                UdpadeBlendShape();
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void editorRightTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 0)
            {
                AddBlendShape();
            }

            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 1) 
            {
                UdpadeBlendShape();
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void sfmOneTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 0)
            {
                AddBlendShape();
            }

            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 1)
            {
                UdpadeBlendShape();
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void editorOnePanelTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 0)
            {
                AddBlendShape();
            }

            if (e.KeyCode == Keys.Enter && nameSelectItem.typeInitWindows == 1)
            {
                UdpadeBlendShape();
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void CreateBlendShape_Shown(object sender, EventArgs e)
        {
            CreateBlendShape.ActiveForm.BackColor = InterfaceWork.backgroundFormColor;
            statusStrip.BackColor = InterfaceWork.backgroundFormColor;
            createButton.ForeColor = InterfaceWork.whiteColor;

            sfmOneTextBox.BackColor = InterfaceWork.backgroundTextColor;
            editorOnePanelTextBox.BackColor = InterfaceWork.backgroundTextColor;

            sfmTwoTextBox.BackColor = InterfaceWork.backgroundTextColor;
            editorLeftTextBox.BackColor = InterfaceWork.backgroundTextColor;
            editorRightTextBox.BackColor = InterfaceWork.backgroundTextColor;

            getAllContols(Controls[0]);

            foreach (Control item in listContols)
            {
                
                if (item is Label)
                {
                    item.ForeColor = InterfaceWork.whiteColor;
                }

                if (item is RichTextBox) 
                {
                    item.ForeColor = InterfaceWork.whiteColor;
                    item.BackColor = InterfaceWork.backgroundTextColor;
                }

                if (item is ComboBox) 
                {
                    item.BackColor = InterfaceWork.backgroundTextColor;
                }

                if (item is Button)
                {
                    item.ForeColor = InterfaceWork.whiteColor;
                }
            }

            //Убирает выделение текста в первых текстбоксах
            sfmOneTextBox.SelectionStart = sfmOneTextBox.Text.Length;
            //sfmOneTextBox.Focus();

            sfmTwoTextBox.SelectionStart = sfmTwoTextBox.Text.Length;
            //sfmTwoTextBox.Focus();

            typeBlendShapeComboBox.ForeColor = InterfaceWork.whiteColor;
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

        private void CreateBlendShape_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
