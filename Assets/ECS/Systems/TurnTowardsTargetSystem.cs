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

public class TurnTowardsTargetSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;
    EntityQuery entityQuery;

    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        entityQuery = GetEntityQuery(
            ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Rotation)),
            ComponentType.ReadOnly(typeof(RotationSpeed)),
            ComponentType.ReadOnly(typeof(TargetSelection)),
            ComponentType.ReadOnly(typeof(TurnTowardsTarget)));
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var turnTowardTargetArray = entityQuery.ToComponentDataArray<TargetSelection>(Allocator.TempJob);
        var targetIsValidArray = new NativeArray<bool>(turnTowardTargetArray.Length, Allocator.TempJob);
        var targetTranslations = new NativeArray<Translation>(turnTowardTargetArray.Length, Allocator.TempJob);

        for (int i = 0; i < turnTowardTargetArray.Length; i++)
        {
            var target = turnTowardTargetArray[i].target;
            targetIsValidArray[i] = World.Active.EntityManager.Exists(target) && World.Active.EntityManager.HasComponent<Translation>(target);
            if(targetIsValidArray[i])
                targetTranslations[i] = World.Active.EntityManager.GetComponentData<Translation>(target);
        }

        turnTowardTargetArray.Dispose();

        JobHandle validateTargets = new RemoveComponentJob
        {
            targetExists = targetIsValidArray,
            commandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(validateTargets);

        JobHandle rotateTowardsTargets = new RotateJob
        {
            dt = Time.deltaTime,
            targetTranslations = targetTranslations,
            targetExists = targetIsValidArray
        }.Schedule(this, inputDeps);

        JobHandle deallocDep = JobHandle.CombineDependencies(validateTargets, rotateTowardsTargets);

        JobHandle deallocateJob = new DeallocateJob<bool>
        {
            toDeallocate = targetIsValidArray
        }.Schedule(deallocDep);

        return deallocateJob;
    }

    protected override void OnDestroyManager()
    {
        base.OnDestroyManager();
    }

    [RequireComponentTag(typeof(TurnTowardsTarget), typeof(TargetSelection), typeof(Rotation), typeof(RotationSpeed))]
    struct RemoveComponentJob : IJobForEachWithEntity<Translation>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;
        [ReadOnly] public NativeArray<bool> targetExists;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation trans)
        {
            if (!targetExists[index])
            {
                commandBuffer.RemoveComponent<TargetSelection>(index, entity);
            }
        }
    }

    [RequireComponentTag(typeof(TurnTowardsTarget), typeof(TargetSelection))]
    [BurstCompile]
    struct RotateJob : IJobForEachWithEntity<Translation, Rotation, RotationSpeed>
    {
        public float dt;
        [ReadOnly] public NativeArray<bool> targetExists;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> targetTranslations;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
        {
            if (!targetExists[index])
            {
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