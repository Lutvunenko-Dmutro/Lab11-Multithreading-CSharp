using System;
using System.Text;
using System.Threading;

namespace Lab11
{
    class Program
    {
        // Глобальний об'єкт для синхронізації кольорів (щоб кольори не змішувались)
        static readonly object consoleLock = new object();

        // Масив для Завдання №2
        static int[] task2Array = new int[15];

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Лабораторна робота №11 | Литвиненко Дмитро | Варіант 8";

            while (true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.WriteLine("=============================================");
                Console.WriteLine("    ЛАБОРАТОРНА РОБОТА №11 (ПОТОКИ)");
                Console.WriteLine("    Виконав: Литвиненко Дмитро (Вар. 8)");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. Запустити Завдання №1 (Символи '=' та Числа)");
                Console.WriteLine("2. Запустити Завдання №2 (Масиви: Парні індекси vs Квадрати)");
                Console.WriteLine("0. Вихід");
                Console.WriteLine("=============================================");
                Console.Write("Ваш вибiр > ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunTask1();
                        break;
                    case "2":
                        RunTask2();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Натисніть Enter...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        // ========================================================
        // ЗАВДАННЯ №1 (Варіант 8)
        // Т0: Виводить 5 символів «=».
        // Т1: Виводить 8 випадкових цілих чисел (0..10).
        // ========================================================
        static void RunTask1()
        {
            Console.Clear();
            PrintHeader("ЗАВДАННЯ 1: Потоки символів і чисел");

            Thread t0 = new Thread(Task1_PrintEquals);
            Thread t1 = new Thread(Task1_PrintRandomNumbers);

            t0.Start();
            t1.Start();

            t0.Join();
            t1.Join();

            PrintFooter();
        }

        static void Task1_PrintEquals()
        {
            for (int i = 0; i < 5; i++)
            {
                PrintColored("T0: = ", ConsoleColor.Cyan);
                Thread.Sleep(200); // Затримка для демонстрації паралельності
            }
        }

        static void Task1_PrintRandomNumbers()
        {
            Random random = new Random();
            for (int i = 0; i < 8; i++)
            {
                int num = random.Next(0, 11);
                PrintColored($"T1: {num} ", ConsoleColor.Yellow);
                Thread.Sleep(200);
            }
        }

        // ========================================================
        // ЗАВДАННЯ №2 (Варіант 8)
        // Т0: Вивести всі елементи з парними індексами.
        // Т1: Вивести квадрати всіх елементів з непарними індексами.
        // ========================================================
        static void RunTask2()
        {
            Console.Clear();
            PrintHeader("ЗАВДАННЯ 2: Обробка масиву у потоках");

            // 1. Ініціалізація масиву
            Random rand = new Random();
            Console.Write("Згенерований масив: [ ");
            for (int i = 0; i < task2Array.Length; i++)
            {
                task2Array[i] = rand.Next(1, 20);
                Console.Write(task2Array[i] + " ");
            }
            Console.WriteLine("]\n");

            // 2. Створення потоків
            Thread t0 = new Thread(Task2_EvenIndices);
            Thread t1 = new Thread(Task2_OddSquares);

            t0.Start();
            t1.Start();

            t0.Join();
            t1.Join();

            PrintFooter();
        }

        static void Task2_EvenIndices()
        {
            PrintColored("\n[T0 Старт: Парні індекси]\n", ConsoleColor.Cyan);
            for (int i = 0; i < task2Array.Length; i += 2)
            {
                PrintColored($"T0 [idx {i}]: {task2Array[i]}\n", ConsoleColor.Cyan);
                Thread.Sleep(150);
            }
        }

        static void Task2_OddSquares()
        {
            PrintColored("\n\t[T1 Старт: Квадрати непарних]\n", ConsoleColor.Yellow);
            for (int i = 1; i < task2Array.Length; i += 2)
            {
                int square = task2Array[i] * task2Array[i];
                PrintColored($"\tT1 [idx {i}]: {task2Array[i]}^2 = {square}\n", ConsoleColor.Yellow);
                Thread.Sleep(150);
            }
        }

        // ========================================================
        // ДОПОМІЖНІ МЕТОДИ (Краса та Потокобезпечний вивід)
        // ========================================================

        // Метод для кольорового виводу, захищений від змішування кольорів
        static void PrintColored(string message, ConsoleColor color)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                Console.Write(message);
                Console.ResetColor();
            }
        }

        static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"--- {title} ---");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void PrintFooter()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Потоки завершили роботу. Натисніть будь-яку клавішу...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}