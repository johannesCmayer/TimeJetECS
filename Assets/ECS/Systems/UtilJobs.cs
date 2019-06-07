using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public struct DeallocateJob<T> : IJob where T : struct
{
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<T> toDeallocate;

    public void Execute() { }
}
