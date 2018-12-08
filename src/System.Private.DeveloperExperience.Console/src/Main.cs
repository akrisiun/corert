// using System;

using System;
using System.Runtime;
using Internal.DeveloperExperience;
using Internal.Runtime.CompilerHelpers;

class Program {    
    
    static int Main(string[] args)
    {
        var cell = DispatchCellInfo.Create();

        var load = DeveloperExperience.Default;
        string str = "Hello";
        load.WriteLine(str);
        LibraryInitializer.InitializeLibrary();

        var Console = new DeveloperExperienceConsole();
        Console.WriteLine("Hello Private.DeveloperExperience.Console ");
        
        return 0;
    }
}
