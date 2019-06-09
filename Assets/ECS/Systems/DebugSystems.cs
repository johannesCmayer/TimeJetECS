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

[DisableAutoCreation]
public class DebugDrawLocalDirVecs : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation r, ref Translation t) =>
        {
            var dir = new float3[] { r.Value.forward(), r.Value.up(), r.Value.right() };
            var colors = new Color[] { Color.blue, Color.green, Color.red };

            for (int i = 0; i < dir.Length; i++)
            {
                Debug.DrawLine(t.Value, t.Value + dir[i] * 2, colors[i]);
            }
        });
    }
}

//[DisableAutoCreation]
public class DebugDrawSphereColliders : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation t, ref SphereCollider c) => {
            
        });
    }
}
