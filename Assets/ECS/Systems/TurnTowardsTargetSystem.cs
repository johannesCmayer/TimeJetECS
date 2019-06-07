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

/*
public class TurnTowardsTargetSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var eq = GetEntityQuery(
            ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Rotation)),
            ComponentType.ReadOnly(typeof(RotationSpeed)),
            ComponentType.ReadOnly(typeof(TurnTowardsTarget)));
        var e = eq.ToComponentDataArray<TurnTowardsTarget>(Allocator.TempJob);
        var targetTranslations = new NativeArray<Translation>(e.Length, Allocator.TempJob);
        for (int i = 0; i < e.Length; i++)
        {
            targetTranslations[i] = World.Active.EntityManager.GetComponentData<Translation>(e[i].target);
        }
        e.Dispose();

        var j = new RotateJob
        {
            dt = Time.deltaTime,
            targetTranslations = targetTranslations
        };
        inputDeps = j.Schedule(this, inputDeps);
        return inputDeps;
    }

    [RequireComponentTag(typeof(TurnTowardsTarget))]
    [BurstCompile]
    struct RotateJob : IJobForEachWithEntity<Translation, Rotation, RotationSpeed>
    {
        public float dt;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> targetTranslations;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
        {
            var targetDir = math.normalize(targetTranslations[index].Value - translation.Value);
            var forward = rotation.Value.forward();
            var offset = (targetDir - forward) * rotationSpeed.Value * dt;
            var newForward = forward + offset; 

            rotation.Value = quaternion.LookRotation(newForward, rotation.Value.up());
        }
    }
}*/


public class TurnTowardsTargetSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var eq = GetEntityQuery(
            ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Rotation)),
            ComponentType.ReadOnly(typeof(RotationSpeed)),
            ComponentType.ReadOnly(typeof(TargetSelection)),
            ComponentType.ReadOnly(typeof(TurnTowardsTarget)));
        var turnTowardTargetArray = eq.ToComponentDataArray<TargetSelection>(Allocator.TempJob);
        var targetExistsArray = new NativeArray<bool>(turnTowardTargetArray.Length, Allocator.TempJob);
        var targetTranslations = new NativeArray<Translation>(turnTowardTargetArray.Length, Allocator.TempJob);
        for (int i = 0; i < turnTowardTargetArray.Length; i++)
        {            
            targetExistsArray[i] = World.Active.EntityManager.Exists(turnTowardTargetArray[i].target);
            if(targetExistsArray[i])
                targetTranslations[i] = World.Active.EntityManager.GetComponentData<Translation>(turnTowardTargetArray[i].target);
        }
        turnTowardTargetArray.Dispose();

        RotateJob j = new RotateJob
        {
            dt = Time.deltaTime,
            targetTranslations = targetTranslations,
            targetExists = targetExistsArray,
            commandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        inputDeps = j.Schedule(this, inputDeps);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(inputDeps);

        return inputDeps;
    }

    [RequireComponentTag(typeof(TurnTowardsTarget), typeof(TargetSelection))]
    //[BurstCompile]
    struct RotateJob : IJobForEachWithEntity<Translation, Rotation, RotationSpeed>
    {
        public float dt;
        public EntityCommandBuffer.Concurrent commandBuffer;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<bool> targetExists;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> targetTranslations;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
        {
            if (!targetExists[index])
            {
                commandBuffer.RemoveComponent<TargetSelection>(index, entity);
                return;
            }

            var targetDir = math.normalize(targetTranslations[index].Value - translation.Value);
            var forward = rotation.Value.forward();
            var offset = (targetDir - forward) * rotationSpeed.Value * dt;
            var newForward = forward + offset;

            rotation.Value = quaternion.LookRotation(newForward, rotation.Value.up());
        }
    }
}