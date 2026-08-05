using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using Spectre.Console;

namespace AsyncShowcase
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--benchmark")
            {
                BenchmarkRunner.Run<MultithreadingBenchmarks>();
                return;
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Async Showcase | .NET Concurrency";

            // 1. Кероване скасування (CancellationToken)
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true; 
                cts.Cancel();
                AnsiConsole.MarkupLine("\n[red]Отримано сигнал зупинки. Коректне завершення потоків...[/]");
            };

            while (!cts.Token.IsCancellationRequested)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("Async Showcase")
                        .LeftJustified()
                        .Color(Color.Green));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[cyan]Оберіть дію (використовуйте стрілки):[/]")
                        .PageSize(5)
                        .AddChoices(new[] {
                            "1. Демонстрація Thread-Safety (Символи та числа)",
                            "2. Асинхронна обробка масивів",
                            "3. Запустити Benchmarks (Продуктивність)",
                            "0. Вихід"
                        }));

                if (choice.StartsWith("0"))
                    break;
                
                try
                {
                    if (choice.StartsWith("1"))
                        await RunTask1Async(cts.Token);
                    else if (choice.StartsWith("2"))
                        await RunTask2Async(cts.Token);
                    else if (choice.StartsWith("3"))
                    {
                        AnsiConsole.MarkupLine("[yellow]Запуск Benchmarks... Це краще робити командою: dotnet run -c Release[/]");
                        BenchmarkRunner.Run<MultithreadingBenchmarks>();
                        Pause();
                    }
                }
                catch (OperationCanceledException)
                {
                    AnsiConsole.MarkupLine("\n[red]Операцію було скасовано (Ctrl+C).[/]");
                    break;
                }
            }
        }

        static async Task RunTask1Async(CancellationToken token)
        {
            AnsiConsole.MarkupLine("[bold cyan]--- Демонстрація Thread-Safety (Символи та числа) ---[/]");

            // 2. Візуальна цукерка: Spectre.Console Progress
            await AnsiConsole.Progress()
                .AutoClear(false)
                .Columns(new ProgressColumn[] 
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {
                    var task0 = ctx.AddTask("[cyan]T0: Вивід '='[/]", maxValue: 100);
                    var task1 = ctx.AddTask("[yellow]T1: Випадкові числа[/]", maxValue: 100);

                    // 3. Міграція на Task.Run
                    var t0 = Task.Run(async () =>
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            token.ThrowIfCancellationRequested();
                            AnsiConsole.Markup("[cyan]= [/]");
                            task0.Increment(20);
                            await Task.Delay(200, token); // Замість Thread.Sleep
                        }
                    }, token);

                    var t1 = Task.Run(async () =>
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            token.ThrowIfCancellationRequested();
                            // 4. Оптимізація пам'яті: Random.Shared
                            int num = Random.Shared.Next(0, 11);
                            AnsiConsole.Markup($"[yellow]{num} [/]");
                            task1.Increment(12.5);
                            await Task.Delay(150, token);
                        }
                    }, token);

                    await Task.WhenAll(t0, t1);
                });

            AnsiConsole.WriteLine();
            Pause();
        }

        static async Task RunTask2Async(CancellationToken token)
        {
            AnsiConsole.MarkupLine("[bold green]--- Асинхронна обробка масивів ---[/]");

            // 5. Чисті функції: локальний стан замість глобального
            int[] array = new int[15];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Random.Shared.Next(1, 20);
            }

            AnsiConsole.MarkupLine($"[grey]Згенерований масив: [[{string.Join(", ", array)}]][/]\n");

            await AnsiConsole.Progress()
                .AutoClear(false)
                .Columns(new ProgressColumn[] 
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(),
                })
                .StartAsync(async ctx =>
                {
                    int evenSteps = (int)Math.Ceiling(array.Length / 2.0); 
                    int oddSteps = array.Length / 2;

                    var taskEven = ctx.AddTask("[cyan]T0: Парні індекси[/]", maxValue: evenSteps);
                    var taskOdd = ctx.AddTask("[yellow]T1: Квадрати непарних[/]", maxValue: oddSteps);

                    var t0 = Task.Run(async () =>
                    {
                        for (int i = 0; i < array.Length; i += 2)
                        {
                            token.ThrowIfCancellationRequested();
                            AnsiConsole.MarkupLine($"[cyan]T0 [[idx {i}]]: {array[i]}[/]");
                            taskEven.Increment(1);
                            await Task.Delay(150, token);
                        }
                    }, token);

                    var t1 = Task.Run(async () =>
                    {
                        for (int i = 1; i < array.Length; i += 2)
                        {
                            token.ThrowIfCancellationRequested();
                            int square = array[i] * array[i];
                            AnsiConsole.MarkupLine($"[yellow]\tT1 [[idx {i}]]: {array[i]}^2 = {square}[/]");
                            taskOdd.Increment(1);
                            await Task.Delay(150, token);
                        }
                    }, token);

                    await Task.WhenAll(t0, t1);
                });

            AnsiConsole.WriteLine();

            // Render a beautiful summary table
            var table = new Table().Expand().Border(TableBorder.Rounded);
            table.Title("[bold green]Звіт про обробку масиву[/]");
            table.AddColumn(new TableColumn("[cyan]Індекс[/]").Centered());
            table.AddColumn(new TableColumn("[white]Оригінальне значення[/]").Centered());
            table.AddColumn(new TableColumn("[yellow]Результат обробки[/]").Centered());

            for (int i = 0; i < array.Length; i++)
            {
                if (i % 2 == 0)
                {
                    table.AddRow($"[cyan]{i}[/]", $"[white]{array[i]}[/]", $"[cyan]Без змін ({array[i]})[/]");
                }
                else
                {
                    table.AddRow($"[yellow]{i}[/]", $"[white]{array[i]}[/]", $"[yellow]Квадрат ({array[i] * array[i]})[/]");
                }
            }

            AnsiConsole.Write(table);

            AnsiConsole.WriteLine();
            Pause();
        }

        static void Pause()
        {
            AnsiConsole.MarkupLine("[grey]Натисніть будь-яку клавішу для продовження...[/]");
            Console.ReadKey(true);
        }
    }
}
