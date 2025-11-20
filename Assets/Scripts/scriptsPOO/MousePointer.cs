using UnityEngine;

public class MousePointer : MonoBehaviour
{
    public static MousePointer instance {  get; private set; }

    private void Awake()
    {
        instance = this;    
    }

    public Vector3 GetMousePosition()
    {
        Ray camray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(camray, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }

        return Vector3.zero;

    }
}
