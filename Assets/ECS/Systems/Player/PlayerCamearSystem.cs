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

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(RenderMeshSystemV2))]
public class PlayerCamearSystem : ComponentSystem
{
    //float3 camOffset = new float3(0, -3, 0);
    float3 camOffset = float3.zero;

    public GameObject playerCamRoot;

    protected override void OnCreate()
    {
        playerCamRoot = GameObject.FindGameObjectWithTag("PlayerCameraRoot");
    }

    protected override void OnUpdate()
    {
        playerCamRoot = GameObject.FindGameObjectWithTag("PlayerCameraRoot");

        Entities.WithAll<PlayerTag>().ForEach((ref Translation translation, ref Rotation rotation, ref LocalToWorld localToWorld) => {
            var x = localToWorld.Forward * camOffset.z;
            var y = localToWorld.Up * camOffset.y;
            var z = localToWorld.Right * camOffset.x;

            var relativeCamOffset = x + y + z;

            var camTrans = playerCamRoot.transform;
            camTrans.rotation = rotation.Value;
            camTrans.position = translation.Value + relativeCamOffset;
        });
    }
}
