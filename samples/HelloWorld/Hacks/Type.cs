// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// System.Type is only defined to support C# typeof. We shouldn't have it here since the semantic
// is not very compatible.

using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime;
using Internal.Runtime.CompilerServices;

namespace System
{
    public class TypeHack // : Type
    {
        private readonly RuntimeTypeHandle _typeHandle;

        private TypeHack(RuntimeTypeHandle typeHandle)
        {
            _typeHandle = typeHandle;
        }

        public RuntimeTypeHandle TypeHandle2 => _typeHandle;

        public static TypeHack GetTypeFromHandle2(RuntimeTypeHandle rh)
        {
            return new TypeHack(rh);
        }
    }

    internal sealed class IntrinsicAttribute : Attribute
    {
    }

    internal static class IndirectionConstants
    {
        /// <summary>
        /// Flag set on pointers to indirection cells to distinguish them
        /// from pointers to the object directly
        /// </summary>
        public const int IndirectionCellPointer = 0x1;

        /// <summary>
        /// Flag set on RVAs to indirection cells to distinguish them
        /// from RVAs to the object directly
        /// </summary>
        public const uint RVAPointsToIndirection = 0x80000000u;
    }

    
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class RuntimeExportAttribute : Attribute
    {
        public string EntryPoint;

        public RuntimeExportAttribute(string entry)
        {
            EntryPoint = entry;
        }
    }

    [System.AttributeUsageAttribute(System.AttributeTargets.Class | System.AttributeTargets.Method)]
    internal class McgIntrinsicsAttribute : System.Attribute
    {
        public McgIntrinsicsAttribute() { }
    }

    internal static class TypeCast
    {
        [Flags]
        internal enum AssignmentVariation
        {
            Normal = 0,

            /// <summary>
            /// Assume the source type is boxed so that value types and enums are compatible with Object, ValueType 
            /// and Enum (if applicable)
            /// </summary>
            BoxedSource = 1,

            /// <summary>
            /// Allow identically sized integral types and enums to be considered equivalent (currently used only for 
            /// array element types)
            /// </summary>
            AllowSizeEquivalence = 2,
        }

        [RuntimeExport("RhTypeCast_IsInstanceOfClass2")]
        public static unsafe object IsInstanceOfClass2(void* pvTargetType, object obj)
        {
            return IsInstanceOfClass(obj, pvTargetType);
        }

        [RuntimeExport("RhTypeCast_IsInstanceOfClass")]
        public static unsafe object IsInstanceOfClass(object obj, void* pvTargetType)
        {
            if (obj == null)
            {
                return null;
            }

            EEType* pTargetType = (EEType*)pvTargetType;
            //EEType* pObjType = obj.EEType;

            // Debug.Assert(!pTargetType->IsParameterizedType, "IsInstanceOfClass called with parameterized EEType");
            // Debug.Assert(!pTargetType->IsInterface, "IsInstanceOfClass called with interface EEType");

            // if the EETypes pointers match, we're done
            return obj;
            /*
            if (pObjType == pTargetType)
            {
                return obj;
            }

            // Quick check if both types are good for simple casting: canonical, no related type via IAT, no generic variance
            if (Internal.Runtime.EEType.BothSimpleCasting(pObjType, pTargetType))
            {
                // walk the type hierarchy looking for a match
                do
                {
                    pObjType = pObjType->RawBaseType;

                    if (pObjType == null)
                    {
                        return null;
                    }

                    if (pObjType == pTargetType)
                    {
                        return obj;
                    }
                }
                while (pObjType->SimpleCasting());
            }

            if (pTargetType->IsCloned)
            {
                pTargetType = pTargetType->CanonicalEEType;
            }

            if (pObjType->IsCloned)
            {
                pObjType = pObjType->CanonicalEEType;
            }

            // if the EETypes pointers match, we're done
            if (pObjType == pTargetType)
            {
                return obj;
            }

            if (pTargetType->HasGenericVariance && pObjType->HasGenericVariance)
            {
                // Only generic interfaces and delegates can have generic variance and we shouldn't see
                // interfaces for either input here. So if the canonical types are marked as having variance
                // we know we've hit the delegate case. We've dealt with the identical case just above. And
                // the regular path below will handle casting to Object, Delegate and MulticastDelegate. Since
                // we don't support deriving from user delegate classes any further all we have to check here
                // is that the uninstantiated generic delegate definitions are the same and the type
                // parameters are compatible.

                // NOTE: using general assignable path for the cache because of the cost of the variance checks
                if (CastCache.AreTypesAssignableInternal(pObjType, pTargetType, AssignmentVariation.BoxedSource, null))
                    return obj;
                return null;
            }

            if (pObjType->IsArray)
            {
                // arrays can be cast to System.Object
                if (WellKnownEETypes.IsSystemObject(pTargetType))
                {
                    return obj;
                }

                // arrays can be cast to System.Array
                if (WellKnownEETypes.IsSystemArray(pTargetType))
                {
                    return obj;
                }

                return null;
            }


            // walk the type hierarchy looking for a match
            while (true)
            {
                pObjType = pObjType->NonClonedNonArrayBaseType;
                if (pObjType == null)
                {
                    return null;
                }

                if (pObjType->IsCloned)
                    pObjType = pObjType->CanonicalEEType;

                if (pObjType == pTargetType)
                {
                    return obj;
                }
            }
            */
        }

