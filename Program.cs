using System;
using System.IO;

namespace ASZBlocks
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("************* Лабораторные работы по 3 лекции *************\n" +
                "************* Реализация алгоритмов по работе с Z-блоками *************\n");

            // считываем исходный текст из файла в переменную TextInFile

            Console.WriteLine("Введите название файла в котором будет происходить поиск: ");


            try
            {   // чтение данных из файла
                using (StreamReader sr = new StreamReader(Console.ReadLine()))
                {
                    string TextInFile = sr.ReadToEnd();
                    Console.WriteLine("Текст файла: ");
                    Console.WriteLine(TextInFile);
                    // метод, который демонстрирует работу алгоритмов
                    Realization(TextInFile);

                }


            }
            catch (Exception ex)
            {

                Console.WriteLine("Файл не найден");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Если хотите ввести текст вручную, напишите 1");

                if (Console.ReadLine() == "1")
                {
                    Console.WriteLine("Введите текст для работы с алгоритмами");
                    string Text = Console.ReadLine();
                    Realization(Text);
                }


            }





            Console.Read();
        }




        public static void Realization(string Text)
        {
            // создание и инициализация построения массива z-функций(преф)
            int[] zp = new int[Text.Length];
            // создание и инициализация построения массива z-функций(суф)
            int[] zs = new int[Text.Length];
            // создание и инициализация модифицированного массива граней префиксов
            int[] bpm = new int[Text.Length];
            // создание и инициализация массива граней префиксов
            int[] bp = new int[Text.Length];

            Console.WriteLine("Полученный текст: " + Text);

            Console.WriteLine("Наивный алгоритм построения массива z-функций");
            // демонстрация работы алгоритма построения массива z-функций
            NaiveZValues(Text, zp);
            Show(zp);

            Console.WriteLine("Алгоритм построения массива z-функций");
            // демонстрация работы алгоритма построения массива z-функций
            PrefixZValues(Text, zp);
            Show(zp);

            Console.WriteLine("Алгоритм построения массива cуффиксов z-функций");
            // демонстрация работы алгоритма построения массива cуффиксов z-функций
            SuffixZValues(Text, zs);
            Show(zs);

            Console.WriteLine("Алгоритм построения модифицированного массива граней префиксов");
            // демонстрация работы алгоритма построения модифицированного массива граней префиксов 
            // из массива z-функций(преф)
            ZPToBPM(zp, bpm, Text.Length);
            Show(bpm);

            Console.WriteLine("Алгоритм построения массива граней префиксов");
            // демонстрация работы алгоритма построения массива граней префиксов
            // из массива z-функций(преф)
            ZPToBP(zp, bp, Text.Length);
            Show(bp);

            Console.WriteLine("Алгоритм построения массива z-функций из массива граней префиксов");
            // демонстрация работы алгоритма построения массива z-функций
            // из массива граней префиксов(исльзуется методо PrefixBorderArray, описанный во второй лекции,
            // для независимого построения массива z-функций)
            PrefixBorderArray(Text, bp);
            BPToZP(bp, zp, Text.Length);
            Show(zp);


            Console.WriteLine();
        }

        // Метод, в котором реализован наивный алгоритм построения массива z-функций
        // параметр TextForSearching является строкой в котором идет поиск z-функций
        public static void NaiveZValues(string TextForSearching, int[] zp)
        {
            // SizeText - размерность строки
            int SizeText = TextForSearching.Length;

            zp[0] = 0;
            for (int i = 1; i < SizeText; ++i)
            {
                int j = i;
                while (j < SizeText && (TextForSearching[j] == TextForSearching[j - i]))
                    ++j;
                zp[i] = j - i;

            }

        }
        // Метод для вывода на экран содержимого массива z-функций, массива граней и т.п.
        public static void Show(int[] Array)
        {
            for (int i = 0; i < Array.Length; ++i)
            {
                Console.Write(Array[i] + " ");
            }
            Console.WriteLine();
        }


        // Метод, в котором реализован алгоритм построения массива z-функций
        // параметр TextForSearching является строкой в котором идет поиск z-функций
        public static void PrefixZValues(string TextForSearching, int[] zp)
        {
            // SizeText - размерность строки
            int SizeText = TextForSearching.Length;
            // l - последнее значение начальной позиции блока, соответствующего r
            int l = 0;
            // r - последнее значение позиции за правым концом всех Z-блоков
            int r = 0;
            zp[0] = 0;
            for (int i = 1; i < SizeText; ++i)
            {
                zp[i] = 0;
                if (i >= r)
                {// Позиция i не покрыта Z-блоком, он вычисляется заново
                    zp[i] = StrComp(TextForSearching, SizeText, 0, i);
                    l = i;
                    r = l + zp[i];
                }
                else
                {// Позиция i покрыта Z-блоком, он используется
                    int j = i - l;
                    if (zp[j] < r - i)
                        zp[i] = zp[j];
                    else
                    {
                        zp[i] = r - i + StrComp(TextForSearching, SizeText, r - i, r);
                        l = i;
                        r = l + zp[i];
                    }
                }
            }
        }

        // Метод, который возвращает наибольшую длину совпадающих подстрок
        // i1 - первый индекс подстроки
        // i2 - второй индекс подстроки
        public static int StrComp(string TextForSearching, int SizeText, int i1, int i2)
        {
            // MaxLen - наибольшая длина
            int MaxLen = 0;
            while (i1 < SizeText && i2 < SizeText && TextForSearching[i1++] == TextForSearching[i2++])
                ++MaxLen;
            return MaxLen;
        }



        // Метод, в котором реализован алгоритм построения массива z-функций(суфф)
        // параметр TextForSearching является строкой в котором идет поиск z-функций
        public static void SuffixZValues(string TextForSearching, int[] zs)
        {
            // SizeText - размерность строки
            int SizeText = TextForSearching.Length;
            // l - последнее значение начальной позиции блока, соответствующего r
            int l = SizeText - 1;
            // r - последнее значение позиции за правым концом всех Z-блоков
            int r = SizeText - 1;
            zs[SizeText - 1] = 0;

            for (int i = SizeText - 2; i >= 0; --i)
            {
                zs[i] = 0;
                if (i <= l)
                { // Позиция i не покрыта Z-блоком, он вычисляется заново
                    zs[i] = StrCompBack(TextForSearching, i, SizeText - 1);
                    r = i;
                    l = r - zs[i];
                }
                else
                {// Позиция i покрыта Z-блоком, он используется
                    int j = SizeText - (r + 1 - i);
                    if (zs[j] < i - l) zs[i] = zs[j];
                    else
                    {
                        zs[i] = i - l + StrCompBack(TextForSearching, l, SizeText - i + l); r = i; l = r - zs[i];
                    }
                }

            }

        }
        // Метод, который возвращает наибольшую длину совпадающих обратных подстрок
        // i1 - первый индекс подстроки
        // i2 - второй индекс подстроки
        public static int StrCompBack(string TextForSearching, int i1, int i2)
        {
            // MaxLen - наибольшая длина подстроки
            int MaxLen = 0;
            while (i1 >= 0 && i2 >= 0 && TextForSearching[i1--] == TextForSearching[i2--])
                ++MaxLen;
            return MaxLen;
        }


        // Метод, в котором реализован алгоритм построения модифицированного массива граней префиксов 
        // из массива z-функций(преф)
        // SizeText - размерность строки(в данном случае это максимальная размерность массива z-функций и мод. массива граней) 
        public static void ZPToBPM(int[] zp, int[] bpm, int SizeText)
        {
            // в языке c# инициализация происходит при его создании и автоматически заполняется 0
            // for (int i = 0; i < SizeText; ++i) 
            //    bpm[i] = 0;
            for (int j = SizeText - 1; j != 0; --j)
            {
                int i = j + zp[j] - 1; bpm[i] = zp[j];
            }
        }
        // Метод, в котором реализован алгоритм построения массива граней префиксов 
        // из массива z-функций(преф)
        // SizeText - размерность строки(в данном случае это максимальная размерность массива z-функций и массива граней) 
        public static void ZPToBP(int[] zp, int[] bp, int SizeText)
        {
            // в языке c# инициализация происходит при его создании и автоматически заполняется 0
            // for (i = 0; i < SizeText; ++i) 
            // bp[i] = 0; 
            for (int j = 1; j < SizeText; ++j)
                for (int i = j + zp[j] - 1; i >= j; --i)
                {
                    if (bp[i] != 0) break;
                    bp[i] = i - j + 1;
                }
        }

        // Метод, в котором реализован алгоритм построения массива z-функций
        // из массива граней префиксов
        // SizeText - размерность строки(в данном случае это максимальная размерность массива z-функций и массива граней) 
        public static void BPToZP(int[] bp, int[] zp, int SizeText)
        {
            // l - последнее значение начальной позиции блока, соответствующего r
            int l = 0;
            // r - последнее значение позиции за правым концом всех Z-блоков
            int r = 0;
            zp[0] = 0;
            for (int i = 1; i < SizeText; ++i)
            {
                zp[i] = 0;
                if (i >= r)
                { // Позиция i не покрыта Z-блоком, он вычисляется заново
                    zp[i] = ValGrow(bp, SizeText, i, 1); l = i; r = l + zp[i];
                }
                else
                {// Позиция i покрыта Z-блоком, он используется
                    int j = i - l;
                    if (zp[j] < r - i) zp[i] = zp[j];
                    else
                    {
                        zp[i] = r - i + ValGrow(bp, SizeText, r, r - i + 1); l = i; r = l + zp[i];
                    }
                }
            }
        }
        // Метод, который реализует проверку на серию в позиции nPos, начинающуюся со значения nVal
        public static int ValGrow(int[] nArr, int SizeText, int nPos, int nVal)
        {
            int nSeqLen = 0;
            while (nPos < SizeText && nArr[nPos++] == nVal++)
                ++nSeqLen;
            return nSeqLen;
        }



        // Метед, который реализует алгоритм для построения массива граней префиксов, из второй лекции
        static void PrefixBorderArray(string TextInFile, int[] bp)
        {
            int n = TextInFile.Length;
            int bpRight;
            bp[0] = 0;
            for (int i = 1; i < n; ++i)
            { // i –длина рассматриваемого префикса
                bpRight = bp[i - 1]; // Позиция справа от предыдущей грани
                while (bpRight != 0 && TextInFile[i] != TextInFile[bpRight])
                    bpRight = bp[bpRight - 1];
                // Длина на 1 больше, чем позиция
                if (TextInFile[i] == TextInFile[bpRight])
                    bp[i] = bpRight + 1;
                else bp[i] = 0;

            }


        }




    }
}

