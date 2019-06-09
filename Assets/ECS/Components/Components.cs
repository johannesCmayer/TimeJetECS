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

public struct PlayerTag : IComponentData { }
public struct FriendlyAITag : IComponentData { }
public struct EnemyAITag : IComponentData { }
public struct MissileTag : IComponentData { }
public struct HasTrailTag : IComponentData { }
public struct ExplosionTag : IComponentData { }

public struct TrailSystemState : ISystemStateComponentData {
    public int trailIdx;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}

public struct AngularVelocity : IComponentData
{
    public float3 Value;
}

public struct TurnTowardsTarget : IComponentData { }

public struct TargetSelection : IComponentData
{
    public Entity target;
}

public struct MoveSpeed : IComponentData
{
    public float3 Value;
}

public struct RotationSpeed : IComponentData
{
    public float Value;
}

public struct SteeringInput : IComponentData
{
    public float pitch;
    public float yaw;
    public float roll;
}

public struct FireWeapon : IComponentData
{
    public float cooldown;
}

public struct SphereCollider : IComponentData
{
    public float size;
}

public struct DestroyAfterTime : IComponentData
{
    public float timeToDestruction;
}
