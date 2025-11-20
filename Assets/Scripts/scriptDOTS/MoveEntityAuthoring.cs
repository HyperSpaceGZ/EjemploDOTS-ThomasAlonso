using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoveEntityAuthoring : MonoBehaviour
{
    public float MoveSpeed;
    public float RotationSpeed;
    public class Baker : Baker<MoveEntityAuthoring>
    {
        public override void Bake(MoveEntityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveUnitComponent
            {
                MoveSpeed = authoring.MoveSpeed,
                RotationSpeed = authoring.RotationSpeed     
            });
                
            
        }
    }


}
public struct MoveUnitComponent : IComponentData
{
    public float MoveSpeed;
    public float RotationSpeed;
    public float3 TargetPosition;
    

}
