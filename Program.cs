class Program
{
    [STAThread]
    static void Main()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        TextCollector.CollectInFolder($@"{desktop}\aqa-fullstack\ApiTests", ".cs, .java, .json");
        TextReplacer.InFile($@"{desktop}\ss.txt", "datu2", "ss");
    }
}