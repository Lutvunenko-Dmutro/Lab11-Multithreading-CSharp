```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8973/25H2/2025Update/HudsonValley2)
11th Gen Intel Core i5-11400H 2.70GHz (Max: 2.69GHz), 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.302
  [Host]     : .NET 8.0.22 (8.0.22, 8.0.2225.52707), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 8.0.22 (8.0.22, 8.0.2225.52707), X64 RyuJIT x86-64-v4


```
| Method                   | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|------------------------- |-----------:|----------:|----------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| Legacy_ProcessArray      | 162.603 μs | 3.1687 μs | 4.0074 μs | 1.001 |    0.03 | 0.7324 | 0.7324 | 0.7324 |     400 B |        1.00 |
| Modern_ProcessArrayAsync |   1.166 μs | 0.0152 μs | 0.0135 μs | 0.007 |    0.00 | 0.0763 |      - |      - |     479 B |        1.20 |
