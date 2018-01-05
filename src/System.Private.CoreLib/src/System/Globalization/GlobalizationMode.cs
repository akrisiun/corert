// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Globalization
{
    internal static partial class GlobalizationMode
    {
        private const string c_InvariantModeConfigSwitch = "System.Globalization.Invariant";
        internal static bool Invariant { get; } = GetGlobalizationInvariantMode();
    }
}
