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

public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var j = new MoveJob
        {
            dt = Time.deltaTime
        };
        inputDeps = j.Schedule(this, inputDeps);    
        return inputDeps;
    }

    [BurstCompile]
    struct MoveJob : IJobForEachWithEntity<Translation, MoveSpeed, Rotation>
    {
        public float dt;
        public void Execute(Entity entity, int index, ref Translation translation, [ReadOnly] ref MoveSpeed moveSpeed, [ReadOnly] ref Rotation rotation)
        {
            translation.Value += rotation.Value.forward() * moveSpeed.Value * dt;
        }
    }
}