        [RuntimeExport("RhTypeCast_CheckCastClass2")]
        public static unsafe object CheckCastClass2(void* pvTargetEEType, object obj)
        {
            return CheckCastClass(obj, pvTargetEEType);
        }

        [RuntimeExport("RhTypeCast_CheckCastClass")]
        public static unsafe object CheckCastClass(object obj, void* pvTargetEEType)
        {
            // a null value can be cast to anything
            if (obj == null)
                return null;

            object result = IsInstanceOfClass(obj, pvTargetEEType);

            if (result == null)
            {
                // Throw the invalid cast exception defined by the classlib, using the input EEType* 
                // to find the correct classlib.

                throw ((EEType*)pvTargetEEType)->GetClasslibException(ExceptionIDs.InvalidCast);
            }

            return result;
        }

        [RuntimeExport("RhTypeCast_CheckUnbox")]
        public static unsafe void CheckUnbox(object obj, byte expectedCorElementType)
        {
            if (obj == null)
            {
                return;
            }

            //if (obj.EEType->CorElementType == (CorElementType)expectedCorElementType)
            //    return;

            // Throw the invalid cast exception defined by the classlib, using the input object's EEType* 
            // to find the correct classlib.

            Debugger.Break();
            // 
            throw new NotImplementedException(); // obj.EEType->GetClasslibException(ExceptionIDs.InvalidCast);
        }

        [RuntimeExport("RhTypeCast_IsInstanceOfArray2")]
        public static unsafe object IsInstanceOfArray2(void* pvTargetType, object obj)
        {
            return IsInstanceOfArray(obj, pvTargetType);
        }

        [RuntimeExport("RhTypeCast_IsInstanceOfArray")]
        public static unsafe object IsInstanceOfArray(object obj, void* pvTargetType)
        {
            if (obj == null)
            {
                return null;
            }

            EEType* pTargetType = (EEType*)pvTargetType;
            EEType* pObjType = obj.EEType;

            // Debug.Assert(pTargetType->IsArray, "IsInstanceOfArray called with non-array EEType");
            // Debug.Assert(!pTargetType->IsCloned, "cloned array types are disallowed");

            // if the types match, we are done
            if (pObjType == pTargetType)
            {
                return obj;
            }

            // if the object is not an array, we're done
            if (!pObjType->IsArray)
            {
                return null;
            }

            // Debug.Assert(!pObjType->IsCloned, "cloned array types are disallowed");

            // compare the array types structurally

            if (pObjType->ParameterizedTypeShape != pTargetType->ParameterizedTypeShape)
            {
                // If the shapes are different, there's one more case to check for: Casting SzArray to MdArray rank 1.
                if (!pObjType->IsSzArray || pTargetType->ArrayRank != 1)
                {
                    return null;
                }
            }

            if (CastCache.AreTypesAssignableInternal(pObjType->RelatedParameterType, pTargetType->RelatedParameterType,
                AssignmentVariation.AllowSizeEquivalence, null))
            {
                return obj;
            }

            return null;
        }

        [RuntimeExport("RhTypeCast_CheckCastArray2")]
        public static unsafe object CheckCastArray2(void* pvTargetEEType, object obj)
        {
            return CheckCastArray(obj, pvTargetEEType);
        }

