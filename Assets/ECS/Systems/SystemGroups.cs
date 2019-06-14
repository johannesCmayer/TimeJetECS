using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class CTransformSystemGroup : ComponentSystemGroup { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TransformSystemGroup))]
public class SpawningSystemGroup : ComponentSystemGroup { }
