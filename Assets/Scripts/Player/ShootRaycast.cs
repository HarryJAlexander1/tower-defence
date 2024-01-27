using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRaycast : MonoBehaviour
{
    GameObject GameManagerObject;
    GameManager GameManager;
    private float RaycastDistance;
    private List<Graph.Vertex> Vertices;
    // Start is called before the first frame update
    void Awake()
    {
        RaycastDistance = 20f;
        GameManagerObject = GameObject.Find("Game Manager");
        GameManager = GameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            FireRaycast();
        }
    }

    private void FireRaycast() 
    {
        Vector3 raycastOrigin = transform.position;
        Vector3 raycastDirection = transform.forward;

        // Create a Ray from the origin and direction
        Ray ray = new Ray(raycastOrigin, raycastDirection);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, RaycastDistance))
        {
            Debug.Log("Hit: " + hitInfo.point);
            GameManager.PlaceBlockOnNearestEmptyVertex(hitInfo.point);
        }
        else
        {
            // If the ray doesn't hit anything, you can handle that case here
            Debug.Log("No hit");
        }
    }
}
