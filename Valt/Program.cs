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
                        var c = new ValtCompiler();
                        Console.WriteLine("Compiling: " + dirFile);
                        c.CompileFile(dirFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

            });
        }

        private static void CompileFile()
        {           
            var c = new ValtCompiler();
            c.CompileFile("v-master/vlib/v/gen/tests/4.vv");
        }
    }
}