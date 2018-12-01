
# https://dotnet.myget.org/feed/dotnet-buildtools/package/nuget/Microsoft.DotNet.BuildTools/3.0.0-preview1-03220-01
# https://dotnet.myget.org/F/dotnet-buildtools/api/v2/package/Microsoft.DotNet.BuildTools/3.0.0-preview1-03220-01

& "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.16.27023\bin\Hostx86\x64\cl.exe" `
    -D_TARGET_AMD64_=1 -D_AMD64_ -DBIT64=1 -DUNICODE=1 -DFEATURE_BACKGROUND_GC -DFEATURE_BASICFREEZE -DFEATURE_CONSERVATIVE_GC -DFEATURE_CUSTOM_IMPORTS -DFEATURE_DYNAMIC_CODE -DFEATURE_REDHAWK -DVERIFY_HEAP -DCORERT `
    -DFEATURE_CACHED_INTERFACE_DISPATCH -D_LIB -DEETYPE_TYPE_MANAGER -DFEATURE_EMBEDDED_CONFIG -DSTRESS_HEAP -DFEATURE_RX_THUNKS -EP `
    -I"E:/Beta/dot64/corert/src/Native/Runtime/amd64" E:/Beta/dot64/corert/src/Runtime.Base/src/AsmOffsets.cspp `
    > "E:/Beta/dot64/corert/src/Native/Runtime/Full/AsmOffsets.cs"
# &gt;     