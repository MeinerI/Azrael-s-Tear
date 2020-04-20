//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
  using System; using System.IO; using System.Linq; using System.Text; using System.Collections; 
  using System.Collections.Generic; using System.Text.RegularExpressions; using System.Globalization; 
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

sealed class Test {

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    static string[] readText; 

    static List<string> newlist = new List<string>();

    static int i = 0; // общий "указатель" строк

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public static void Main()
    {
        SearchOption SOAD = SearchOption.AllDirectories;
        string[] filesName = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.XAF",  SOAD); 
        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

        foreach (var file in filesName)
        {
            List<XMaterial> Materials = new List<XMaterial>(); 
            List<Frame> Frames = new List<Frame>(); 
            List<Mesh> Meshes = new List<Mesh>(); 

            readText = File.ReadAllLines(file); // для прохода по строкам и поиска "шаблонов"

        //  создаём список в котором будет искать фреймы и удалять лишние строки
        //  создаём именно здесь, потому что далее содержимое readText будет изменяться 

            foreach (var o in readText)
            {
                if (o.Contains("animation_set {")) break; // отбрасываем лишние строки с Анимацией
                newlist.Add(o);
            }

            XAnimationSet animset = new XAnimationSet(); //  этот блок вроде он всегда один

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

            for ( i = 0; i < readText.Length; i++ ) // Ищем материалы
            {
                // Материалы // лучше искать отдельно для каждого файла

                if ( readText[i].Contains("material \"") && readText[i].EndsWith("\" {") ) 
                {
                    XMaterial xmat = new XMaterial(); // СОЗДАЁМ НОВЫЙ ОБЪЕКТ
                    xmat.Name = Regex.Split(readText[i],"\"")[1]; // первая строка - это всегда имя материала

                    for ( int iii = 1; iii < 4; iii++ ) // цикл по 3 следущим строкам
                    {
                        if (readText[i+iii].Contains("texture")) 
                            xmat.Filename = Regex.Split(readText[i+iii],"\"")[1]; // имя текстуры 

                        if (readText[i+iii].Contains("colour")) //  colour { 0, 0, 0, 0 }
                        {
                            MatchCollection allNums = Regex.Matches(readText[i+iii], @"\d+"); 
                            xmat.FaceColor.R = float.Parse(allNums[0].Value); 
                            xmat.FaceColor.G = float.Parse(allNums[1].Value); 
                            xmat.FaceColor.B = float.Parse(allNums[2].Value); 
                            xmat.FaceColor.A = float.Parse(allNums[3].Value); 
                        }

                        if (readText[i+iii].Contains("specular")) //  specular { 0, 15 }
                        {
                            MatchCollection allNums = Regex.Matches(readText[i+iii], @"\d+"); 
                            xmat.Intensity = float.Parse(allNums[0].Value); 
                            xmat.Power     = float.Parse(allNums[1].Value); 
                        }
                    }
                      Materials.Add(xmat); // ДОБАВЛЯЕМ МАТЕРИАЛ В СПИСОК 
                }
            }

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

            for ( i = 0; i < readText.Length; i++ ) 
            {
                // Фреймы //жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
                
                if ((readText[i].Contains("object")) && (readText[i].Contains("{")))
                {
                    Frame frame = new Frame();
                    frame.FrameName = "Frame " + Regex.Split(readText[i],"\"")[1] + " { //начало фрейма";
                    frame.FrameTransformMatrix = ReadMatrix(i+1);
                    frame.FrameMeshName = "{ " + frame.FrameName + " }";
                    Frames.Add(frame);
                } 
            }

                // Меши //жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

            for ( i = 0; i < readText.Length; i++ ) 
            {
                if (readText[i].Contains("mesh {")) // 
                {
                    Mesh mesh = new Mesh();
                    mesh.MeshName = Regex.Split(readText[i-7],"\"")[1];

                    for(;;)
                    {
                        i++; // в цикле опускаемся каждый раз на строку ниже от "mesh {"
                        // пока не достигнем двух идущих подряд строк содержащих "}" означающую конец МЕШа

                        if (readText[i].Contains("}") && readText[i+1].Contains("}")) break; 
                        if (readText[i].Contains("}") && readText[i+1].Contains("object \"")) break; // -_- долго искал этот косяк -_-

                        if (readText[i].Contains("vertices {"))             mesh.MeshVertices = ReadVector2or3(i);
                        if (readText[i].Contains("normals {"))              mesh.MeshNormals = ReadVector2or3(i);
                        if (readText[i].Contains("texture_coordinates {"))  mesh.MeshTextureCoords = ReadVector2or3(i);

                        if (readText[i].Contains("faces {"))
                        {
                            List<string> findex, nordex;
                            ReadFaces(i, out findex, out nordex);
                            mesh.FacesIndices = findex;
                            mesh.NormalsIndices = nordex;
                        }

                        if (readText[i].Contains("face_materials {"))
                        {
                            int offset = i;
                            for (;;)
                            {
                                offset++; // шагаем по строкам
                                if (readText[offset].Contains("}")) break;
                                mesh.face_materials.Add(Regex.Split(readText[offset],"\"")[1]);
                            }
                        }

                        if (readText[i].Contains("material \"") && readText[i].EndsWith("\"") )
                            mesh.MeshMaterial = "{ " + Regex.Split(readText[i],"\"")[1] + " }"; // имя материала
                    }
                        Meshes.Add(mesh);
                } // if 
            }

              // анимация //жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

            for ( i = 0; i < readText.Length; i++ ) 
            {
                if (readText[i].Contains("animation_set {")) // найден набор анимаций
                {
                    int ii = 0;  // счётчик

                    for(;;) // цикл по всем следующим строкам
                    {
                        if ( readText[i+ii].Contains("animation ") && readText[i+ii].EndsWith(" {") ) 
                        {
                            XAnimation anima = new XAnimation();
                            anima.Name = " Animation " + Regex.Split(readText[i+ii],"\"")[1] + " {"; // имя анимации
                            anima.Keys = new List<XAnimationKey>();

                            for(;;) 
                            {
                                if (readText[i+ii].Contains("object_name ")) // object_name "dummy"
                                {
                                    anima.FrameReference = "  { " + Regex.Split(readText[i+ii],"\"")[1] + " }"; // имя ссылки на фрейм
                                }

                                if (readText[i+ii].Contains("rotate_keys {"))
                                {
                                    int iii = 1;
                                    XAnimationKey rotate_keys = new XAnimationKey();
                                    rotate_keys.Name = "rotate_keys";

                                    for (;;)
                                    {
                                        string[] numbers = readText[i+ii+iii].Split(new char[] { ',' },   
                                                            StringSplitOptions.RemoveEmptyEntries);

                                        foreach ( var f in numbers ) rotate_keys.KeysF.Add(float.Parse(f));  

                                        rotate_keys.KeysS.Add(numbers[0] + ";4;" + numbers[1] + "," + numbers[2] + "," + numbers[3] + "," + numbers[4] + ";;,");

                                        iii++ ;
                                        if ( readText[i+ii+iii].Contains("}") ) break;
                                    }
                                        rotate_keys.KeysS.Insert(0, "  AnimationKey rot {");
                                        rotate_keys.KeysS.Insert(1, "  0;");
                                        rotate_keys.KeysS.Insert(2, "  " + (rotate_keys.KeysS.Count-2) + ";" );
                                        rotate_keys.KeysS[rotate_keys.KeysS.Count-1] = rotate_keys.KeysS[rotate_keys.KeysS.Count-1].Replace(";;,",";;;");
                                        rotate_keys.KeysS.Add("  }");

                                        anima.Keys.Add(rotate_keys);
                                }

                                if (readText[i+ii].Contains("scale_keys {"))
                                {
                                    int iii = 1;
                                    XAnimationKey scale_keys = new XAnimationKey();
                                    scale_keys.Name = "scale_keys";

                                    for (;;)
                                    {
                                        string[] numbers = readText[i+ii+iii].Split(new char[] { ',' },   
                                                            StringSplitOptions.RemoveEmptyEntries);

                                        foreach ( var f in numbers ) scale_keys.KeysF.Add(float.Parse(f));  

                                        scale_keys.KeysS.Add(numbers[0] + ";3;" + numbers[1] + "," + numbers[2] + "," + numbers[3] + ";;,");

                                        iii++ ;
                                        if ( readText[i+ii+iii].Contains("}") ) break;
                                    }
                                        scale_keys.KeysS.Insert(0, "  AnimationKey scale {");
                                        scale_keys.KeysS.Insert(1, "  1;");
                                        scale_keys.KeysS.Insert(2, "  " + (scale_keys.KeysS.Count-2) + ";" );
                                        scale_keys.KeysS[scale_keys.KeysS.Count-1] = scale_keys.KeysS[scale_keys.KeysS.Count-1].Replace(";;,",";;;");
                                        scale_keys.KeysS.Add("  }");

                                        anima.Keys.Add(scale_keys);
                                }

                                if (readText[i+ii].Contains("position_keys {"))
                                {
                                    int iii = 1;
                                    XAnimationKey position_keys = new XAnimationKey();
                                    position_keys.Name = "position_keys";

                                    for (;;)
                                    {
                                        string[] numbers = readText[i+ii+iii].Split(new char[] { ',' },   
                                                            StringSplitOptions.RemoveEmptyEntries);

                                        foreach ( var f in numbers ) position_keys.KeysF.Add(float.Parse(f));  

                                        position_keys.KeysS.Add(numbers[0] + ";3;" + numbers[1] + "," + numbers[2] + "," + numbers[3] + ";;,");

                                        iii++ ;
                                        if ( readText[i+ii+iii].Contains("}") ) break;
                                    }
                                        position_keys.KeysS.Insert(0, "  AnimationKey pos {");
                                        position_keys.KeysS.Insert(1, "  2;");
                                        position_keys.KeysS.Insert(2, "  " + (position_keys.KeysS.Count-2) + ";" );
                                        position_keys.KeysS[position_keys.KeysS.Count-1] = position_keys.KeysS[position_keys.KeysS.Count-1].Replace(";;,",";;;");
                                        position_keys.KeysS.Add("  }");

                                        anima.Keys.Add(position_keys);
                                }
                                
                                if ( readText[i+ii].Contains("}") && readText[i+ii+1].Contains("}") ) break;

                                ii++;
                            }
                                ii++;
                            animset.Animations.Add(anima);  //  добавляем анимации с ключами в набор
                        }

                        i++; 

                        if ( (i+ii) - readText.Length == 0 ) // конец файла
                        break;  //  это выход для [animation_set]

                    } //for 

                }//if анимация

            } // for // чтение всех строк

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

// СУПЕР КОСТЫЛЬ ОСТАВЛЯЮЩИЙ ФРЕЙМЫ И УБИРАЮЩИЙ ВСЁ ЛИШНЕЕ 
// иначе нужно юзать реккурсию и список дочерних фреймов для "Объектов"

 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains(",")) { newlist[i] = ""; } } // удаляем все строки содержащие запятые 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str)); // каждый раз удаляем образовавшиеся пустые строки
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("faces {")) { newlist[i+0] = ""; newlist[i+1] = ""; newlist[i+2] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("matrix {")) { newlist[i] = ""; newlist[i+1] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("texture \"")) { newlist[i] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("normals {")) { newlist[i] = ""; newlist[i+1] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("texture_coordinates {")) { newlist[i] = ""; newlist[i+1] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("vertices {")) { newlist[i] = ""; newlist[i+1] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("material \"") && newlist[i].EndsWith("\"") ) { newlist[i] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("mesh {")) { newlist[i] = ""; newlist[i+1] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("material") && newlist[i].Contains("{")) { newlist[i] = ""; newlist[i+1] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));
 for ( i = 0; i < newlist.Count; i++ ) { if (newlist[i].Contains("face_materials")) { newlist[i] = ""; newlist[i+1] = ""; } } 
 newlist.RemoveAll(str => String.IsNullOrWhiteSpace(str));

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    using (StreamWriter new_xaf = new StreamWriter(file + ".x"))   new_xaf.Write("xof 0303txt 0032\n\n");   //   HEADER

