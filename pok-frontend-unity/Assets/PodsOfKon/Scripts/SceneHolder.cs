using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHolder : MonoBehaviour
{
    public GameObject sceneObjectToInstantiate;
    public GameObject locationToPutScene;
    private GameObject sceneInstance;
    void Start()
    {
        sceneInstance = Instantiate(sceneObjectToInstantiate,
            new Vector3(locationToPutScene.transform.position.y, locationToPutScene.transform.position.y, 0),
            Quaternion.identity);
    }

    // Update is called once per frame
    public void DestroyScene()
    {       
        Destroy(sceneInstance);
    }
}
