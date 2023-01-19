namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var reader = new DataReader();
            reader.ImportAndPrintData("data.csv");
        }
    }
}