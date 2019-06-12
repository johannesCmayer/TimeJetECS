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


public class PlayerSpawnSystem : ComponentSystem
{
    protected override void OnUpdate() {
        Entities.WithAll<Alive>().WithNone<RespawnSystemState>().ForEach((Entity e, ref Respawn r) => {
            PostUpdateCommands.AddComponent(e, new RespawnSystemState { pos = r.pos, prefab = r.prefab });
            PostUpdateCommands.RemoveComponent<Respawn>(e);
        });

        Entities.WithNone<Alive>().WithAll<RespawnSystemState>().ForEach((Entity e, ref RespawnSystemState respawnSystemState) => {
            if (!EntityManager.Exists(respawnSystemState.prefab))
            {
                Debug.LogWarning($"No prefab {respawnSystemState.prefab} fround to respawn {e}");
                PostUpdateCommands.RemoveComponent<RespawnSystemState>(e);
                return;
            }

            var newE = PostUpdateCommands.Instantiate(respawnSystemState.prefab);
            PostUpdateCommands.SetComponent(newE, new Translation { Value = respawnSystemState.pos });
            PostUpdateCommands.RemoveComponent<RespawnSystemState>(e);
        });
    }
}
