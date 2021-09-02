using System.Configuration;
using System.IO;

namespace CSharpFitsTests.util
{
    public static class TestFileSetup
    {
        static TestFileSetup()
        {
            SourceFolder = ConfigurationManager.AppSettings.Get("SourceFolder");
            TargetFolder = ConfigurationManager.AppSettings.Get("TargetFolder");

            if (!Directory.Exists(SourceFolder))
            {
                throw new ConfigurationErrorsException("Source folder does not exist.");
            }

            if (!Directory.Exists(TargetFolder))
            {
                Directory.CreateDirectory(TargetFolder);
            }
        }

        public static string SourceFolder { get; set; }
        public static string TargetFolder { get; set; }

        public static void ClearTargetFolder()
        {
            string[] files = Directory.GetFiles(TargetFolder);

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        public static void ClearAndCopyToTarget()
        {
            ClearTargetFolder();
            CopySourceToTarget();
        }

        public static void CopySourceToTarget()
        {
            string[] files = Directory.GetFiles(SourceFolder);

            foreach (var sourceFilename in files)
            {
                string sourceFile = Path.GetFileName(sourceFilename);
                string targetFilename = GetTargetFilename(sourceFile);

                File.Copy(sourceFilename, targetFilename);
            }
        }

        public static string GetTargetFilename(string filename)
        {
            return Path.Combine(TargetFolder, filename);
        }

    }
}