        [RuntimeExport("RhTypeCast_CheckCastArray")]
        public static unsafe object CheckCastArray(object obj, void* pvTargetEEType)
        {
            // a null value can be cast to anything
            if (obj == null)
                return null;

            object result = IsInstanceOfArray(obj, pvTargetEEType);

            if (result == null)
            {
                // Throw the invalid cast exception defined by the classlib, using the input EEType* 
                // to find the correct classlib.

                // throw ((EEType*)pvTargetEEType)->GetClasslibException(ExceptionIDs.InvalidCast);
            }

            return result;
        }

        /*
        [RuntimeExport("RhTypeCast_IsInstanceOfInterface2")]
        public static unsafe object IsInstanceOfInterface2(void* pvTargetType, object obj)
        {
            return IsInstanceOfInterface(obj, pvTargetType);
        }

        
        [RuntimeExport("RhTypeCast_IsInstanceOfInterface")]
        public static unsafe object IsInstanceOfInterface(object obj, void* pvTargetType)
        {
            if (obj == null)
            {
                return null;
            }

            EEType* pTargetType = (EEType*)pvTargetType;
            EEType* pObjType = obj.EEType;

            if (CastCache.AreTypesAssignableInternal_SourceNotTarget_BoxedSource(pObjType, pTargetType, null))
                return obj;

            // If object type implements ICastable then there's one more way to check whether it implements
            // the interface.
            if (pObjType->IsICastable && IsInstanceOfInterfaceViaCastableObject(obj, pTargetType))
                return obj;

            return null;
        }

        private static unsafe bool IsInstanceOfInterfaceViaCastableObject(object obj, EEType* pTargetType)
        {
            // To avoid stack overflow, it is not possible to implement the ICastableObject interface
            // itself via ICastableObject
            if (pTargetType->IsICastable)
                return false;

            // TODO!! BEGIN REMOVE THIS CODE WHEN WE REMOVE ICASTABLE
            // Call the ICastable.IsInstanceOfInterface method directly rather than via an interface
            // dispatch since we know the method address statically. We ignore any cast error exception
            // object passed back on failure (result == false) since IsInstanceOfInterface never throws.
            IntPtr pfnIsInstanceOfInterface = obj.EEType->ICastableIsInstanceOfInterfaceMethod;
            Exception castError = null;
            if (CalliIntrinsics.Call<bool>(pfnIsInstanceOfInterface, obj, pTargetType, out castError))
                return true;

            if (obj is CastableObjectSupport.ICastableObject)
            {
                // TODO!! END REMOVE THIS CODE WHEN WE REMOVE ICASTABLE

                // We ignore any cast error exception
                // object passed back on failure (result == false) since IsInstanceOfInterface never throws.
                CastableObjectSupport.ICastableObject castableObject = (CastableObjectSupport.ICastableObject)obj;
                Exception castableObjectCastError = null;
                if (CastableObjectSupport.GetCastableTargetIfPossible(castableObject, pTargetType, false, ref castableObjectCastError) != null)
                    return true;

                // TODO!! BEGIN REMOVE THIS CODE WHEN WE REMOVE ICASTABLE
            }
            // TODO!! END REMOVE THIS CODE WHEN WE REMOVE ICASTABLE
            return false;
        }
        
        private static unsafe bool IsInstanceOfInterfaceViaCastableObjectWithException(object obj, EEType* pTargetType, ref Exception castError)
        {
            // TODO!! BEGIN REMOVE THIS CODE WHEN WE REMOVE ICASTABLE
            // Call the ICastable.IsInstanceOfInterface method directly rather than via an interface
            // dispatch since we know the method address statically.
            IntPtr pfnIsInstanceOfInterface = obj.EEType->ICastableIsInstanceOfInterfaceMethod;
            if (CalliIntrinsics.Call<bool>(pfnIsInstanceOfInterface, obj, pTargetType, out castError))
                return true;
            if (obj is CastableObjectSupport.ICastableObject)
            {
                // TODO!! END REMOVE THIS CODE WHEN WE REMOVE ICASTABLE

                CastableObjectSupport.ICastableObject castableObject = (CastableObjectSupport.ICastableObject)obj;
                return CastableObjectSupport.GetCastableTargetIfPossible(castableObject, pTargetType, true, ref castError) != null;
                // TODO!! BEGIN REMOVE THIS CODE WHEN WE REMOVE ICASTABLE
            }
            return false;
            // TODO!! END REMOVE THIS CODE WHEN WE REMOVE ICASTABLE
        }
        

        internal static unsafe bool ImplementsInterface(EEType* pObjType, EEType* pTargetType, EETypePairList* pVisited)
        {
            // Debug.Assert(!pTargetType->IsParameterizedType, "did not expect paramterized type");
            // Debug.Assert(pTargetType->IsInterface, "IsInstanceOfInterface called with non-interface EEType");

            // This can happen with generic interface types
            // Debug.Assert(!pTargetType->IsCloned, "cloned interface types are disallowed");

            // canonicalize target type
            if (pTargetType->IsCloned)
                pTargetType = pTargetType->CanonicalEEType;

            int numInterfaces = pObjType->NumInterfaces;
            EEInterfaceInfo* interfaceMap = pObjType->InterfaceMap;
            for (int i = 0; i < numInterfaces; i++)
            {
                EEType* pInterfaceType = interfaceMap[i].InterfaceType;

                // canonicalize the interface type
                if (pInterfaceType->IsCloned)
                    pInterfaceType = pInterfaceType->CanonicalEEType;

                if (pInterfaceType == pTargetType)
                {
                    return true;
                }
            }

            // We did not find the interface type in the list of supported interfaces. There's still one
            // chance left: if the target interface is generic and one or more of its type parameters is co or
            // contra variant then the object can still match if it implements a different instantiation of
            // the interface with type compatible generic arguments.
            //
            // Interfaces which are only variant for arrays have the HasGenericVariance flag set even if they
            // are not variant.
            bool fArrayCovariance = pObjType->IsArray;
            if (pTargetType->HasGenericVariance)
            {
                // Grab details about the instantiation of the target generic interface.
                EEType* pTargetGenericType = pTargetType->GenericDefinition;
                EETypeRef* pTargetInstantiation = pTargetType->GenericArguments;
                int targetArity = (int)pTargetType->GenericArity;
                GenericVariance* pTargetVarianceInfo = pTargetType->GenericVariance;

                Debug.Assert(pTargetVarianceInfo != null, "did not expect empty variance info");


                for (int i = 0; i < numInterfaces; i++)
                {
                    EEType* pInterfaceType = interfaceMap[i].InterfaceType;

                    // We can ignore interfaces which are not also marked as having generic variance
                    // unless we're dealing with array covariance. 
                    //
                    // Interfaces which are only variant for arrays have the HasGenericVariance flag set even if they
                    // are not variant.
                    if (pInterfaceType->HasGenericVariance)
                    {
                        EEType* pInterfaceGenericType = pInterfaceType->GenericDefinition;

                        // If the generic types aren't the same then the types aren't compatible.
                        if (pInterfaceGenericType != pTargetGenericType)
                            continue;

                        // Grab instantiation details for the candidate interface.
                        EETypeRef* pInterfaceInstantiation = pInterfaceType->GenericArguments;
                        int interfaceArity = (int)pInterfaceType->GenericArity;
                        GenericVariance* pInterfaceVarianceInfo = pInterfaceType->GenericVariance;

                        Debug.Assert(pInterfaceVarianceInfo != null, "did not expect empty variance info");

                        // The types represent different instantiations of the same generic type. The
                        // arity of both had better be the same.
                        Debug.Assert(targetArity == interfaceArity, "arity mismatch betweeen generic instantiations");

                        // Compare the instantiations to see if they're compatible taking variance into account.
                        if (TypeParametersAreCompatible(targetArity,
                                                        pInterfaceInstantiation,
                                                        pTargetInstantiation,
                                                        pTargetVarianceInfo,
                                                        fArrayCovariance,
                                                        pVisited))
                            return true;
                    }
                }
            }

            // Interface type equivalence check.
            // Currently only implemented to allow ICastable to be defined in multiple type spaces
            if (pTargetType->IsICastable)
            {
                for (int i = 0; i < numInterfaces; i++)
                {
                    EEType* pInterfaceType = interfaceMap[i].InterfaceType;
                    if (pInterfaceType->IsICastable)
                        return true;
                }
            }

            return false;
        }

         
        [RuntimeExport("RhTypeCast_CheckCastInterface2")]
        public static unsafe object CheckCastInterface2(void* pvTargetEEType, object obj)
        {
            return CheckCastInterface(obj, pvTargetEEType);
        }

        
        //
        // Array stelem/ldelema helpers with RyuJIT conventions
        //
        

        [RuntimeExport("RhTypeCast_AreTypesEquivalent")]
        static unsafe public bool AreTypesEquivalent(EETypePtr pType1, EETypePtr pType2)
        {
            return (AreTypesEquivalentInternal(pType1.ToPointer(), pType2.ToPointer()));
        }
         

        [RuntimeExport("RhTypeCast_IsInstanceOf2")]  // Helper with RyuJIT calling convention
        public static unsafe object IsInstanceOf2(void* pvTargetType, object obj)
        {
            return IsInstanceOf(obj, pvTargetType);
        }
        */

