using System.Collections.Generic;
using System.IO;

namespace Valt.Compiler
{
    public class FileResolver
    {
        public List<string> SearchPaths = new List<string>()
        {
            ".",
            "v-master/vlib"
        };

        public string ResolveFile(string fileName)
        {
            if (File.Exists(fileName))
                return fileName;
            
            throw new FileNotFoundException(fileName);
        }
    }
}