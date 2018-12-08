using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Internal.Runtime;

namespace System.Runtime
{
    internal enum DispatchCellType
    {
        InterfaceAndSlot = 0x0,
        MetadataToken = 0x1,
        VTableOffset = 0x2,
    }

    internal struct DispatchCellInfo
    {
        public DispatchCellType CellType;
        public EETypePtr InterfaceType;
        public ushort InterfaceSlot;
        public byte HasCache;
        public uint MetadataToken;
        public uint VTableOffset;

        public static DispatchCellInfo Create()
        {
            var stru = new DispatchCellInfo();
            stru.CellType = DispatchCellType.InterfaceAndSlot;
            stru.InterfaceType = default(EETypePtr);
            stru.InterfaceSlot = 0;
            stru.HasCache = 0;
            stru.MetadataToken = 0;
            stru.VTableOffset = 0;

            return stru;
        }
    }

    // Constants used with RhpGetClasslibFunction, to indicate which classlib function
    // we are interested in. 
    // Note: make sure you change the def in ICodeManager.h if you change this!
    internal enum ClassLibFunctionId
    {
        GetRuntimeException = 0,
        FailFast = 1,
        // UnhandledExceptionHandler = 2, // unused
        AppendExceptionStackFrame = 3,
        CheckStaticClassConstruction = 4,
        GetSystemArrayEEType = 5,
        OnFirstChance = 6,
        DebugFuncEvalHelper = 7,
        DebugFuncEvalAbortHelper = 8,
    }

    internal class Redhawk { public const string BaseName = "*"; }

    internal enum InternalGCCollectionMode
    {
        NonBlocking = 0x00000001,
        Blocking = 0x00000002,
        Optimized = 0x00000004,
        Compacting = 0x00000008,
    }


    internal static class InternalCalls
    {
        //
        // internalcalls for System.GC.
        //

        // Force a garbage collection.
        [RuntimeExport("RhCollect")]
        internal static void RhCollect(int generation, InternalGCCollectionMode mode)
        {
            RhpCollect(generation, mode);
        }

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RhpCollect(int generation, InternalGCCollectionMode mode);

        [RuntimeExport("RhGetGcTotalMemory")]
        internal static long RhGetGcTotalMemory()
        {
            return RhpGetGcTotalMemory();
        }

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        private static extern long RhpGetGcTotalMemory();

        [RuntimeExport("RhStartNoGCRegion")]
        internal static int RhStartNoGCRegion(long totalSize, bool hasLohSize, long lohSize, bool disallowFullBlockingGC)
        {
            return RhpStartNoGCRegion(totalSize, hasLohSize, lohSize, disallowFullBlockingGC);
        }

        [RuntimeExport("RhEndNoGCRegion")]
        internal static int RhEndNoGCRegion()
        {
            return RhpEndNoGCRegion();
        }

        //
        // internalcalls for System.Runtime.__Finalizer.
        //