        // this is necessary for shared generic code - Foo<T> may be executing
        // for T being an interface, an array or a class
        [RuntimeExport("RhTypeCast_IsInstanceOf")]
        public static unsafe object IsInstanceOf(object obj, void* pvTargetType)
        {
            // @TODO: consider using the cache directly, but beware of ICastable in the interface case
            EEType* pTargetType = (EEType*)pvTargetType;

            Debugger.Break();
            //if (pTargetType->IsArray)
            //    return IsInstanceOfArray(obj, pvTargetType);
            //else if (pTargetType->IsInterface)
            //    return IsInstanceOfInterface(obj, pvTargetType);
            //else
                return IsInstanceOfClass(obj, pvTargetType);
        }
        
      

        internal unsafe struct EETypePairList
        {
            private EEType* _eetype1;
            private EEType* _eetype2;
            private EETypePairList* _next;

            public EETypePairList(EEType* pEEType1, EEType* pEEType2, EETypePairList* pNext)
            {
                _eetype1 = pEEType1;
                _eetype2 = pEEType2;
                _next = pNext;
            }

            public static bool Exists(EETypePairList* pList, EEType* pEEType1, EEType* pEEType2)
            {
                while (pList != null)
                {
                    if (pList->_eetype1 == pEEType1 && pList->_eetype2 == pEEType2)
                        return true;
                    if (pList->_eetype1 == pEEType2 && pList->_eetype2 == pEEType1)
                        return true;
                    pList = pList->_next;
                }
                return false;
            }
        }
        
