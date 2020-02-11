﻿using System;
using System.IO;
using Valt.Compiler;
using Valt.Compiler.Declarations;

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
            CompileFile();
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

        private static void CompileFile()
        {
            var content = File.ReadAllText("v-master/vlib/v/gen/tests/4.vv");
            var tokens = Lexer.Tokenize(content);
            var declarations = FirstPassCompiler.getTopLevelDeclarations(tokens.items);
            var c = new ValtCompiler();
            ModuleDeclaration moduleDefs = c.SetupDefinitions(declarations);
        }
    }
}