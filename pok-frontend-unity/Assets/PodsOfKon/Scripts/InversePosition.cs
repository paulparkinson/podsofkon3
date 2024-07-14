using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InversePosition : MonoBehaviour
{
    public GameObject targetGameObject; // The GameObject whose position you want to inverse
   // public Transform target; // The GameObject whose position you want to inverse

    void Update()
    {
        if (targetGameObject != null)
        {
            // Get the position of the target GameObject
            Vector3 targetPosition = targetGameObject.transform.position;
            
            // Invert the x and y coordinates of the target GameObject’s position
            float inverseX = -targetPosition.x;
        //    float inverseY = -targetPosition.y;
            float inverseY = targetPosition.y;
            
            // Set the position of the current GameObject to the inverted position
            transform.position = new Vector3(inverseX, inverseY, targetPosition.z);
        }
    }
}

