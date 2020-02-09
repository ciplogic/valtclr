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
            var content = File.ReadAllText("v-master/vlib/v/ast/ast.v");
            var tokens = Lexer.Tokenize(content);
            var declarations = FirstPassCompiler.getTopLevelDeclarations(tokens.items);
            TimeIt("Built in", () =>
            {
                var dirFiles = Directory.GetFiles("v-master/", "*.v", SearchOption.AllDirectories);
                foreach (var dirFile in dirFiles)
                {
                    try
                    {
                        Console.WriteLine("Source: " + dirFile);
                        var content = File.ReadAllText(dirFile);
                        var tokens = Lexer.Tokenize(content);
                        FirstPassCompiler.getTopLevelDeclarations(tokens.items);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            });
        }
    }
}