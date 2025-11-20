using Unity.Entities;
using UnityEngine;

public class SelectionAuthoring : MonoBehaviour
{
    public GameObject VisualEffect;
    public float DepthScale;
    public class Baker : Baker<SelectionAuthoring>
    {
        public override void Bake(SelectionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SelectedUnit
            {
                VisualEffect = GetEntity(authoring.VisualEffect, TransformUsageFlags.Dynamic),
                DepthScale = authoring.DepthScale,
            }

            );
            SetComponentEnabled<SelectedUnit>(entity, false);

        }
    }

}

public struct SelectedUnit : IComponentData, IEnableableComponent
{
    public Entity VisualEffect;
    public float DepthScale;


}