//  можно записывать всё в одном блоке using 

    using (StreamWriter new_xaf = File.AppendText(file + ".x")) 
    {

//  ЗАПИСЫВАЕМ  МАТЕРИАЛЫ  В  ФАЙЛ

        if ( Materials.Count > 0 )
        {
            for ( int mati = 0; mati < Materials.Count; mati++ )
            {
                new_xaf.WriteLine("Material " + Materials[mati].Name + " {");
                new_xaf.WriteLine(Materials[mati].FaceColor);
                new_xaf.WriteLine(Materials[mati].Power + ";" );
                new_xaf.WriteLine("0.000000;0.000000;0.000000;;");
                new_xaf.WriteLine("0.000000;0.000000;0.000000;;");

                if (Materials[mati].Filename == "" || Materials[mati].Filename == " " 
                 || Materials[mati].Filename == String.Empty || Materials[mati].Filename == null )  new_xaf.WriteLine("}");
                else new_xaf.WriteLine("TextureFilename { \"" + Materials[mati].Filename + "\"; } }\n");
            }
              Materials.Clear();
        }

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

// ЗАПИСЫВАЕМ  Фреймы  И  МЕШИ  В  ФАЙЛ

// теперь надо пробежаться по newlist списку
// чтобы запихнуть после его матрицы - тело соответствующего меша
// таким образом нужные скобки сдвинуться

// лучше заюзаю новый список // или юзай метод отсюда https://stackoverflow.com/questions/9462518/short-way-to-add-item-after-item-in-list

int co = 0;

List<string> NewNewList = new List<string>();

foreach (var o in newlist) // идём по строкам в новом списке
{
  //if (newlist.Contains("Frame ")) // если встретился объект

    if ( o.Contains("object") /*&& o.Contains("{")*/ )
    {
        NewNewList.Add(Frames[co].FrameName);
        NewNewList.Add(Frames[co].FrameTransformMatrix);
        NewNewList.Add(Meshes[co].ToString());
        co++; // переходим к следущему фрейму и его мешу
    }
    if ( o.Contains("}"))
    {
        NewNewList.Add(o + " // конец фрейма"); // ставим закрывающую скобку
    }

    // if ( co > ( Frames.Count - 1 ) ) break; 
    // и если нет больше фреймов с мешами - выходим
    // эта строчка тоже вызывала ошибку 
    // не добавляла последнюю скобку закрывающую фрейм
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

// ЗАПИСЫВАЕМ  Фреймы  И  МЕШИ  В  ФАЙЛ

foreach ( var str in NewNewList ) 
{
    new_xaf.WriteLine(str);
}

Frames.Clear();
Meshes.Clear();

NewNewList.Clear();
newlist.Clear();

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

//  ЗАПИСЫВАЕМ  АНИМАЦИИ  В  ФАЙЛ

        if (animset.Animations.Count > 0)
        {
            new_xaf.WriteLine("\nAnimationSet {\n");

            foreach (var o in animset.Animations)
            {
                new_xaf.WriteLine(o.Name + "\n" + o.FrameReference); 
                new_xaf.WriteLine("  AnimationOptions { 1; 0; } \n");

                foreach (var oo in o.Keys)
                    foreach (var ooo in oo.KeysS)
                        new_xaf.WriteLine(ooo);

                  new_xaf.WriteLine(" }");  //  для каждого блока Animation
            }
              new_xaf.WriteLine("}");  //  для блока AnimationSet

              animset.Animations.Clear();
        }

    }  // запись в файл 

animset.Animations.Clear();
animset = null;

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

        } // foreach (var file in filesName)

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    } // public static void Main()

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

  public static string ReadMatrix(int offset)
  {
      string Matrix = "\n" + readText[offset+1] + 
      "\n" + readText[offset+2] + "\n" + readText[offset+3] + "\n" + readText[offset+4];

      char[] chars = Matrix.ToCharArray(); // преобразовали строку в массив символов
      chars[Matrix.Length - 1] = ';'; // заменили последнюю ',' на ';'
      Matrix = new string(chars); // создали новую строку из массива символов
    //Matrix = Regex.Replace(Matrix, " {2,}", " "); // замена двойных пробелов одинарными

      Matrix = "  FrameTransformMatrix { " + Matrix + "; }\n"; // добавили необходимые "шаблоны"
      return Matrix;
  }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

  public static List<string> ReadVector2or3(int offset)
  {
      List<string> VectorXYz = new List<string>();

  //  offset++; // со следующей за "VectorXYz {" стоки
  //  while ( !readText[offset].Contains("}") ) 

      for (;;)  //  читаем строки, пока не встретится "}"
      {
          offset++; // шагаем по строкам
          if (readText[offset].Contains("}")) break;
          string xyz = readText[offset].Replace(',', ';') + ",";
          VectorXYz.Add(xyz);
      }

      char[] chars = VectorXYz[VectorXYz.Count - 1].ToCharArray();
      chars[VectorXYz[VectorXYz.Count - 1].Length - 1] = ';';
      VectorXYz[VectorXYz.Count - 1] = new string(chars);

  //  добаляем в начало списка его длину
      VectorXYz.Insert(0, VectorXYz.Count + ";");

      return VectorXYz;
  }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

  public static void ReadFaces(int offset, out List<string> FI, out List<string> NI)
  {
      FI = new List<string>();
      NI = new List<string>();

      for (;;)
      {
          offset++; 
          if (readText[offset].Contains("}") || readText[offset].EndsWith("0") ) break;

          string[] indices = readText[offset].Split(',');

          FI.Add( "3;" + indices[1] + "," + indices[3] + "," + indices[5] + ";,");
          NI.Add( "3;" + indices[2] + "," + indices[4] + "," + indices[6] + ";,");
      }

          char[] chars = FI[FI.Count - 1].ToCharArray();
          chars[FI[FI.Count - 1].Length - 1] = ';';
          FI[FI.Count - 1] = new string(chars);
          FI.Insert(0, FI.Count + ";");

          chars = NI[NI.Count - 1].ToCharArray();
          chars[NI[NI.Count - 1].Length - 1] = ';';
          NI[NI.Count - 1] = new string(chars);
          NI.Insert(0, NI.Count + ";");
  }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

} // sealed class Test

