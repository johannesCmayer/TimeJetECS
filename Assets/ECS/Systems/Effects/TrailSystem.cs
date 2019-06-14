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

public class TrailPool
{
    public Stack<int> freeTrailsIdx = new Stack<int>();
    public List<TrailRenderer> trails = new List<TrailRenderer>();
    public GameObject prefab;
    public TrailRenderer prefabTrail;

    public TrailPool(GameObject prefab)
    {
        this.prefab = prefab;
        prefabTrail = prefab.GetComponent<TrailRenderer>();
    }
}

public class TrailSystem : ComponentSystem
{
    Dictionary<TrailID, TrailPool> trailMap = new Dictionary<TrailID, TrailPool>();

    protected override void OnCreate()
    {
        Setup.StartHook += SetupTrails;
        
    }

    void SetupTrails()
    {
        foreach (TrailID id in System.Enum.GetValues(typeof(TrailID)))
        {
            if (id != TrailID.NotSet)
                trailMap.Add(id, new TrailPool(GlobalData.instance.GetTrail(id)));
        }
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<TrailSystemState>().ForEach((Entity e, ref Translation t, ref HasTrail hasTrail) =>
        {
            PostUpdateCommands.AddComponent(e, new TrailSystemState { trailIdx = AquireTrailIdx(t.Value, hasTrail.trailID), trailID = hasTrail.trailID });
        });

        Entities.WithAll<HasTrail>().ForEach((Entity e, ref Translation t, ref TrailSystemState tss) =>
        {
            trailMap[tss.trailID].trails[tss.trailIdx].transform.position = t.Value;
        });

        Entities.WithNone<HasTrail>().ForEach((Entity e, ref TrailSystemState tss) =>
        {
            var map = trailMap[tss.trailID];

            var lengthFadeSpeed = 2f;
            var widthFadeSpeed = 1f;

            map.trails[tss.trailIdx].time = math.max(0, map.trails[tss.trailIdx].time - Time.deltaTime * lengthFadeSpeed);
            map.trails[tss.trailIdx].widthMultiplier = math.max(0, map.trails[tss.trailIdx].widthMultiplier - Time.deltaTime * widthFadeSpeed);

            if (map.trails[tss.trailIdx].time <= 0 || map.trails[tss.trailIdx].widthMultiplier <= 0)
            {
                map.freeTrailsIdx.Push(tss.trailIdx);
                PostUpdateCommands.RemoveComponent<TrailSystemState>(e);
            }
        });
    }

    public int AquireTrailIdx(float3 pos, TrailID trailID)
    {
        var map = trailMap[trailID];
        if (map.freeTrailsIdx.Count == 0)
        {
            map.trails.Add(MonoBehaviour.Instantiate(map.prefab, pos, quaternion.identity).GetComponent<TrailRenderer>());
            return map.trails.Count - 1;
        }
        else
        {
            var idx = map.freeTrailsIdx.Pop();
            var trailRend = map.trails[idx];
            trailRend.transform.position = pos;
            trailRend.Clear();            
            map.trails[idx].time = map.prefabTrail.time;
            map.trails[idx].widthMultiplier = map.prefabTrail.widthMultiplier;
            return idx;
        }
    }
}