        // source type + target type + assignment variation -> true/false
        [System.Runtime.CompilerServices.EagerStaticClassConstructionAttribute]
        private static class CastCache
        {
            //
            // Cache size parameters
            //

            // Start with small cache size so that the cache entries used by startup one-time only initialization
            // will get flushed soon
            private const int InitialCacheSize = 128; // MUST BE A POWER OF TWO
            private const int DefaultCacheSize = 1024;
            private const int MaximumCacheSize = 128 * 1024;

            //
            // Cache state
            //
            private static Entry[] s_cache = new Entry[InitialCacheSize];   // Initialize the cache eagerly to avoid null checks.
            private static UnsafeGCHandle s_previousCache;
            private static long s_tickCountOfLastOverflow = InternalCalls.PalGetTickCount64();
            private static int s_entries;
            private static bool s_roundRobinFlushing;


            private sealed class Entry
            {
                public Entry Next;
                public Key Key;
                public bool Result;     // @TODO: consider storing this bit in the Key -- there is room
            }

            private unsafe struct Key
            {
                private IntPtr _sourceTypeAndVariation;
                private IntPtr _targetType;

                public Key(EEType* pSourceType, EEType* pTargetType, AssignmentVariation variation)
                {
                    Debug.Assert((((long)pSourceType) & 3) == 0, "misaligned EEType!");
                    Debug.Assert(((uint)variation) <= 3, "variation enum has an unexpectedly large value!");

                    _sourceTypeAndVariation = (IntPtr)(((byte*)pSourceType) + ((int)variation));
                    _targetType = (IntPtr)pTargetType;
                }

                private static int GetHashCode(IntPtr intptr)
                {
                    return unchecked((int)((long)intptr));
                }

                public int CalculateHashCode()
                {
                    return ((GetHashCode(_targetType) >> 4) ^ GetHashCode(_sourceTypeAndVariation));
                }

