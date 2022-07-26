using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;

namespace ConvertorAnimationSFM
{
    class MyPanel : Panel
    {
        public MyPanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }
    }

    class InterfaceWork
    {
        protected static byte chouseButton = 0; // 1 - ПРОЕКТ, 2 - ИМПОРТ, 3 - ЭКСПОРТ
        protected static byte previousСhouseButton = 3; // 0 - ПРОЕКТ, 0 - ИМПОРТ, 0 - ЭКСПОРТ, 3 - ПЕРВЫЙ ЗАПУСК
        protected static Button[] button = new Button[3];

        public static Color whiteColor;
        public static Color whiteColorPush;

        public static Color backgroundFormColor;
        public static Color backgroundTextColor;
        //public static Color backgroundColor = Color.FromArgb(43, 43, 43);

       // 28; 28; 28


        public static void initializedInerfeceWork(Button buttonProject, Button buttonImport, Button buttonExport)
        {
            //Получаем индетификаторы кнопок с главной формы
            button[0] = buttonProject;
            button[1] = buttonImport;
            button[2] = buttonExport;

            whiteColor = Color.FromArgb(230, 230, 230);
            whiteColorPush = Color.FromArgb(220, 220, 220); //Color.FromArgb(160, 160, 160)
            backgroundFormColor = Color.FromArgb(43, 43, 43);
            backgroundTextColor = Color.FromArgb(100, 100, 100);
    }

        //Переключение цвета выбранной кнопки левого меню
        public static void chancgeColorChouseButton(byte indexChouseButton)
        {
            switch (indexChouseButton)
            {
                case 0:
                    chouseButton = 0;
                    swapColorButtons(chouseButton, previousСhouseButton);
                    previousСhouseButton = 0; //Запоминаем индекс выбранной кнопки(она станет предыдущей)
                    break;
                case 1:
                    chouseButton = 1;
                    swapColorButtons(chouseButton, previousСhouseButton);
                    previousСhouseButton = 1; //Запоминаем индекс выбранной кнопки(она станет предыдущей)
                    break;
                case 2:
                    chouseButton = 2;
                    swapColorButtons(chouseButton, previousСhouseButton);
                    previousСhouseButton = 2; //Запоминаем индекс выбранной кнопки(она станет предыдущей)
                    break;
            }
        }

        //Смена цветов выбранной кнопки и предыдущей
        protected static void swapColorButtons(byte chBtn, byte prevChBtn)
        {
            //Меняем цвета выбранной внопке
            button[chBtn].BackColor = Color.FromArgb(52, 52, 52); //Цвет кнопки
            button[chBtn].FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 48, 48); //Цвет при клике
            button[chBtn].FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 52, 52); //Цвет при наведении

            if (prevChBtn < 3)//Возвращаем цвета предыдущей выбранной кнопке
            {
                button[prevChBtn].BackColor = Color.FromArgb(28, 28, 28); //Цвет кнопки
                button[prevChBtn].FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 48, 48); //Цвет при клике
                button[prevChBtn].FlatAppearance.MouseOverBackColor = Color.FromArgb(42, 42, 42); //Цвет при наведении
            }

            /*
            //Меняем цвета выбранной внопке
            button[chBtn].BackColor = Color.FromArgb(52, 52, 52); //Цвет кнопки
            button[chBtn].FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 48, 48); //Цвет при клике
            button[chBtn].FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 52, 52); //Цвет при наведении

            if (prevChBtn < 3)//Возвращаем цвета предыдущей выбранной кнопке
            {
                button[prevChBtn].BackColor = Color.FromArgb(28, 28, 28); //Цвет кнопки
                button[prevChBtn].FlatAppearance.MouseDownBackColor = Color.FromArgb(48, 48, 48); //Цвет при клике
                button[prevChBtn].FlatAppearance.MouseOverBackColor = Color.FromArgb(42, 42, 42); //Цвет при наведении
            }
             */
        }

        //Описание кастомного рендереа контекстного меню
        public class MyColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return Color.FromArgb(56, 56, 56);
                }
            }

            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return Color.FromArgb(56, 56, 56);
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return Color.FromArgb(56, 56, 56);
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return Color.FromArgb(56, 56, 56);
                }
            }

            public override Color MenuBorder //Цвет рамки меню
            {
                get
                {
                    return Color.Black;
                }
            }

            public override Color MenuItemBorder //Цвет рамки элемента меню
            {
                get
                {
                    return Color.Black;
                }
            }

            public override Color MenuItemSelected
            {
                get
                {
                    return Color.FromArgb(42, 42, 42);
                }
            }

            public override Color MenuStripGradientEnd
            {
                get
                {
                    return Color.Black;
                }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get
                {
                    return Color.FromArgb(56, 56, 56);
                }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return Color.FromArgb(28, 28, 28);
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return Color.FromArgb(28, 28, 28);
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return Color.FromArgb(28, 28, 28);
                }
            }
        }

        //Переопределение цветов для контекстного меню окна Редактор моделей
        public class MyColorTable2 : ProfessionalColorTable
        {
            Color color1 = Color.FromArgb(56, 56, 56);
            Color color2 = Color.FromArgb(28, 28, 28);
            Color color3 = Color.FromArgb(28, 28, 28);
            Color color4 = Color.FromArgb(28, 28, 28);

            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return color1;
                }
            }

            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return color1;
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return color1;
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return color1;
                }
            }

            public override Color MenuBorder //Цвет рамки меню
            {
                get
                {
                    return color4;
                }
            }

            public override Color MenuItemBorder //Цвет рамки элемента меню
            {
                get
                {
                    return color4;
                }
            }

            public override Color MenuItemSelected
            {
                get
                {
                    return color2;
                }
            }

            public override Color MenuStripGradientEnd
            {
                get
                {
                    return color4;
                }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get
                {
                    return color1;
                }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return color3;
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return color3;
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return color3;
                }
            }

        }

        //Переопределенный класс treeView(для управления цветами) 
        /*Нужно изменить тип данных у нового создаваемого treeView 
        this.treeView1 = new InterfaceWork.ClassMyTreeView(); */
        public class CategorTreeView : TreeView
        {
            public CategorTreeView()
            {
                this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            }
            protected override void OnDrawNode(DrawTreeNodeEventArgs e)
            {

                //Цвет рамки выделенного элемента
                SolidBrush colorSelectNode = new SolidBrush(Color.FromArgb(47, 47, 47)); //SystemColors.ControlLight

                Color color = Color.GreenYellow;

                //Цвет фона выделеного элемента
                Color colorBackSelectNode = Color.FromArgb(47, 47, 47);

                //Цвет текста элементов
                Color fore = e.Node.ForeColor;


                //WindowFrame
                TreeNodeStates state = e.State;
                Font font = e.Node.NodeFont ?? e.Node.TreeView.Font;

                if (fore == Color.Empty) fore = e.Node.TreeView.ForeColor;

                Brush colorFonDefuaultNodes = SystemBrushes.FromSystemColor(SystemColors.WindowFrame); //SystemColors.WindowFrame Color.FromArgb(47, 47, 47)

                if (e.Node == e.Node.TreeView.SelectedNode)
                {
                    fore = SystemColors.HighlightText;
                    e.Graphics.FillRectangle(colorSelectNode, e.Bounds); //Цвет рамки выделенного элемента
                    ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, fore, color); //Цвет рамки выделенного элемента
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, colorBackSelectNode, TextFormatFlags.GlyphOverhangPadding);
                }
                else
                {
                    e.Graphics.FillRectangle(colorFonDefuaultNodes, e.Bounds);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, TextFormatFlags.GlyphOverhangPadding);
                }
            }
        }

        public class ClassMyTreeView : TreeView
        {
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case 0x203:
                        m.Msg = 0;
                        break;
                }
                base.WndProc(ref m);
            }

            public ClassMyTreeView()
            {
                this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            }
            protected override void OnDrawNode(DrawTreeNodeEventArgs e)
            {

                //Цвет рамки выделенного элемента
                SolidBrush colorSelectNode = new SolidBrush(Color.FromArgb(47, 47, 47)); //SystemColors.ControlLight

                Color color = Color.GreenYellow;

                //Цвет фона выделеного элемента
                Color colorBackSelectNode = Color.FromArgb(47, 47, 47);

                //Цвет текста элементов
                Color fore = e.Node.ForeColor;


                //WindowFrame
                TreeNodeStates state = e.State;
                Font font = e.Node.NodeFont ?? e.Node.TreeView.Font;

                if (fore == Color.Empty) fore = e.Node.TreeView.ForeColor;

                //Brush colorFonDefuaultNodes = SystemBrushes.FromSystemColor(SystemColors.WindowFrame);
                
                SolidBrush colorFonDefuaultNodes = new SolidBrush(InterfaceWork.backgroundTextColor);

                //Brush colorFonDefuaultNodes = SystemBrushes.FromSystemColor(SystemColors.WindowFrame); //Color.FromArgb(47, 47, 47)

                if (e.Node == e.Node.TreeView.SelectedNode)
                {
                    fore = SystemColors.HighlightText;
                    e.Graphics.FillRectangle(colorSelectNode, e.Bounds); //Цвет рамки выделенного элемента
                    ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, fore, color); //Цвет рамки выделенного элемента
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, colorBackSelectNode, TextFormatFlags.GlyphOverhangPadding);
                }
                else
                {
                    e.Graphics.FillRectangle(colorFonDefuaultNodes, e.Bounds);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, TextFormatFlags.GlyphOverhangPadding);
                }
            }
        }

        public partial class NonFlickerSplitContainer : SplitContainer
        {
            public NonFlickerSplitContainer()
            {
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                              ControlStyles.UserPaint |
                              ControlStyles.OptimizedDoubleBuffer, true);

                MethodInfo objMethodInfo = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);

                object[] objArgs = new object[] { ControlStyles.AllPaintingInWmPaint |
                                      ControlStyles.UserPaint |
                                      ControlStyles.OptimizedDoubleBuffer, true };

                objMethodInfo.Invoke(this.Panel1, objArgs);
                objMethodInfo.Invoke(this.Panel2, objArgs);
            }
        }

    }
}
