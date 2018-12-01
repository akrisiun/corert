using System;
#if SUPPORT_JIT
extern alias System_Private_CoreLib;
using TextWriter = System_Private_CoreLib::System.IO.TextWriter;
#endif

namespace HelloWorld
{
    class Program
    {
        static int Main() // string[] args
        {
            //  String str = "Hello World!";
            // Object obj = str;
            // Console.WriteLine(str);
            return 0;
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
