using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
partial struct MoveSystem : ISystem
{
   
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        MoveUnitJobs moveUnitJobs = new MoveUnitJobs
        {
            deltatime = SystemAPI.Time.DeltaTime,
        };
        moveUnitJobs.ScheduleParallel();



        /*
         foreach((RefRW<LocalTransform> localTransform,
                 RefRO<MoveUnitComponent> moveUnit,
                 RefRW<PhysicsVelocity> physicsVelocity)
                    in SystemAPI.Query<
                        RefRW<LocalTransform>,
                        RefRO<MoveUnitComponent>,
                        RefRW<PhysicsVelocity>>())
        {

            
            float3 moveDirection = moveUnit.ValueRO.TargetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRW.Rotation, 
                quaternion.LookRotation(moveDirection, math.up()), SystemAPI.Time.DeltaTime * moveUnit.ValueRO.RotationSpeed);



            physicsVelocity.ValueRW.Linear = moveDirection * moveUnit.ValueRO.MoveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
        
          */


    }

}

[BurstCompile]
public partial struct MoveUnitJobs : IJobEntity
{

    public float deltatime;
    private void Execute( ref LocalTransform localTransform, in MoveUnitComponent moveUnit, ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = moveUnit.TargetPosition - localTransform.Position;

        float reachedTargetDistanceSq = 2f;
        if (math.lengthsq(moveDirection)< reachedTargetDistanceSq)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
        moveDirection = math.normalize(moveDirection);

        localTransform.Rotation = math.slerp(localTransform.Rotation,
            quaternion.LookRotation(moveDirection, math.up()), deltatime * moveUnit.RotationSpeed);

        physicsVelocity.Linear = moveDirection * moveUnit.MoveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}
