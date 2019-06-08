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
        JobHandle moveJob = new MoveJob
        {
            dt = Time.deltaTime
        }.Schedule(this, inputDeps);    

        return moveJob;
    }

    [ExcludeComponent(typeof(Velocity))]
    [BurstCompile]
    struct MoveJob : IJobForEach<Translation, MoveSpeed, Rotation>
    {
        public float dt;
        public void Execute(ref Translation translation, [ReadOnly] ref MoveSpeed moveSpeed, [ReadOnly] ref Rotation rotation)
        {
            var x = rotation.Value.left() * moveSpeed.Value.x;
            var y = rotation.Value.up() * moveSpeed.Value.y;
            var z = rotation.Value.forward() * moveSpeed.Value.z;
            translation.Value += (x + y + z) * dt;
        }
    }
}

public class VelocityMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle velocityMoveJob = new MoveVelocityJob
        {
            dt = Time.deltaTime
        }.Schedule(this, inputDeps);

        return velocityMoveJob;
    }

    [BurstCompile]
    struct MoveVelocityJob : IJobForEach<Translation, MoveSpeed, Rotation, Velocity>
    {
        public float dt;
        public void Execute(ref Translation translation, [ReadOnly] ref MoveSpeed moveSpeed, [ReadOnly] ref Rotation rotation, ref Velocity velocity)
        {
            var x = rotation.Value.left() * moveSpeed.Value.x;
            var y = rotation.Value.up() * moveSpeed.Value.y;
            var z = rotation.Value.forward() * moveSpeed.Value.z;
            velocity.Value += (x + y + z) * dt;
            translation.Value += velocity.Value * dt;
        }
    }
}