//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public sealed class Mesh
    {
        public List<String> MeshBody = new List<String>();

        public string MeshName { get; set; }
        public string MeshMaterial { get; set; }

        public List<String> MeshVertices = new List<String>();
        public List<String> MeshNormals = new List<String>();
        public List<String> MeshTextureCoords = new List<String>();

        public List<String> FacesIndices = new List<String>();
        public List<String> NormalsIndices = new List<String>();

        public List<String> face_materials = new List<String>();

        public override string ToString() 
        { 
            MeshBody.Add("\nMesh " + MeshName + " {" + "// начало меша" + "\n");
            foreach (var vertex in MeshVertices) MeshBody.Add(vertex + "\n"); MeshBody.Add("\n");
            foreach (var faces  in FacesIndices) MeshBody.Add(faces + "\n");

            MeshBody.Add("\nMeshNormals {\n");
            foreach (var normals in MeshNormals) MeshBody.Add(normals + "\n"); MeshBody.Add("\n");
            foreach (var nordex  in NormalsIndices) MeshBody.Add(nordex + "\n"); MeshBody.Add("\n}");

            if (MeshTextureCoords.Count > 0)
            {
                MeshBody.Add("\nMeshTextureCoords {\n");
                foreach (var UVs in MeshTextureCoords) MeshBody.Add(UVs + "\n");
                MeshBody.Add("}\n\n");
            }

            if ( MeshMaterial != null )
            {
                MeshBody.Add("MeshMaterialList {\n1;\n1;\n0;\n");
                MeshBody.Add(MeshMaterial + "\n}\n");
            }

            MeshBody.Add("} // конец меша"); // закрываем меш

            return String.Join("", MeshBody.ToArray());
        }
    }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public sealed class Frame
    {
        public string FrameName { get; set; }
        public string FrameTransformMatrix { get; set; }
        public string FrameMeshName { get; set; }
    }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public sealed class XColorRgba
    {
        public float R { get; set; }    public float G { get; set; }
        public float B { get; set; }    public float A { get; set; }

        public override string ToString() { 
          return string.Format("{0}; {1}; {2}; {3};;", R, G, B, A); }
    }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public sealed class XMaterial
    {
        public string Name     { get; set; }    public float Intensity { get; set; }
        public string Filename { get; set; }    public float Power     { get; set; }
        
        public XColorRgba FaceColor { get; set; }
        public XMaterial() { FaceColor = new XColorRgba(); }

        public override string ToString() { 
          return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", Name, Filename, FaceColor, Intensity, Power); }
    }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public sealed class XAnimationSet {
    //  public string Name { get; set; }  //  вроде нигде имён для AnimationSet нет  
    //  желательно создать для каждой анимации свой отдельный Набор, чтобы легче было их проигрывать независимо
        public List<XAnimation> Animations = new List<XAnimation>();
    }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public sealed class XAnimation {
        public string Name { get; set; }
        public string FrameReference { get; set; }
        public List<XAnimationKey> Keys = new List<XAnimationKey>();
    }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж

    public sealed class XAnimationKey {
        public string Name { get; set; }
        public List<float> KeysF = new List<float>();
        public List<string> KeysS = new List<string>();
    }
//жжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
