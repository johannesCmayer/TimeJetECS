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

public class TrailSystem : ComponentSystem
{
    public Stack<int> freeTrailsIdx = new Stack<int>();
    public List<TrailRenderer> trails = new List<TrailRenderer>();

    protected override void OnUpdate()
    {
        Entities.WithNone<TrailSystemState>().WithAll<HasTrailTag>().ForEach((Entity e, ref Translation t) =>
        {
            PostUpdateCommands.AddComponent(e, new TrailSystemState { trailIdx = AquireTrailIdx(t.Value) });
        });

        Entities.WithAll<HasTrailTag>().ForEach((Entity e, ref Translation t, ref TrailSystemState tss) =>
        {
            trails[tss.trailIdx].transform.position = t.Value;
        });

        Entities.WithNone<HasTrailTag>().WithAll<TrailSystemState>().ForEach((Entity e, ref TrailSystemState tss) =>
        {
            var lengthFadeSpeed = 2f;
            var widthFadeSpeed = 1f;

            trails[tss.trailIdx].time = math.max(0, trails[tss.trailIdx].time - Time.deltaTime * lengthFadeSpeed);
            trails[tss.trailIdx].widthMultiplier = math.max(0, trails[tss.trailIdx].widthMultiplier - Time.deltaTime * widthFadeSpeed);

            if (trails[tss.trailIdx].time <= 0 || trails[tss.trailIdx].widthMultiplier <= 0)
            {
                freeTrailsIdx.Push(tss.trailIdx);
                PostUpdateCommands.RemoveComponent<TrailSystemState>(e);
            }
        });
    }

    public int AquireTrailIdx(float3 pos)
    {
        if (freeTrailsIdx.Count == 0)
        {
            trails.Add(MonoBehaviour.Instantiate(GlobalData.instance.TrailRenderer, pos, quaternion.identity).GetComponent<TrailRenderer>());
            return trails.Count - 1;
        }
        else
        {
            var idx = freeTrailsIdx.Pop();
            var trail = trails[idx];
            trail.GetComponent<TrailRenderer>().Clear();
            return idx;
        }
    }
}
