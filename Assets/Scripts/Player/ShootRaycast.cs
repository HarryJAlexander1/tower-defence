using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            if (!GameManager.IsAttackSequence && !GameManager.AgentExists)
            {
                ManageBlocks(hitInfo, rightClickPressed);
            }
            else 
            {
                ManageHitOnEnemy(hitInfo);
            }
        }
        else
        {
            Debug.Log("No hit");
        }
    }

    private void ManageHitOnEnemy(RaycastHit hitInfo) 
    {
        if (hitInfo.collider.gameObject.CompareTag("AgentBody")) 
        {
            AgentBehaviour agentBehaviour = hitInfo.collider.gameObject.GetComponentInParent<AgentBehaviour>();
            agentBehaviour.Hitpoints--;
            Debug.Log(agentBehaviour.Hitpoints);
        }
    }
    private void ManageBlocks(RaycastHit hitInfo, bool removeBlock) 
    {
        if (removeBlock)
        {
            if (hitInfo.collider.gameObject.CompareTag("Block"))
            {
                GameManager.RemoveBlock(hitInfo.point, hitInfo.collider.gameObject);
                GameManager.Fund += 5;
            }
        }
        else
        {
            if (hitInfo.collider.gameObject.CompareTag("Floor") && GameManager.Fund >= 10)
            {
                GameManager.PlaceBlockOnNearestEmptyVertex(hitInfo.point);
                GameManager.Fund -= 10;
            }
        }
    }
}
