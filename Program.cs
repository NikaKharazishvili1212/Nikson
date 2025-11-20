class Program
{
    [STAThread]
    static void Main()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        TextCollector.CollectInFolder($@"{desktop}\aqa-fullstack", ".cs, .json");
        // TextReplacer.InFile($@"{desktop}\ss.txt", "datu2", "ss");
        // TextReplacer.InFolder($@"{desktop}\aqa-fullstack", ".cs", "[Parallelizable(ParallelScope.All)]", "[Parallelizable(ParallelScope.Fixtures)]");
    }
}