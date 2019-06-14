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

[UpdateInGroup(typeof(CTransformSystemGroup))]
public class MovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle moveJob = new MoveJob
        {
            dt = Time.deltaTime,
            udt = Time.unscaledDeltaTime
        }.Schedule(this, inputDeps);    


        return moveJob;
    }

    [ExcludeComponent(typeof(Velocity))]
    [BurstCompile]
    struct MoveJob : IJobForEachWithEntity<Translation, MoveSpeed, Rotation, LocalToWorld>
    {
        public float dt;
        public float udt;
        public void Execute(Entity entity, int index, ref Translation translation, [ReadOnly] ref MoveSpeed moveSpeed, [ReadOnly] ref Rotation rotation, [ReadOnly] ref LocalToWorld localToWorld)
        {
            var x = localToWorld.Right * moveSpeed.Value.x;
            var y = localToWorld.Up * moveSpeed.Value.y;
            var z = localToWorld.Forward * moveSpeed.Value.z;
            translation.Value += (x + y + z) * dt;
        }
    }
}

[UpdateInGroup(typeof(CTransformSystemGroup))]
public class VelocityMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle velocityMoveJob = new MoveVelocityJob
        {
            dt = Time.deltaTime,
            udt = Time.unscaledDeltaTime
        }.Schedule(this, inputDeps);

        return velocityMoveJob;
    }

    [BurstCompile]
    struct MoveVelocityJob : IJobForEachWithEntity<Translation, Velocity, MoveSpeed, Rotation, LocalToWorld>
    {
        public float dt;
        public float udt;
        public void Execute(Entity entity, int index, ref Translation translation, ref Velocity velocity, [ReadOnly] ref MoveSpeed moveSpeed, [ReadOnly] ref Rotation rotation, [ReadOnly] ref LocalToWorld localToWorld)
        {
            var x = localToWorld.Right * moveSpeed.Value.x;
            var y = localToWorld.Up * moveSpeed.Value.y;
            var z = localToWorld.Forward * moveSpeed.Value.z;

            velocity.Value += (x + y + z) * dt;
            translation.Value += velocity.Value * dt;
        }
    }
}

[UpdateInGroup(typeof(CTransformSystemGroup))]
public class VelocityMovementChunkSystem : JobComponentSystem
{
    EntityQuery querry;

    protected override void OnCreate()
    {
        querry = GetEntityQuery(typeof(Translation), typeof(Velocity), ComponentType.ReadOnly<MoveSpeed>(), ComponentType.ReadOnly<Rotation>(), ComponentType.ReadOnly<LocalToWorld>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        JobHandle velocityMoveJob = new MoveVelocityJob
        {
            dt = Time.deltaTime,
            udt = Time.unscaledDeltaTime
        }.Schedule(querry, inputDeps);

        return velocityMoveJob;
    }

    [BurstCompile]
    struct MoveVelocityJob : IJobChunk
    {
        public float dt;
        public float udt;


        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var deltaTime = dt;
            if (chunk.Has<UseUnscaledDeltatime>())
                deltaTime = udt;
            var tranlation.Value = chunk.GetNativeArray();

            var x = localToWorld.Right * moveSpeed.Value.x;
            var y = localToWorld.Up * moveSpeed.Value.y;
            var z = localToWorld.Forward * moveSpeed.Value.z;

            velocity.Value += (x + y + z) * dt;
            translation.Value += velocity.Value * dt;
        }
    }
}