                public bool Equals(ref Key other)
                {
                    return (_sourceTypeAndVariation == other._sourceTypeAndVariation) && (_targetType == other._targetType);
                }

                public AssignmentVariation Variation
                {
                    get { return (AssignmentVariation)(unchecked((int)(long)_sourceTypeAndVariation) & 3); }
                }

                public EEType* SourceType { get { return (EEType*)(((long)_sourceTypeAndVariation) & ~3L); } }
                public EEType* TargetType { get { return (EEType*)_targetType; } }
            }

            public static unsafe bool AreTypesAssignableInternal(EEType* pSourceType, EEType* pTargetType, AssignmentVariation variation, EETypePairList* pVisited)
            {
                // Important special case -- it breaks infinite recursion in CastCache itself!
                if (pSourceType == pTargetType)
                    return true;

                Key key = new Key(pSourceType, pTargetType, variation);
                Entry entry = LookupInCache(s_cache, ref key);
                if (entry == null)
                    return CacheMiss(ref key, pVisited);

                return entry.Result;
            }

            // This method is an optimized and customized version of AreTypesAssignable that achieves better performance
            // than AreTypesAssignableInternal through 2 significant changes
            // 1. Removal of sourceType to targetType check (This propery must be known before calling this function. At time
            //    of writing, this is true as its is only used if sourceType is from an object, and targetType is an interface.)
            // 2. Force inlining (This particular variant is only used in a small number of dispatch scenarios that are particularly
            //    high in performance impact.)
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static unsafe bool AreTypesAssignableInternal_SourceNotTarget_BoxedSource(EEType* pSourceType, EEType* pTargetType, EETypePairList* pVisited)
            {
                Debug.Assert(pSourceType != pTargetType, "target is source");
                Key key = new Key(pSourceType, pTargetType, AssignmentVariation.BoxedSource);
                Entry entry = LookupInCache(s_cache, ref key);
                if (entry == null)
                    return CacheMiss(ref key, pVisited);

                return entry.Result;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static Entry LookupInCache(Entry[] cache, ref Key key)
            {
                int entryIndex = key.CalculateHashCode() & (cache.Length - 1);
                Entry entry = cache[entryIndex];
                while (entry != null)
                {
                    if (entry.Key.Equals(ref key))
                        break;
                    entry = entry.Next;
                }
                return entry;
            }

            private static unsafe bool CacheMiss(ref Key key, EETypePairList* pVisited)
            {
                //
                // First, check if we previously visited the input types pair, to avoid infinite recursions
                //
                if (EETypePairList.Exists(pVisited, key.SourceType, key.TargetType))
                    return false;

                bool result = false;
                // bool previouslyCached = false;

                //
                // Try to find the entry in the previous version of the cache that is kept alive by weak reference
                //
                if (s_previousCache.IsAllocated)
                {
                    // Unchecked cast to avoid recursive dependency on array casting
                    Entry[] previousCache = Unsafe.As<Entry[]>(s_previousCache.Target);
                    if (previousCache != null)
                    {
                        Entry previousEntry = LookupInCache(previousCache, ref key);
                        if (previousEntry != null)
                        {
                            result = previousEntry.Result;
                            // previouslyCached = true;
                        }
                    }
                }

                //
                // Call into the type cast code to calculate the result
                //
                Debugger.Break();
                /*
                if (!previouslyCached)
                {
                    EETypePairList newList = new EETypePairList(key.SourceType, key.TargetType, pVisited);
                    result = TypeCast.AreTypesAssignableInternal(key.SourceType, key.TargetType, key.Variation, &newList);
                }

                //
                // Update the cache under the lock
                //
                InternalCalls.RhpAcquireCastCacheLock();
                */
                try
                {
                    try
                    {
                        // Avoid duplicate entries
                        Entry existingEntry = LookupInCache(s_cache, ref key);
                        if (existingEntry != null)
                            return existingEntry.Result;

                        // Resize cache as necessary
                        Entry[] cache = ResizeCacheForNewEntryAsNecessary();

                        int entryIndex = key.CalculateHashCode() & (cache.Length - 1);

                        Entry newEntry = new Entry() { Key = key, Result = result, Next = cache[entryIndex] };

                        // BEWARE: Array store check can lead to infinite recursion. We avoid this by making certain 
                        // that the cache trivially answers the case of equivalent types without triggering the cache
                        // miss path. (See CastCache.AreTypesAssignableInternal)
                        cache[entryIndex] = newEntry;
                        return newEntry.Result;
                    }
                    catch (OutOfMemoryException)
                    {
                        // Entry allocation failed -- but we can still return the correct cast result.
                        return result;
                    }
                }
                finally
                {
                    InternalCalls.RhpReleaseCastCacheLock();
                }
            }

            private static Entry[] ResizeCacheForNewEntryAsNecessary()
            {
                Entry[] cache = s_cache;

                int entries = s_entries++;

                // If the cache has spare space, we are done
                if (2 * entries < cache.Length)
                {
                    if (s_roundRobinFlushing)
                    {
                        cache[2 * entries] = null;
                        cache[2 * entries + 1] = null;
                    }
                    return cache;
                }

                //
                // Now, we have cache that is overflowing with results. We need to decide whether to resize it or start 
                // flushing the old entries instead
                //

                // Start over counting the entries
                s_entries = 0;

                // See how long it has been since the last time the cache was overflowing
                long tickCount = InternalCalls.PalGetTickCount64();
                int tickCountSinceLastOverflow = (int)(tickCount - s_tickCountOfLastOverflow);
                s_tickCountOfLastOverflow = tickCount;

                bool shrinkCache = false;
                bool growCache = false;

                if (cache.Length < DefaultCacheSize)
                {
                    // If the cache have not reached the default size, just grow it without thinking about it much
                    growCache = true;
                }
                else
                {
                    if (tickCountSinceLastOverflow < cache.Length)
                    {
                        // We 'overflow' when 2*entries == cache.Length, so we have cache.Length / 2 entries that were
                        // filled in tickCountSinceLastOverflow ms, which is 2ms/entry

                        // If the fill rate of the cache is faster than ~2ms per entry, grow it
                        if (cache.Length < MaximumCacheSize)
                            growCache = true;
                    }
                    else
                    if (tickCountSinceLastOverflow > cache.Length * 16)
                    {
                        // We 'overflow' when 2*entries == cache.Length, so we have ((cache.Length*16) / 2) entries that
                        // were filled in tickCountSinceLastOverflow ms, which is 32ms/entry

                        // If the fill rate of the cache is slower than 32ms per entry, shrink it
                        if (cache.Length > DefaultCacheSize)
                            shrinkCache = true;
                    }
                    // Otherwise, keep the current size and just keep flushing the entries round robin
                }

                Entry[] newCache = null;
                if (growCache || shrinkCache)
                {
                    try
                    {
                        newCache = new Entry[shrinkCache ? (cache.Length / 2) : (cache.Length * 2)];
                    }
                    catch (OutOfMemoryException)
                    {
                        // Failed to allocate a bigger/smaller cache.  That is fine, keep the old one.
                    }
                }

                if (newCache != null)
                {
                    s_roundRobinFlushing = false;

                    // Keep the reference to the old cache in a weak handle. We will try to use it to avoid hitting the 
                    // cache miss path until the GC collects it.
                    if (s_previousCache.IsAllocated)
                    {
                        s_previousCache.Target = cache;
                    }
                    else
                    {
                        try
                        {
                            s_previousCache = UnsafeGCHandle.Alloc(cache, GCHandleType.Weak);
                        }
                        catch (OutOfMemoryException)
                        {
                            // Failed to allocate the handle to utilize the old cache, that is fine, we will just miss
                            // out on repopulating the new cache from the old cache.
                            s_previousCache = default(UnsafeGCHandle);
                        }
                    }

                    return s_cache = newCache;
                }
                else
                {
                    s_roundRobinFlushing = true;
                    return cache;
                }
            }
        }
    }

    public enum CallingConvention2
    {
        Winapi = 1,
        Cdecl = 2,
        StdCall = 3,
        ThisCall = 4,
        FastCall = 5,
    }

    // This class represents the void return type
    public struct Void
    {
    }
}


namespace Internal.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ObjHeader
    {
        // Contents of the object header
        private IntPtr _objHeaderContents;
    }
}


namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DllImportAttribute : Attribute
    {
        public CallingConvention CallingConvention;

        public DllImportAttribute(string dllName)
        {
        }
    }
}
