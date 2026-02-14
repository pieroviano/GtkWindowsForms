using System.Text;
using Prepare.Common.Languages;

namespace ChineseExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                PrintUsage();
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string inputPath = args[0];

            bool saveAsText = args.Any(a =>
                a.Equals("/SaveAsText", StringComparison.OrdinalIgnoreCase));

            bool isJsonFile = File.Exists(inputPath) &&
                              Path.GetExtension(inputPath)
                                  .Equals(".json", StringComparison.OrdinalIgnoreCase);

            bool isDirectory = Directory.Exists(inputPath);

            if (!isJsonFile && !isDirectory)
            {
                Console.WriteLine("Argument must be a directory or a .json file.");
                return;
            }

            ChineseDetector.DetectChinese(
                isDirectory,
                inputPath,
                isJsonFile,
                saveAsText
            );
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  ChineseExtractor <directory> [/SaveAsText]");
            Console.WriteLine("  ChineseExtractor <file.json> [/SaveAsText]");
        }
    }
}