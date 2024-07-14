using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateAndSetObjects : MonoBehaviour
{
    public GameObject prefab;
    public GameObject gameObjectToDeactivate;
    public GameObject gameObjectToActivate;
    public GameObject gameGridQuad;
    public GameObject gameGridLeftWall;
    public GameObject gameGridRightWall;
    public Material newMaterial; 
    public Material newMaterialFinal20; 
    void Start()
    {
        GameObject prefabInstance = Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);
        CustomTimer customTimer = prefabInstance.GetComponent<CustomTimer>();
        customTimer.gameObjectToActivate = this.gameObjectToActivate;
        customTimer.gameObjectToDeactivate = this.gameObjectToDeactivate;
        customTimer.gameGridQuad = this.gameGridQuad;
        customTimer.gameGridLeftWall = this.gameGridLeftWall;
        customTimer.gameGridRightWall = this.gameGridRightWall;
    }

}
