using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Transform[] cameraPositions;
    public Camera camera1;

    private int currentIndex = 0;

    void Start()
    {
        if (camera1 == null)
        {
            camera1 = Camera.main;
        }

        if (cameraPositions.Length > 0)
        {
            MoveCamera(0);
        }
        else
        {
            Debug.LogWarning("array vacio");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            NextView();
        }
    }

    void NextView()
    {
        if (cameraPositions.Length == 0) return;

        currentIndex++;

        if (currentIndex >= cameraPositions.Length)
        {
            currentIndex = 0;
        }

        MoveCamera(currentIndex);
    }

    void MoveCamera(int index)
    {
        camera1.transform.position = cameraPositions[index].position;
        camera1.transform.rotation = cameraPositions[index].rotation;
    }
}
