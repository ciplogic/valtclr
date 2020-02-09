using System;
using System.IO;
using Valt.Compiler;

namespace Valt
{
    class Program
    {
        static void TimeIt(string message, Action action)
        {
            var start = Environment.TickCount;
            action();
            var end = Environment.TickCount;
            Console.WriteLine($"{message}: {(end - start)} ms");
        }
        static void Main(string[] args)
        {
            var content = File.ReadAllText("v-master/vlib/builtin/string_test.v");
            var tokens = Lexer.Tokenize(content);
            TimeIt("Built in", () =>
            {
                var dirFiles = Directory.GetFiles("v-master/vlib/builtin/", "*.v");
                foreach (var dirFile in dirFiles)
                {
                    var content = File.ReadAllText(dirFile);
                    var tokens = Lexer.Tokenize(content);
                }

            });
        }
    }
}