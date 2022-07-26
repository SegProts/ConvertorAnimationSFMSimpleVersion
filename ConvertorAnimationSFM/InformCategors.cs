using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ConvertorAnimationSFM
{
    public static class nameSelectItem
    {
        public static String nameSelectModel;
        public static String nameSelectMainModel;

        public static String nameSelectBlendShape;
        public static String nameSelectMainBlendShape;
        public static int indexSelectBlendShape;

        public static int typeInitWindows;
        public static BlenShapeNote selectBlendShape;

        //Ошибка экспорта анимации из-за несоотвествия BlendShapes выбранной модели
        public static bool errorExprotAnamnation = false;

        public static bool modelInFileAnamnation = false;
        public static string nameModelInAnimation = "";

        //Выбран ли файл с анимацией
        public static bool choiseFile = false;
        public static bool blendShapeAnimatDone = false;

        //Имя модели загруженной анимации
        public static String nameInitModel = "";
        public static String frameRateAnimation = "";


        public static List<BlenShapeNoteAnimation> listBlendShapeAnimation = new List<BlenShapeNoteAnimation>();
    }

    public class BlenShapeNote
    {
        public String name;
        public int type;
        public String nameSFM;
        public String nameEditorLeft;
        public String nameEditorRight;

        public BlenShapeNote(String inName, int inType, String inNameSFM, String inNameEditorLeft, String inEditorRight)
        {
            name = inName;
            type = inType;
            nameSFM = inNameSFM;
            nameEditorLeft = inNameEditorLeft;
            nameEditorRight = inEditorRight;
        }
    }

    public class BlenShapeNoteAnimation
    {
        public String name;
        public List<double> listTimes;
        public List<double> listValues;

        public BlenShapeNoteAnimation(String inName, List<double> inListTimes, List<double> inListValues)
        {
            name = inName;

            listTimes = new List<double>();
            foreach (double item in inListTimes)
            {
                listTimes.Add(item);
            }

            listValues = new List<double>();
            foreach (double item in inListValues)
            {
                listValues.Add(item);
            }
        }
    }

    //Класс для хранения информации в реадкторе об BlendShape и его анимации из файла
    public class BlenShapeNoteAnimationConvert
    {
        //Запись об анимации BlendShape из файла
        public BlenShapeNoteAnimation noteBlendShapeAnim;

        //Запись об BlendShape из редактора
        public BlenShapeNote noteBlendShape;

        public BlenShapeNoteAnimationConvert(BlenShapeNoteAnimation noteAnim, BlenShapeNote noteBl)
        {
            noteBlendShapeAnim = noteAnim;
            noteBlendShape = noteBl;
        }
    }

    static public class Animation
    {
        static public List<String> GetScript(int typeEditorScript) //0- Blender, 1 - Maya
        {
            List<String> script = new List<String>();
            List<BlenShapeNoteAnimationConvert> listBlendShapesAnimationConvertor = new List<BlenShapeNoteAnimationConvert>();
            List<BlenShapeNote> blenShapeNotesConvert = new List<BlenShapeNote>();

            /*-----------------Загрузка данных об BlendShapes из Базы Данных-------------------*/
            if (File.Exists(@"ModelsResources\BlendShapes\" + nameSelectItem.nameSelectMainModel + ".db"))
            {
                SQLiteConnection DB;
                SQLiteCommand CMD;

                DB = new SQLiteConnection(@"Data Source = ModelsResources\BlendShapes\" + nameSelectItem.nameSelectMainModel + ".db; Version=3;");
                DB.Open();
                CMD = DB.CreateCommand();

                CMD.CommandText = "select * from BlendShapes";

                SQLiteDataReader SQL = CMD.ExecuteReader();
                if (SQL.HasRows)
                {
                    while (SQL.Read())
                    {
                        //Сохраняем записи всех BlendShape модели
                        blenShapeNotesConvert.Add(new BlenShapeNote(
                                SQL["name"].ToString(), Int32.Parse(SQL["type"].ToString()), SQL["nameSFM"].ToString(),
                                SQL["nameEditorLeft"].ToString(), SQL["nameEditorRight"].ToString()));
                    }
                }

                CMD.Dispose();
                DB.Close();
                DB.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            foreach (BlenShapeNoteAnimation item in nameSelectItem.listBlendShapeAnimation)
            {
                //Поиск записи об BlendShape в списке blenShapeNotesConvert
                BlenShapeNote item2 = blenShapeNotesConvert.Find(x => x.nameSFM.Contains(item.name));

                listBlendShapesAnimationConvertor.Add(new BlenShapeNoteAnimationConvert(item, item2));
            }


            script.Add("import bpy");
            script.Add("me = bpy.context.object.data");

            /*-------------------------------Составления скрипта-------------------------------*/
            //Если нужен скрипт для Blender
            if (typeEditorScript == 0)
            {

                foreach (BlenShapeNoteAnimationConvert item in listBlendShapesAnimationConvertor)
                {
                    //double[] inPutTime = new double[item.noteBlendShapeAnim.listTimes.Count];
                    //double[] inPutValues = new double[item.noteBlendShapeAnim.listTimes.Count];
                    if (item.noteBlendShape.nameSFM == "lipSideways") 
                    {
                        int g = 6;
                    }
                    List<String> listNote = printScript(item.noteBlendShapeAnim.listTimes, item.noteBlendShapeAnim.listValues, item);

                    //if (listNote.Count > 0) 
                    //{
                    if (listNote[0] != "-1")
                    {
                        foreach (String itemString in listNote)
                        {
                            script.Add(itemString);
                        }
                    }
                    //}
                }

                return script;
            }
            //Если нужен скрипт для Maya
            else
            {
                List<String> s = new List<String>();
                s.Add("");
                s.Add("");
                s.Add("");

                return s;
            }
        }

        public static List<String> printScript(List<double> times, List<double> values, BlenShapeNoteAnimationConvert item)
        {
            List<String> script = new List<String>();

            if (times.Count == 2 && values[0] == 0 && values[1] == 0)
            {
                script.Add("-1");
            }
            else
            {
                double t0; //Начало временного промежутка

                double t2; //Конец временного промежутка

                double t = 0.0417; //Время одного фрейма

                double v0; //Значение в начале временного промежутка
                double v2; //Значение в конеце временного промежутка

                double a1;
                double b1;

                double y; //Значение в выбранное время фрейма

                if (item.noteBlendShape.type == 0)
                {
                    List<double> listTimes = new List<double>();
                    List<double> listValues = new List<double>();

                    //Вставка первого значения
                    if (times[0] != 0)
                    {
                        listTimes.Add(0);
                        listValues.Add(0);

                        times.Insert(0, 0);
                        values.Insert(0, 0);
                    }
                    else
                    {
                        listTimes.Add(times[0]);
                        listValues.Add(values[0]);
                    }

                    int frame = 1; //Так как первое значение уже введено то начинаем с 1 фрейма
                    for (int i = 1; i < times.Count; i++)
                    {
                        //Вычисляем все значения для фреймов помещающихся в отрезок времени
                        while (t >= times[i - 1] && t <= times[i])
                        {
                            if (t != times[times.Count() - 1])
                            {
                                t0 = times[i - 1];
                                t2 = times[i];

                                v0 = values[i - 1];
                                v2 = values[i];

                                a1 = (v2 - v0) / (t2 - t0);
                                b1 = v0 - a1 * t0;

                                y = a1 * t + b1;

                                listTimes.Add(frame);
                                listValues.Add(y);

                                t += 0.0417;
                                frame++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    //Вставка последнего значения
                    listTimes.Add(frame++);
                    listValues.Add(values[values.Count - 1]);

                    for (int i = 0; i < listTimes.Count(); i++)
                    {
                        script.Add("bpy.context.scene.frame_set(" + listTimes[i] + ")");

                        script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].value = " +
                            String.Format("{0:0.0000000000}", listValues[i]).Replace(",", "."));

                        script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].keyframe_insert(data_path = \"value\")");
                        /*script.Add(String.Format("{0:0}", listTimes[i]).Replace(",", ".") + " : " +
                                String.Format("{0:0.0000000000}", listValues[i]).Replace(",", ".")*/
                    }
                }

                if (item.noteBlendShape.type == 1)
                {
                    List<double> listTimesLeft = new List<double>();
                    List<double> listTimesRight = new List<double>();

                    List<double> listValuesLeft = new List<double>();
                    List<double> listValuesRight = new List<double>();

                    List<double> timesLeft = new List<double>();
                    List<double> timesRight = new List<double>();

                    List<double> valuesLeft = new List<double>();
                    List<double> valuesRight = new List<double>();

                    //List<double>

                    //Вставка первого значения
                    if (times[0] != 0)
                    {
                        times.Insert(0, 0);
                        values.Insert(0, 0.5);
                    }

                    for (int i = 0; i < times.Count; i++)
                    {
                        if (values[i] == 0.5)
                        {
                            listTimesLeft.Add(times[i]);
                            listValuesLeft.Add(0);
                            listTimesRight.Add(times[i]);
                            listValuesRight.Add(0);
                        }

                        if (values[i] < 0.5)
                        {
                            listTimesLeft.Add(times[i]);
                            listValuesLeft.Add(Math.Round(values[i] != 0.5 ? Math.Abs(values[i] * 2.0 - 1.0) : 0.0, 6));
                        }
                        if (values[i] > 0.5)
                        {
                            listTimesRight.Add(times[i]);
                            if (values[i] > 1)
                            {
                                listValuesRight.Add(1);
                            }
                            else
                            {
                                listValuesRight.Add(Math.Round(values[i] != 0.5 ? Math.Abs(values[i] * 2.0 - 1.0) : 0.0, 6));
                            }
                        }
                    }

                    if (item.noteBlendShape.nameSFM == "lipSideways")
                    {
                        int h = 6;
                    }


                    int frame = 1; //Так как первое значение уже введено то начинаем с 1 фрейма
                    for (int i = 1; i < listTimesLeft.Count; i++)
                    {
                        //Вычисляем все значения для фреймов помещающихся в отрезок времени
                        if (listTimesLeft.Count != 2 && listValuesLeft[0] != 0 && listValuesLeft[1] != 0)
                        {

                        }
                        while (t >= listTimesLeft[i - 1] && t <= listTimesLeft[i]) //ни разу не выполнилось условие
                        {
                            if (t != listTimesLeft[listTimesLeft.Count() - 1])
                            {
                                t0 = listTimesLeft[i - 1];
                                t2 = listTimesLeft[i];

                                v0 = listValuesLeft[i - 1];
                                v2 = listValuesLeft[i];

                                a1 = (v2 - v0) / (t2 - t0);
                                b1 = v0 - a1 * t0;

                                y = a1 * t + b1;

                                //script.Add("bpy.context.scene.frame_set(" + frame + ")");

                                //script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].value = " +
                                    //String.Format("{0:0.0000000000}", y).Replace(",", "."));

                                //script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].keyframe_insert(data_path = \"value\")");
                                
                                timesLeft.Add(frame);
                                valuesLeft.Add(y);

                                t += 0.0417;
                                frame++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    frame = 1; //Так как первое значение уже введено то начинаем с 1 фрейма
                    t = 0.0417;
                    for (int i = 1; i < listTimesRight.Count; i++)
                    {
                        //Вычисляем все значения для фреймов помещающихся в отрезок времени
                        while (t >= listTimesRight[i - 1] && t <= listTimesRight[i])
                        {
                            if (t != listTimesRight[listTimesRight.Count() - 1])
                            {
                                t0 = listTimesRight[i - 1];
                                t2 = listTimesRight[i];

                                v0 = listValuesRight[i - 1];
                                v2 = listValuesRight[i];

                                a1 = (v2 - v0) / (t2 - t0);
                                b1 = v0 - a1 * t0;

                                y = a1 * t + b1;

                                timesRight.Add(frame);
                                valuesRight.Add(y);

                                //script.Add("bpy.context.scene.frame_set(" + frame + ")");

                                //script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorRight + "'].value = " +
                                //    String.Format("{0:0.0000000000}", y).Replace(",", "."));

                                //script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorRight + "'].keyframe_insert(data_path = \"value\")");
                                
                                t += 0.0417;
                                frame++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    for (int j = 0; j < values.Count; j++) 
                    {
                        values[j] = Math.Round(values[j] != 0.5 ? Math.Abs(values[j] * 2.0 - 1.0) : 0.0, 6);
                    }

                    //Вставка последнего значения
                    if (values[values.Count - 1] < 0.5)
                    {
                        timesLeft.Add(frame++);
                        valuesLeft.Add(Math.Round(values[values.Count - 1] != 0.5 ? Math.Abs(values[values.Count - 1] * 2.0 - 1.0) : 0.0, 6));
                        //script.Add("bpy.context.scene.frame_set(" + frame++ + ")");

                        //script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].value = " +
                        //    String.Format("{0:0.0000000000}", 
                        //        Math.Round(listValuesRight[listValuesLeft.Count - 1] != 0.5 ? 
                        //            Math.Abs(listValuesRight[listValuesLeft.Count - 1] * 2.0 - 1.0) : 0.0, 6)).Replace(",", "."));

                        //script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].keyframe_insert(data_path = \"value\")");
                        
                    }
                    if (values[values.Count - 1] > 0.5)
                    {
                        timesRight.Add(frame++);
                        valuesRight.Add(Math.Round(values[values.Count - 1] != 0.5 ? Math.Abs(values[values.Count - 1] * 2.0 - 1.0) : 0.0, 6));
                        /*script.Add("bpy.context.scene.frame_set(" + frame++ + ")");

                        script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].value = " +
                            String.Format("{0:0.0000000000}",
                                Math.Round(listValuesRight[listValuesRight.Count - 1] != 0.5 ?
                                    Math.Abs(listValuesRight[listValuesRight.Count - 1] * 2.0 - 1.0) : 0.0, 6)).Replace(",", "."));

                        script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorRight + "'].keyframe_insert(data_path = \"value\")");
                        */
                    }

                    //Переводим расчитанные значения для фреймов в скрипт
                    /*for (int i = 0; i < listValuesLeft.Count(); i++)
                    {
                        //Выставляем нулевые значения там где установленно значения для другого BlendShape
                        if (listValuesRight.Contains(listValuesLeft[i]))
                        {
                            script.Add("bpy.context.scene.frame_set(" + listTimesLeft[i] + ")");

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].value = " +
                                String.Format("{0:0.0000000000}", 0).Replace(",", "."));

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].keyframe_insert(data_path = \"value\")");
                        }
                        else
                        {
                            script.Add("bpy.context.scene.frame_set(" + listTimesLeft[i] + ")");

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].value = " +
                                String.Format("{0:0.0000000000}", listTimesLeft[i]).Replace(",", "."));

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorLeft + "'].keyframe_insert(data_path = \"value\")");
                        }

                        //script.Add(String.Format("{0:0}", listTimes[i]).Replace(",", ".") + " : " +
                        //String.Format("{0:0.0000000000}", listValues[i]).Replace(",", ".")
                    }

                    for (int i = 0; i < listValuesRight.Count(); i++)
                    {
                        //Выставляем нулевые значения там где установленно значения для другого BlendShape
                        if (listValuesLeft.Contains(listValuesRight[i]))
                        {
                            script.Add("bpy.context.scene.frame_set(" + listValuesRight[i] + ")");

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorRight + "'].value = " +
                                String.Format("{0:0.0000000000}", 0).Replace(",", "."));

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorRight + "'].keyframe_insert(data_path = \"value\")");
                        }
                        else
                        {
                            script.Add("bpy.context.scene.frame_set(" + listValuesRight[i] + ")");

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorRight + "'].value = " +
                                String.Format("{0:0.0000000000}", listValuesRight[i]).Replace(",", "."));

                            script.Add("me.shape_keys.key_blocks['" + item.noteBlendShape.nameEditorRight + "'].keyframe_insert(data_path = \"value\")");
                        }
                        //script.Add(String.Format("{0:0}", listTimes[i]).Replace(",", ".") + " : " +
                        //String.Format("{0:0.0000000000}", listValues[i]).Replace(",", ".")
                    }*/
                }

                //Удаление нулевых значений спереди
                /*for (int i = 0; i < listValues.Count(); i++)
                {
                    if (listValues[i] != 0)
                    {
                        //Вычисляем сколько раз нужно удалить первую строку списка, чтобы осталось всего одно 0 значение
                        int n = i - 2;
                        while (n >= 0)
                        {
                            listTimes.RemoveAt(0);
                            listValues.RemoveAt(0);
                            n--;
                        }
                        break;
                    }
                }*/

                //Удаление нулевых значений сзади
                /*for (int i = listValues.Count() - 1; i >= 0; i--)
                {
                    if (listValues[i] != 0)
                    {
                        int n = i + 2;
                        while (n >= 0)
                        {
                            listTimes.RemoveAt(listTimes.Count() - 1);
                            listValues.RemoveAt(listValues.Count() - 1);
                            n--;
                        }
                        break;
                    }
                }*/
            }

            return script;
        }
    }
}
