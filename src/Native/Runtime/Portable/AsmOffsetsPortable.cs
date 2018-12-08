








static class AsmOffsets
{
























internal const int RH_LARGE_OBJECT_SIZE = 0x14c08;
internal const int CLUMP_SIZE = 0x800;
internal const int LOG2_CLUMP_SIZE = 0xb;


internal const int OFFSETOF__Object__m_pEEType = 0x0;

internal const int OFFSETOF__Array__m_Length = 0x8;

internal const int OFFSETOF__String__m_Length = 0x8;
internal const int OFFSETOF__String__m_FirstChar = 0xC;
internal const int STRING_COMPONENT_SIZE = 0x2;
internal const int STRING_BASE_SIZE = 0x16;

internal const int OFFSETOF__EEType__m_usComponentSize = 0x0;
internal const int OFFSETOF__EEType__m_usFlags = 0x2;
internal const int OFFSETOF__EEType__m_uBaseSize = 0x4;

internal const int OFFSETOF__EEType__m_VTable = 0x20;




internal const int OFFSETOF__Thread__m_rgbAllocContextBuffer = 0x0;
internal const int OFFSETOF__Thread__m_ThreadStateFlags = 0x38;
internal const int OFFSETOF__Thread__m_pTransitionFrame = 0x40;
internal const int OFFSETOF__Thread__m_pHackPInvokeTunnel = 0x48;
internal const int OFFSETOF__Thread__m_ppvHijackedReturnAddressLocation = 0x68;
internal const int OFFSETOF__Thread__m_pvHijackedReturnAddress = 0x70;

internal const int OFFSETOF__Thread__m_uHijackedReturnValueFlags = 0x78;

internal const int OFFSETOF__Thread__m_pExInfoStackHead = 0x80;
internal const int OFFSETOF__Thread__m_threadAbortException = 0x88;

internal const int SIZEOF__EHEnum = 0x20;

internal const int OFFSETOF__gc_alloc_context__alloc_ptr = 0x0;
internal const int OFFSETOF__gc_alloc_context__alloc_limit = 0x8;


internal const int OFFSETOF__InterfaceDispatchCell__m_pCache = 0x8;



internal const int OFFSETOF__InterfaceDispatchCache__m_rgEntries = 0x20;
internal const int SIZEOF__InterfaceDispatchCacheEntry = 0x10;


internal const int OFFSETOF__StaticClassConstructionContext__m_initialized = 0x8;


internal const int OFFSETOF__CallDescrData__pSrc = 0x0;
internal const int OFFSETOF__CallDescrData__numStackSlots = 0x8;
internal const int OFFSETOF__CallDescrData__fpReturnSize = 0xC;
internal const int OFFSETOF__CallDescrData__pArgumentRegisters = 0x10;
internal const int OFFSETOF__CallDescrData__pFloatArgumentRegisters = 0x18;
internal const int OFFSETOF__CallDescrData__pTarget = 0x20;
internal const int OFFSETOF__CallDescrData__pReturnBuffer = 0x28;




















internal const int SIZEOF__ExInfo = 0x260;
internal const int OFFSETOF__ExInfo__m_pPrevExInfo = 0x0;
internal const int OFFSETOF__ExInfo__m_pExContext = 0x8;
internal const int OFFSETOF__ExInfo__m_exception = 0x10;
internal const int OFFSETOF__ExInfo__m_kind = 0x18;
internal const int OFFSETOF__ExInfo__m_passNumber = 0x19;
internal const int OFFSETOF__ExInfo__m_idxCurClause = 0x1c;
internal const int OFFSETOF__ExInfo__m_frameIter = 0x20;
internal const int OFFSETOF__ExInfo__m_notifyDebuggerSP = 0x250;

internal const int OFFSETOF__PInvokeTransitionFrame__m_RIP = 0x0;
internal const int OFFSETOF__PInvokeTransitionFrame__m_FramePointer = 0x8;
internal const int OFFSETOF__PInvokeTransitionFrame__m_pThread = 0x10;
internal const int OFFSETOF__PInvokeTransitionFrame__m_Flags = 0x18;
internal const int OFFSETOF__PInvokeTransitionFrame__m_PreservedRegs = 0x20;

internal const int SIZEOF__StackFrameIterator = 0x230;
internal const int OFFSETOF__StackFrameIterator__m_FramePointer = 0x10;
internal const int OFFSETOF__StackFrameIterator__m_ControlPC = 0x18;
internal const int OFFSETOF__StackFrameIterator__m_RegDisplay = 0x20;

internal const int SIZEOF__PAL_LIMITED_CONTEXT = 0x100;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__IP = 0x0;

internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Rsp = 0x8;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Rbp = 0x10;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Rdi = 0x18;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Rsi = 0x20;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Rax = 0x28;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Rbx = 0x30;

internal const int OFFSETOF__PAL_LIMITED_CONTEXT__R12 = 0x38;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__R13 = 0x40;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__R14 = 0x48;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__R15 = 0x50;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm6 = 0x60;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm7 = 0x70;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm8 = 0x80;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm9 = 0x90;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm10 = 0x0a0;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm11 = 0x0b0;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm12 = 0x0c0;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm13 = 0x0d0;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm14 = 0x0e0;
internal const int OFFSETOF__PAL_LIMITED_CONTEXT__Xmm15 = 0x0f0;

internal const int SIZEOF__REGDISPLAY = 0x130;
internal const int OFFSETOF__REGDISPLAY__SP = 0x78;

internal const int OFFSETOF__REGDISPLAY__pRbx = 0x18;
internal const int OFFSETOF__REGDISPLAY__pRbp = 0x20;
internal const int OFFSETOF__REGDISPLAY__pRsi = 0x28;
internal const int OFFSETOF__REGDISPLAY__pRdi = 0x30;
internal const int OFFSETOF__REGDISPLAY__pR12 = 0x58;
internal const int OFFSETOF__REGDISPLAY__pR13 = 0x60;
internal const int OFFSETOF__REGDISPLAY__pR14 = 0x68;
internal const int OFFSETOF__REGDISPLAY__pR15 = 0x70;
internal const int OFFSETOF__REGDISPLAY__Xmm = 0x90;























































































}