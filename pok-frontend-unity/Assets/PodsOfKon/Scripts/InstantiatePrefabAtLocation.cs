using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefabAtLocation : MonoBehaviour
{
    public GameObject collisionActionWithPhysics; 
    public GameObject prefab; 
    public GameObject referenceObject;
    public Transform parentTransform;
    public float zadjustment = 0.001f;
    // public Vector3 instantiatePosition; 
    // public Quaternion instantiateRotation = Quaternion.identity; 
    void OnEnable()
    {
        collisionActionWithPhysics.SetActive(true);
    }
    void OnEnable0()
    {
        
        if (prefab != null && referenceObject != null)
        {
            Vector3 adjustedPosition = referenceObject.transform.position;
            adjustedPosition.z += zadjustment;
            GameObject instance = Instantiate(prefab, adjustedPosition, referenceObject.transform.rotation);
            instance.tag = "explosion";
            instance.transform.localScale = referenceObject.transform.localScale;
            if (parentTransform != null)
            {
                instance.transform.SetParent(parentTransform, true); // true to maintain world position
            }
        }
    }

}
