using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R = UnityEngine.Random;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public static class A
{
    public static float3 forward(this quaternion quat)
    {
        var a = new quaternion(0, 0, 1, 0);
        var r = math.mul(math.mul(quat.value, a), math.inverse(quat.value));
        return new float3(r.value.x, r.value.y, r.value.z);
    }

    public static float3 up(this quaternion quat)
    {
        var a = new quaternion(0, 1, 0, 0);
        var r = math.mul(math.mul(quat.value, a), math.inverse(quat.value));
        return new float3(r.value.x, r.value.y, r.value.z);
    }

    public static float3 right(this quaternion quat)
    {
        var a = new quaternion(1, 0, 0, 0);
        var r = math.mul(math.mul(quat.value, a), math.inverse(quat.value));
        return new float3(r.value.x, r.value.y, r.value.z);
    }

}
