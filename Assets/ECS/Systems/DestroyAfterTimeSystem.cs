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

public class DestroyAfterTimeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref DestroyAfterTime dat) => {
            dat.timeToDestruction -= Time.deltaTime;
            if (dat.timeToDestruction <= 0)
                PostUpdateCommands.DestroyEntity(e);
        });
    }
}
