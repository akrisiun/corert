using System;
using System.Diagnostics;

#if !SUPPORT_JIT
// extern alias System_Private_CoreLib;
// using TextWriter = System_Private_CoreLib::System.IO.TextWriter;
#endif

namespace HelloWorld
{
    class Program
    {
        static int Main() // string[] args
        {
            string str = "Hello World! #3";
            Console.WriteLine(str);
            object obj = str;
            
            // Microsoft.NETCore.App\2.1.6
            Console.WriteLine("BaseDirectory");
            Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("CurrentDirectory");
            // Console.WriteLine(System.Environment.CurrentDirectory);

            Console.WriteLine("Hack:");
            var h = Hacks.Type(str);
            var str2 = h.ToString();
            Console.WriteLine("str2:");
            Console.WriteLine(str2);
            // error CS0012: The type 'Object' is defined in an assembly that is not referenced. 
            // You must add a reference to assembly 'System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'.

            Console.ReadKey();
            return 0;
        }
    }

    public class Hacks {

        public static object Type(string str)
        {
            var hack = new System.TypeHack(null); // str.GetType());
            return hack;
        }
   }

}

/*
namespace ILCompiler
{
    using System.IO;
    using TextWriter = System_Private_CoreLib::System.IO.TextWriter;
    // Poor man's logger. We can do better than this.

    public class Logger
    {
        // public static Logger Null = new Logger(TextWriter.Null, false);

        public TextWriter Writer { get; }

        public bool IsVerbose { get; }

        public Logger(TextWriter writer, bool isVerbose)
        {
            Writer = writer;
            IsVerbose = isVerbose;
        }
    }
}

*/
