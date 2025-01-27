using System.Diagnostics;
int[] sizeArray = [100000, 1000000, 1000000];

foreach (int size in sizeArray)
{
    Console.WriteLine($"size = {size}");
    var array = Enumerable.Range(0, size).ToArray();
    
    SumArray(array, SequentiallySum, "SequentiallySum");
    SumArray(array, ParallelSum, "ParallelSum");
    SumArray(array, ThreadedSum, "ThreadedSum");
 
}

void SumArray(int[] array, Func<int[], long> func, string nameSum)
{
    var stw = Stopwatch.StartNew();
    var sum = func(array);
    stw.Stop();
    Console.WriteLine($"{nameSum} sum = {sum} time = {stw.ElapsedMilliseconds}");
}

long SequentiallySum(int[] array)
{
    int sum = 0;
    foreach (var i in array)
    {
        sum += i;
    }
    return sum;
}

long ParallelSum(int[] array)
{
    return array.AsParallel().Sum();
}

long ThreadedSum(int[] array)
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
    public int Sum; 

    public void SumArraySegment(int[] array, int start, int end)
    {
        int localSum = 0;
        for (int i = start; i < end; i++)
        {
            localSum += array[i];
        }

        Interlocked.Add(ref Sum, localSum); 
    }
}