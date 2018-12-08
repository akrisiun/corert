// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.DeveloperExperience;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Diagnostics
{
    public static partial class DebugMono
    {
        public static void Assert (bool condition) => Debug.Assert(condition);

        [DllImport("api-ms-win-core-debug-l1-1-0.dll", EntryPoint = "IsDebuggerPresent", CharSet = CharSet.Unicode)]
        internal extern static bool IsDebuggerPresent();

        [DllImport("api-ms-win-core-debug-l1-1-0.dll", EntryPoint = "OutputDebugStringW", CharSet = CharSet.Unicode)]
        internal extern static void OutputDebugString(string lpOutputString);

    }

    // .NET Native-specific Debug implementation
    public static partial class Debug
    {
#if MONO
		public static void WriteLine(string format, params object[] args)
		{
            System.Diagnostics.Private.Debug.WriteLine(String.Format(format ?? "", args) ?? "");
		}        

        [ConditionalAttribute ("DEBUG")]
		public static void Assert (bool condition)
		{
		}        

        [ConditionalAttribute ("DEBUG")]
		public static void Assert (bool condition, string message, params object[] args)
		{
		}

        [ConditionalAttribute ("DEBUG")]
		public static void Assert (bool condition, string message, string detailMessageFormat, params object[] args)
		{
		}

		[ConditionalAttribute ("DEBUG")]
		public static void Fail (string message)
		{
		}

		[ConditionalAttribute ("DEBUG")]
		public static void Fail (string message, string detailMessage)
		{
		}
#endif

        [DebuggerHidden]
        [Intrinsic]
        internal static void DebugBreak()
        {
            // IMPORTANT: This call will let us detect if debug break is broken, and also
            // gives double chances.
            DebugBreak();
        }
    }
}
