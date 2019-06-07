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

public class DebugDrawSphereColliders : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation t, ref SphereCollider c) => {
            
        });
    }
}
