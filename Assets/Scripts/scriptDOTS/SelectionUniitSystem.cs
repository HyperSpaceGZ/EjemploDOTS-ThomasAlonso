using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct SelectionUniitSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach  (RefRO<SelectedUnit> selected in SystemAPI.Query<RefRO<SelectedUnit>>().WithDisabled<SelectedUnit>())
        {
            RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEffect);
            visualLocalTransform.ValueRW.Scale = 0f;
        }

        foreach (RefRO<SelectedUnit> selected in SystemAPI.Query<RefRO<SelectedUnit>>())
        {
            RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEffect);
            visualLocalTransform.ValueRW.Scale = selected.ValueRO.DepthScale;

        }
    }

   
}
