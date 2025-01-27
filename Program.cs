using System.Diagnostics;
using System.Management;

long[] sizeArray = [100000, 1000000, 10000000];


Console.WriteLine("Версия ОС: " + Environment.OSVersion);
Console.WriteLine("Количество процессоров: " + Environment.ProcessorCount);

// Получение информации о процессоре
var searcherCpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
foreach (ManagementObject obj in searcherCpu.Get())
{
    Console.WriteLine("Процессор: " + obj["Name"]);
}

// Получение информации о оперативной памяти
var searcherMemory = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
ulong totalMemory = 0;
foreach (ManagementObject obj in searcherMemory.Get())
{
    totalMemory += (ulong)obj["Capacity"];
}

Console.WriteLine("Объем оперативной памяти: " + totalMemory / (1024 * 1024) + " МБ");

foreach (long size in sizeArray)
{
    Console.WriteLine($"size = {size}");
    Random rand = new Random();
    long[] array = new long[size];

    for (int i = 0; i < size; i++)
    {
        array[i] = (long)rand.Next(1, Int32.MaxValue); // Большие значения
    }
    
    SumArray(array, SequentiallySum, "SequentiallySum");
    SumArray(array, ThreadedSum, "ThreadedSum");
    SumArray(array, ParallelSum, "ParallelSum");
}

void SumArray(long[] array, Func<long[], long> func, string nameSum)
{
    var stw = Stopwatch.StartNew();
    var sum = func(array);
    stw.Stop();
    Console.WriteLine($"{nameSum} sum = {sum} time = {stw.ElapsedMilliseconds}");
}

long SequentiallySum(long[] array)
{
    long sum = 0;
    foreach (var i in array)
    {
        sum += i;
    }
    return sum;
}

long ParallelSum(long[] array)
{
    return array.AsParallel().Sum();
}

long ThreadedSum(long[] array)
{
    int numberOfThreads = 10; 
    SumCalculator calculator = new SumCalculator();
    Thread[] threads = new Thread[numberOfThreads];
    int segmentSize = array.Length / numberOfThreads;

    for (int numberThread = 0; numberThread < numberOfThreads; numberThread++)
    {
        int start = numberThread * segmentSize;
        int end = (numberThread == numberOfThreads - 1) ? array.Length : (numberThread + 1) * segmentSize;
        threads[numberThread] = new Thread(() => calculator.SumArraySegment(array, start, end));
        threads[numberThread].Start();
    }

    foreach (var thread in threads)
    {
        thread.Join();
    }

    return calculator.Sum;
}

class SumCalculator
{
    public long Sum; 

    public void SumArraySegment(long[] array, int start, int end)
    {
        long localSum = 0;
        for (int i = start; i < end; i++)
        {
            localSum += array[i];
        }

        Interlocked.Add(ref Sum, localSum); 
    }
}