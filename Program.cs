using System.Diagnostics;

int[] size = [100000, 1000000, 1000000];

foreach (int i in size)
{
    Console.WriteLine($"size = {i}");
    var array = Enumerable.Range(0, i).ToArray();
    
    var stw = Stopwatch.StartNew();
    SequentiallySum(array);
    stw.Stop();
    Console.WriteLine($"SequentiallySum time = {stw.ElapsedMilliseconds}");
    
    stw.Restart();
    ParallelSum(array);
    stw.Stop();
    Console.WriteLine($"ParallelSum time = {stw.ElapsedMilliseconds}");
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
    return array.AsParallel().Sum(i => (long)i);
}