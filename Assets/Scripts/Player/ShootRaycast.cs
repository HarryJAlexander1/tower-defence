using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ShootRaycast : MonoBehaviour
{
    GameObject GameManagerObject;
    GameManager GameManager;
    private float RaycastDistance;
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
            FireRaycast(false);
        }
        else if (Input.GetMouseButtonDown(1)) 
        {
            FireRaycast(true);
        }
    }

    private void FireRaycast(bool rightClickPressed) 
    {
        Vector3 raycastOrigin = transform.position;
        Vector3 raycastDirection = transform.forward;

        // Create a Ray from the origin and direction
        Ray ray = new Ray(raycastOrigin, raycastDirection);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, RaycastDistance))
        {
            Debug.Log("Hit: " + hitInfo.point);
            if (!GameManager.IsAttackSequence && !GameManager.AgentExists) 
            {
                ManageBlocks(hitInfo, rightClickPressed);
            }     
        }
        else
        {
            // If the ray doesn't hit anything, you can handle that case here
            Debug.Log("No hit");
        }
    }
    private void ManageBlocks(RaycastHit hitInfo, bool removeBlock) 
    {
        if (removeBlock)
        {
            if (hitInfo.collider.gameObject.CompareTag("Block"))
            {
                GameManager.RemoveBlock(hitInfo.point, hitInfo.collider.gameObject);
            }
        }
        else
        {
            if (hitInfo.collider.gameObject.CompareTag("Floor"))
            {
                GameManager.PlaceBlockOnNearestEmptyVertex(hitInfo.point);
            }
        }
    }
}
