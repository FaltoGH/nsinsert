using System.Reflection;
namespace Nsinsert
{
    internal static class Program
    {
        private static void Help()
        {
            Console.WriteLine("usage: Nsinsert [-C <path>] <input path> <output path>");
        }


        private static int Main(string[] args)
        {
            try
            {
                if(args.Length < 1)
                {
                    Help();
                    return 1;
                }

                string inputPath;
                string outputPath;

                if(args.Length == 2)
                {
                    inputPath = args[0];
                    outputPath = args[1];
                }
                else if(args.Length == 4)
                {
                    if(args[0] != "-C")
                    {
                        Help();
                        return 1;
                    }

                    Directory.SetCurrentDirectory(args[1]);
                    inputPath = args[2];
                    outputPath = args[3];
                }
                else
                {
                    Help();
                    return 1;
                }

                string[] allLines = File.ReadAllLines(inputPath);
                var inserter = new Inserter(allLines);
                List<string> ret = inserter.Insert();
                File.WriteAllLines(outputPath, ret);

                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
    }
}
