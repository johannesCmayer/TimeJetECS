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

public class ExplosionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<ExplosionTag>().ForEach((Entity e, ref Scale s, ref DestroyAfterTime dat) => {
            s.Value = dat.timeToDestruction * 10;
        });
    }
}
