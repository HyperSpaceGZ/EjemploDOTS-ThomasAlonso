using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;


public class EntitySelector : MonoBehaviour
{

    public static EntitySelector Instance { get; private set; }        

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    private Vector2 selectionStartMousePosition;

    private void Awake()
    {
        Instance = this;
    }

   
    private void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;
            
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SelectedUnit>().Build(entityManager);

            //deseleccionar las unidades
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<SelectedUnit>(entityArray[i], false);
            }

            //definicion del maximo y minimo de un area de deteccion para tomar solamente 1 unidad
            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSelectionSIzeMin = 40f;
            bool isMultipleSelection = selectionAreaSize > multipleSelectionSIzeMin;

            if (isMultipleSelection)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<SelectedUnit>().Build(entityManager);

                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localtransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                for (int i = 0; i < localtransformArray.Length; i++)
                {
                    LocalTransform unitlocalTransform = localtransformArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitlocalTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        //aca va si la unidad desta dentro
                        entityManager.SetComponentEnabled<SelectedUnit>(entityArray[i], true);
                    }

                }
            }
            else
            {
                // seleccionar solo 1
                
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>(); 
                CollisionWorld collisionWolrd = physicsWorldSingleton.CollisionWorld;

                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                int unitsLayer = 6;
                RaycastInput raycastInput = new RaycastInput {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter {
                        //como no se puede convertir un int en unidad se le pone la "u" al 1 para hacer bien el bitmask
                        BelongsTo = ~0u,
                           CollidesWith = 1u << unitsLayer,
                                GroupIndex = 0,
                    }
                };
                if(collisionWolrd.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
                {
                    if (entityManager.HasComponent<Unit>(raycastHit.Entity))
                    {
                        // selecciona la unidad
                        entityManager.SetComponentEnabled<SelectedUnit>(raycastHit.Entity, true);
                    }
                }  

                
                
                
            }

            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonDown(1))
        {

            Vector3 mouseWorldPosition = MousePointer.instance.GetMousePosition();
            EntityManager entityManager   = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<MoveUnitComponent, SelectedUnit>().Build(entityManager);

            //aca le damos la orden para moverse
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<MoveUnitComponent> unityMoveArray = entityQuery.ToComponentDataArray<MoveUnitComponent>(Allocator.Temp);
            // metemos el generador de posiciones en anillo
            NativeArray<float3> movePositionArray  =  GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);
            for (int i = 0; i < unityMoveArray.Length;  i++)
            {
                MoveUnitComponent unitMove = unityMoveArray[i];
                //aca remplazamos el mouseworlposition por los anillos 
                unitMove.TargetPosition = movePositionArray[i];
                entityManager.SetComponentData(entityArray[i], unitMove);

            }
            


        }
    }

    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(
            Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));

        Vector2 upperRightCorner = new Vector2(
            Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));

        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y
            );
    }
    //creamos una native array para generar las posiciones de nuestro circulo de formacion y lo ponemos mas tarde con el targetposition
    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
    {
        //definimos como lo va a formar una vez que demos click, generara anillos sobre nuestro targetpoition para generar posiciones que las unidades seleccionadas puedan usar 
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
        if(positionCount == 0)
        {
            return positionArray;
        }  
        positionArray[0] = targetPosition;
        if(positionCount == 1)
        {
            return positionArray;
        }
        //se definen las dimensiones de nuestro anillo 
        float ringSize = 2.2f;
        int ring = 0;
        int positionIndex = 1;

        while(positionIndex < positionCount)
        {
            //se define un minimo el cual multiplicara por 2 las siguientes numeros de posiciones que pueden existir dentro del anillo
            int ringPositionCount = 3 + ring * 2; 

            for(int i = 0; i < ringPositionCount; i++)
            {
                //generado las grillas
                float angle = i * ((math.PI2) / ringPositionCount);
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                float3 ringPostion = targetPosition + ringVector;
                // se le adiere la poscion a cada uno
                positionArray[positionIndex] = ringPostion;
                positionIndex++;

                if(positionIndex >= positionCount)
                {
                    break;
                }

            }
            ring++;
        }
        return positionArray;
    }

}