        // Fetch next object which needs finalization or return null if we've reached the end of the list.
        [RuntimeImport(Redhawk.BaseName, "RhpGetNextFinalizableObject")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal static extern object RhpGetNextFinalizableObject();

        //
        // internalcalls for System.Runtime.InteropServices.GCHandle.
        //

        // Allocate handle.
        [RuntimeImport(Redhawk.BaseName, "RhpHandleAlloc")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal static extern IntPtr RhpHandleAlloc(object value, GCHandleType type);

        // Allocate dependent handle.
        [RuntimeImport(Redhawk.BaseName, "RhpHandleAllocDependent")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal static extern IntPtr RhpHandleAllocDependent(object primary, object secondary);

        // Allocate variable handle.
        [RuntimeImport(Redhawk.BaseName, "RhpHandleAllocVariable")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal static extern IntPtr RhpHandleAllocVariable(object value, uint type);

        [RuntimeImport(Redhawk.BaseName, "RhHandleGet")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal static extern object RhHandleGet(IntPtr handle);

        [RuntimeImport(Redhawk.BaseName, "RhHandleSet")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal static extern IntPtr RhHandleSet(IntPtr handle, object value);

        //
        // internal calls for allocation
        //
        [RuntimeImport(Redhawk.BaseName, "RhpNewFast")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe object RhpNewFast(EEType* pEEType);  // BEWARE: not for finalizable objects!

        [RuntimeImport(Redhawk.BaseName, "RhpNewFinalizable")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe object RhpNewFinalizable(EEType* pEEType);

        [RuntimeImport(Redhawk.BaseName, "RhpNewArray")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe object RhpNewArray(EEType* pEEType, int length);

#if FEATURE_64BIT_ALIGNMENT
        [RuntimeImport(Redhawk.BaseName, "RhpNewFastAlign8")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe object RhpNewFastAlign8(EEType * pEEType);  // BEWARE: not for finalizable objects!

        [RuntimeImport(Redhawk.BaseName, "RhpNewFinalizableAlign8")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe object RhpNewFinalizableAlign8(EEType* pEEType);

        [RuntimeImport(Redhawk.BaseName, "RhpNewArrayAlign8")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe object RhpNewArrayAlign8(EEType* pEEType, int length);

        [RuntimeImport(Redhawk.BaseName, "RhpNewFastMisalign")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe object RhpNewFastMisalign(EEType * pEEType);
#endif // FEATURE_64BIT_ALIGNMENT

        [RuntimeImport(Redhawk.BaseName, "RhpBox")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe void RhpBox(object obj, ref byte data);

        [RuntimeImport(Redhawk.BaseName, "RhUnbox")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Sometimes)]
        internal extern static unsafe void RhUnbox(object obj, ref byte data, EEType* pUnboxToEEType);

        [RuntimeImport(Redhawk.BaseName, "RhpCopyObjectContents")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void RhpCopyObjectContents(object objDest, object objSrc);

        [RuntimeImport(Redhawk.BaseName, "RhpCompareObjectContents")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static bool RhpCompareObjectContentsAndPadding(object obj1, object obj2);

        [RuntimeImport(Redhawk.BaseName, "RhpAssignRef")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void RhpAssignRef(ref object address, object obj);

#if FEATURE_GC_STRESS
        //
        // internal calls for GC stress
        //
        [RuntimeImport(Redhawk.BaseName, "RhpInitializeGcStress")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void RhpInitializeGcStress();
#endif // FEATURE_GC_STRESS

        //[RuntimeImport(Redhawk.BaseName, "RhpEHEnumInitFromStackFrameIterator")]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[ManuallyManaged(GcPollPolicy.Never)]
        //internal extern static unsafe bool RhpEHEnumInitFromStackFrameIterator(ref StackFrameIterator pFrameIter, byte** pMethodStartAddress, void* pEHEnum);

        [RuntimeImport(Redhawk.BaseName, "RhpEHEnumNext")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe bool RhpEHEnumNext(void* pEHEnum, void* pEHClause);

        [RuntimeImport(Redhawk.BaseName, "RhpGetArrayBaseType")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe EEType* RhpGetArrayBaseType(EEType* pEEType);

        [RuntimeImport(Redhawk.BaseName, "RhpHasDispatchMap")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe bool RhpHasDispatchMap(EEType* pEETypen);

        [RuntimeImport(Redhawk.BaseName, "RhpGetDispatchMap")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe DispatchResolve.DispatchMap* RhpGetDispatchMap(EEType* pEEType);

        [RuntimeImport(Redhawk.BaseName, "RhpGetSealedVirtualSlot")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe IntPtr RhpGetSealedVirtualSlot(EEType* pEEType, ushort slot);

        [RuntimeImport(Redhawk.BaseName, "RhpGetDispatchCellInfo")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void RhpGetDispatchCellInfo(IntPtr pCell, out DispatchCellInfo newCellInfo);

        [RuntimeImport(Redhawk.BaseName, "RhpSearchDispatchCellCache")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe IntPtr RhpSearchDispatchCellCache(IntPtr pCell, EEType* pInstanceType);

        [RuntimeImport(Redhawk.BaseName, "RhpUpdateDispatchCellCache")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe IntPtr RhpUpdateDispatchCellCache(IntPtr pCell, IntPtr pTargetCode, EEType* pInstanceType, ref DispatchCellInfo newCellInfo);

        [RuntimeImport(Redhawk.BaseName, "RhpGetClasslibFunctionFromCodeAddress")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void* RhpGetClasslibFunctionFromCodeAddress(IntPtr address, ClassLibFunctionId id);

        [RuntimeImport(Redhawk.BaseName, "RhpGetClasslibFunctionFromEEType")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void* RhpGetClasslibFunctionFromEEType(IntPtr pEEType, ClassLibFunctionId id);

        //
        // StackFrameIterator
        //

        //[RuntimeImport(Redhawk.BaseName, "RhpSfiInit")]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[ManuallyManaged(GcPollPolicy.Never)]
        //internal static extern unsafe bool RhpSfiInit(ref StackFrameIterator pThis, void* pStackwalkCtx, bool instructionFault);

        //[RuntimeImport(Redhawk.BaseName, "RhpSfiNext")]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[ManuallyManaged(GcPollPolicy.Never)]
        //internal static extern bool RhpSfiNext(ref StackFrameIterator pThis, out uint uExCollideClauseIdx, out bool fUnwoundReversePInvoke);

        //
        // DebugEventSource
        //

        //[RuntimeImport(Redhawk.BaseName, "RhpGetRequestedExceptionEvents")]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[ManuallyManaged(GcPollPolicy.Never)]
        //internal static extern ExceptionEventKind RhpGetRequestedExceptionEvents();

        //[DllImport(Redhawk.BaseName)]
        //internal static extern unsafe void RhpSendExceptionEventToDebugger(ExceptionEventKind eventKind, byte* ip, UIntPtr sp);

        //
        // Miscellaneous helpers.
        //

        // Get the rarely used (optional) flags of an EEType. If they're not present 0 will be returned.
        [RuntimeImport(Redhawk.BaseName, "RhpGetEETypeRareFlags")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe uint RhpGetEETypeRareFlags(EEType* pEEType);

        // Retrieve the offset of the value embedded in a Nullable<T>.
        [RuntimeImport(Redhawk.BaseName, "RhpGetNullableEETypeValueOffset")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe byte RhpGetNullableEETypeValueOffset(EEType* pEEType);

        // Retrieve the target type T in a Nullable<T>.
        [RuntimeImport(Redhawk.BaseName, "RhpGetNullableEEType")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe EEType* RhpGetNullableEEType(EEType* pEEType);

        // For an ICastable type return a pointer to code that implements ICastable.IsInstanceOfInterface.
        [RuntimeImport(Redhawk.BaseName, "RhpGetICastableIsInstanceOfInterfaceMethod")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe IntPtr RhpGetICastableIsInstanceOfInterfaceMethod(EEType* pEEType);

        // For an ICastable type return a pointer to code that implements ICastable.GetImplType.
        [RuntimeImport(Redhawk.BaseName, "RhpGetICastableGetImplTypeMethod")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe IntPtr RhpGetICastableGetImplTypeMethod(EEType* pEEType);

        [RuntimeImport(Redhawk.BaseName, "RhpGetNextFinalizerInitCallback")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe IntPtr RhpGetNextFinalizerInitCallback();

        //[RuntimeImport(Redhawk.BaseName, "RhpCallCatchFunclet")]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[ManuallyManaged(GcPollPolicy.Never)]
        //internal extern static unsafe IntPtr RhpCallCatchFunclet(
        //    object exceptionObj, byte* pHandlerIP, void* pvRegDisplay, ref EH.ExInfo exInfo);

        [RuntimeImport(Redhawk.BaseName, "RhpCallFinallyFunclet")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void RhpCallFinallyFunclet(byte* pHandlerIP, void* pvRegDisplay);

        [RuntimeImport(Redhawk.BaseName, "RhpCallFilterFunclet")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe bool RhpCallFilterFunclet(
            object exceptionObj, byte* pFilterIP, void* pvRegDisplay);

        [RuntimeImport(Redhawk.BaseName, "RhpFallbackFailFast")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void RhpFallbackFailFast();

        [RuntimeImport(Redhawk.BaseName, "RhpSetThreadDoNotTriggerGC")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static void RhpSetThreadDoNotTriggerGC();

        [System.Diagnostics.Conditional("DEBUG")]
        [RuntimeImport(Redhawk.BaseName, "RhpValidateExInfoStack")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static void RhpValidateExInfoStack();

        //[RuntimeImport(Redhawk.BaseName, "RhpCopyContextFromExInfo")]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[ManuallyManaged(GcPollPolicy.Never)]
        //internal extern static unsafe void RhpCopyContextFromExInfo(void* pOSContext, int cbOSContext, EH.PAL_LIMITED_CONTEXT* pPalContext);

        [RuntimeImport(Redhawk.BaseName, "RhpGetCastableObjectDispatchHelper")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static IntPtr RhpGetCastableObjectDispatchHelper();

        [RuntimeImport(Redhawk.BaseName, "RhpGetCastableObjectDispatchHelper_TailCalled")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static IntPtr RhpGetCastableObjectDispatchHelper_TailCalled();

        [RuntimeImport(Redhawk.BaseName, "RhpGetCastableObjectDispatch_CommonStub")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static IntPtr RhpGetCastableObjectDispatch_CommonStub();

        [RuntimeImport(Redhawk.BaseName, "RhpGetTailCallTLSDispatchCell")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static IntPtr RhpGetTailCallTLSDispatchCell();

        [RuntimeImport(Redhawk.BaseName, "RhpSetTLSDispatchCell")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static unsafe void RhpSetTLSDispatchCell(IntPtr pCell);

        [RuntimeImport(Redhawk.BaseName, "RhpGetNumThunkBlocksPerMapping")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static int RhpGetNumThunkBlocksPerMapping();

        [RuntimeImport(Redhawk.BaseName, "RhpGetNumThunksPerBlock")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static int RhpGetNumThunksPerBlock();

        [RuntimeImport(Redhawk.BaseName, "RhpGetThunkSize")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static int RhpGetThunkSize();

        [RuntimeImport(Redhawk.BaseName, "RhpGetThunkDataBlockAddress")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static IntPtr RhpGetThunkDataBlockAddress(IntPtr thunkStubAddress);

        [RuntimeImport(Redhawk.BaseName, "RhpGetThunkStubsBlockAddress")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static IntPtr RhpGetThunkStubsBlockAddress(IntPtr thunkDataAddress);

        [RuntimeImport(Redhawk.BaseName, "RhpGetThunkBlockSize")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [ManuallyManaged(GcPollPolicy.Never)]
        internal extern static int RhpGetThunkBlockSize();

        [RuntimeImport(Redhawk.BaseName, "RhpGetThreadAbortException")]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static Exception RhpGetThreadAbortException();

        //------------------------------------------------------------------------------------------------------------
        // PInvoke-based internal calls
        //
        // These either do not need to be called in cooperative mode or, in some cases, MUST be called in preemptive
        // mode.  Note that they must use the Cdecl calling convention due to a limitation in our .obj file linking
        // support.
        //------------------------------------------------------------------------------------------------------------

        // Block the current thread until at least one object needs to be finalized (returns true) or
        // memory is low (returns false and the finalizer thread should initiate a garbage collection).
        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint RhpWaitForFinalizerRequest();

        // Indicate that the current round of finalizations is complete.
        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RhpSignalFinalizationComplete();

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RhpAcquireCastCacheLock();

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RhpReleaseCastCacheLock();

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal extern static long PalGetTickCount64();

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RhpAcquireThunkPoolLock();

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RhpReleaseThunkPoolLock();

        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr RhAllocateThunksMapping();

        // Enters a no GC region, possibly doing a blocking GC if there is not enough
        // memory available to satisfy the caller's request.
        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RhpStartNoGCRegion(long totalSize, bool hasLohSize, long lohSize, bool disallowFullBlockingGC);

        // Exits a no GC region, possibly doing a GC to clean up the garbage that
        // the caller allocated.
        [DllImport(Redhawk.BaseName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RhpEndNoGCRegion();
    }

    internal static unsafe class DispatchResolve
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct DispatchMapEntry
        {
            public ushort _usInterfaceIndex;
            public ushort _usInterfaceMethodSlot;
            public ushort _usImplMethodSlot;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DispatchMap
        {
            public uint _entryCount;
            public DispatchMapEntry _dispatchMap; // Actually a variable length array
        }

        //public static IntPtr FindInterfaceMethodImplementationTarget(EEType* pTgtType,
        //                                                         EEType* pItfType,
        //                                                         ushort itfSlotNumber)
        //{
        //    DynamicModule* dynamicModule = pTgtType->DynamicModule;

        //    // Use the dynamic module resolver if it's present
        //    if ((dynamicModule != null) && (dynamicModule->DynamicTypeSlotDispatchResolve != IntPtr.Zero))
        //    {
        //        return CalliIntrinsics.Call<IntPtr>(dynamicModule->DynamicTypeSlotDispatchResolve,
        //                                            (IntPtr)pTgtType, (IntPtr)pItfType, itfSlotNumber);
        //    }

        //    // Start at the current type and work up the inheritance chain
        //    EEType* pCur = pTgtType;

        //    if (pItfType->IsCloned)
        //        pItfType = pItfType->CanonicalEEType;

        //    while (pCur != null)
        //    {
        //        ushort implSlotNumber;
        //        if (FindImplSlotForCurrentType(
        //                pCur, pItfType, itfSlotNumber, &implSlotNumber))
        //        {
        //            IntPtr targetMethod;
        //            if (implSlotNumber < pCur->NumVtableSlots)
        //            {
        //                // true virtual - need to get the slot from the target type in case it got overridden
        //                targetMethod = pTgtType->GetVTableStartAddress()[implSlotNumber];
        //            }
        //            else
        //            {
        //                // sealed virtual - need to get the slot form the implementing type, because
        //                // it's not present on the target type
        //                targetMethod = pCur->GetSealedVirtualSlot((ushort)(implSlotNumber - pCur->NumVtableSlots));
        //            }
        //            return targetMethod;
        //        }
        //        if (pCur->IsArray)
        //            pCur = pCur->GetArrayEEType();
        //        else
        //            pCur = pCur->NonArrayBaseType;
        //    }
        //    return IntPtr.Zero;
        //}


        //private static bool FindImplSlotForCurrentType(EEType* pTgtType,
        //                                EEType* pItfType,
        //                                ushort itfSlotNumber,
        //                                ushort* pImplSlotNumber)
        //{
        //    bool fRes = false;

        //    // If making a call and doing virtual resolution don't look into the dispatch map,
        //    // take the slot number directly.
        //    if (!pItfType->IsInterface)
        //    {
        //        *pImplSlotNumber = itfSlotNumber;

        //        // Only notice matches if the target type and search types are the same
        //        // This will make dispatch to sealed slots work correctly
        //        return pTgtType == pItfType;
        //    }

        //    if (pTgtType->HasDispatchMap)
        //    {
        //        // For variant interface dispatch, the algorithm is to walk the parent hierarchy, and at each level
        //        // attempt to dispatch exactly first, and then if that fails attempt to dispatch variantly. This can
        //        // result in interesting behavior such as a derived type only overriding one particular instantiation
        //        // and funneling all the dispatches to it, but its the algorithm.

        //        bool fDoVariantLookup = false; // do not check variance for first scan of dispatch map 

        //        fRes = FindImplSlotInSimpleMap(
        //            pTgtType, pItfType, itfSlotNumber, pImplSlotNumber, fDoVariantLookup);

        //        if (!fRes)
        //        {
        //            fDoVariantLookup = true; // check variance for second scan of dispatch map
        //            fRes = FindImplSlotInSimpleMap(
        //             pTgtType, pItfType, itfSlotNumber, pImplSlotNumber, fDoVariantLookup);
        //        }
        //    }

        //    return fRes;
        //}

        //private static bool FindImplSlotInSimpleMap(EEType* pTgtType,
        //                             EEType* pItfType,
        //                             uint itfSlotNumber,
        //                             ushort* pImplSlotNumber,
        //                             bool actuallyCheckVariance)
        //{
        //    Debug.Assert(pTgtType->HasDispatchMap, "Missing dispatch map");

        //    EEType* pItfOpenGenericType = null;
        //    EETypeRef* pItfInstantiation = null;
        //    int itfArity = 0;
        //    GenericVariance* pItfVarianceInfo = null;

        //    bool fCheckVariance = false;
        //    bool fArrayCovariance = false;

        //    if (actuallyCheckVariance)
        //    {
        //        fCheckVariance = pItfType->HasGenericVariance;
        //        fArrayCovariance = pTgtType->IsArray;

        //        // Non-arrays can follow array variance rules iff
        //        // 1. They have one generic parameter
        //        // 2. That generic parameter is array covariant.
        //        //
        //        // This special case is to allow array enumerators to work
        //        if (!fArrayCovariance && pTgtType->HasGenericVariance)
        //        {
        //            int tgtEntryArity = (int)pTgtType->GenericArity;
        //            GenericVariance* pTgtVarianceInfo = pTgtType->GenericVariance;

        //            if ((tgtEntryArity == 1) && pTgtVarianceInfo[0] == GenericVariance.ArrayCovariant)
        //            {
        //                fArrayCovariance = true;
        //            }
        //        }

        //        // Arrays are covariant even though you can both get and set elements (type safety is maintained by
        //        // runtime type checks during set operations). This extends to generic interfaces implemented on those
        //        // arrays. We handle this by forcing all generic interfaces on arrays to behave as though they were
        //        // covariant (over their one type parameter corresponding to the array element type).
        //        if (fArrayCovariance && pItfType->IsGeneric)
        //            fCheckVariance = true;

        //        // TypeEquivalent interface dispatch is handled at covariance time. At this time we don't have general
        //        // type equivalent interface dispatch, but we do use the model for the interface underlying CastableObject
        //        // which is done by checking the interface types involved for ICastable.
        //        if (pItfType->IsICastable)
        //            fCheckVariance = true;

        //        // If there is no variance checking, there is no operation to perform. (The non-variance check loop
        //        // has already completed)
        //        if (!fCheckVariance)
        //        {
        //            return false;
        //        }
        //    }

        //    DispatchMap* pMap = pTgtType->DispatchMap;
        //    DispatchMapEntry* i = &pMap->_dispatchMap;
        //    DispatchMapEntry* iEnd = (&pMap->_dispatchMap) + pMap->_entryCount;
        //    for (; i != iEnd; ++i)
        //    {
        //        if (i->_usInterfaceMethodSlot == itfSlotNumber)
        //        {
        //            EEType* pCurEntryType =
        //                pTgtType->InterfaceMap[i->_usInterfaceIndex].InterfaceType;

        //            if (pCurEntryType->IsCloned)
        //                pCurEntryType = pCurEntryType->CanonicalEEType;

        //            if (pCurEntryType == pItfType)
        //            {
        //                *pImplSlotNumber = i->_usImplMethodSlot;
        //                return true;
        //            }
        //            else if (fCheckVariance && pCurEntryType->IsICastable && pItfType->IsICastable)
        //            {
        //                *pImplSlotNumber = i->_usImplMethodSlot;
        //                return true;
        //            }
        //            else if (fCheckVariance && ((fArrayCovariance && pCurEntryType->IsGeneric) || pCurEntryType->HasGenericVariance))
        //            {
        //                // Interface types don't match exactly but both the target interface and the current interface
        //                // in the map are marked as being generic with at least one co- or contra- variant type
        //                // parameter. So we might still have a compatible match.

        //                // Retrieve the unified generic instance for the callsite interface if we haven't already (we
        //                // lazily get this then cache the result since the lookup isn't necessarily cheap).
        //                if (pItfOpenGenericType == null)
        //                {
        //                    pItfOpenGenericType = pItfType->GenericDefinition;
        //                    itfArity = (int)pItfType->GenericArity;
        //                    pItfInstantiation = pItfType->GenericArguments;
        //                    pItfVarianceInfo = pItfType->GenericVariance;
        //                }

        //                // Retrieve the unified generic instance for the interface we're looking at in the map.
        //                EEType* pCurEntryGenericType = pCurEntryType->GenericDefinition;

        //                // If the generic types aren't the same then the types aren't compatible.
        //                if (pItfOpenGenericType != pCurEntryGenericType)
        //                    continue;

        //                // Grab instantiation details for the candidate interface.
        //                EETypeRef* pCurEntryInstantiation = pCurEntryType->GenericArguments;

        //                // The types represent different instantiations of the same generic type. The
        //                // arity of both had better be the same.
        //                // Debug.Assert(itfArity == (int)pCurEntryType->GenericArity, "arity mismatch betweeen generic instantiations");

        //                if (TypeCast.TypeParametersAreCompatible(itfArity, pCurEntryInstantiation, pItfInstantiation, pItfVarianceInfo, fArrayCovariance, null))
        //                {
        //                    *pImplSlotNumber = i->_usImplMethodSlot;
        //                    return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}
    }

}


namespace System.Runtime.CompilerServices
{
    // Indicates whether or not a given ManuallyManaged method will rendez-vous 
    // with GC if a suspension is being requested.  Most ManuallyManaged methods
    // will be PollPolicy.Never.  The only distinction made by the code generator
    // is between Always and everything else.  The distinction between Sometimes
    // and Never is merely for documentation purposes.
    internal enum GcPollPolicy
    {
        Always = 1,
        Sometimes = 2,
        Never = 3,
    }

    internal class ManuallyManagedAttribute : Attribute
    {
        public ManuallyManagedAttribute(GcPollPolicy poll)
        {
        }
    }
}
