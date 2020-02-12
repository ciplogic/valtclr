using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Valt.Compiler
{
    public class FileResolver
    {
        public string VLibPath = "v-master/vlib";

        public string ResolveFile(string fileName)
        {
            if (File.Exists(fileName))
                return fileName;
            
            throw new FileNotFoundException(fileName);
        }


        public static string GetFullFileName(string value)
        {
            var fileInfo = new FileInfo(value);
            return fileInfo.FullName;
        }
        public string[] ResolveModule(string runtimeModule)
        {
            var path = Path.Join(VLibPath, runtimeModule);
            if (!Directory.Exists(path))
            {
                return new string[0];
            }

            var vFiles = Directory.GetFiles(path, "*.v")
                .Where(fName=>!fName.EndsWith("_test.v"))
                .Select(fName => GetFullFileName(fName))
                .ToArray();

            return vFiles;
        }
    }
}