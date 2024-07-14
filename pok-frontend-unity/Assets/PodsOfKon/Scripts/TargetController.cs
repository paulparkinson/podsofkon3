using UnityEngine;
using Random = UnityEngine.Random;

public class TargetController : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] targetGameObjects;
    [SerializeField] 
    private GameObject[] player1InventoryObjects;
    [SerializeField] 
    private GameObject[] player2InventoryObjects;
    

    private static TargetController singleton;
    private int randomNumber;
    private int lastRandomNumber;
    //for awake...
    private int firstObject, secondObject;

  public void Awake()
  {
      singleton = this;
      InvokeRepeating("UpdateScoreForSagasCompleted", 2.0f, 1.0f);
  }

  public void OnEnable()
  {
      //todo reset player1InventoryObjects and player2InventoryObjects to have just db and java for replays
      firstObject = Random.Range(0, player1InventoryObjects.Length);
      secondObject = Random.Range(0, player1InventoryObjects.Length);
      while (firstObject == secondObject) 
      {
          secondObject = Random.Range(0, player1InventoryObjects.Length);
      }
      int objectCount = 0;
      foreach (GameObject targetObject in player1InventoryObjects)
      {
          targetObject.SetActive(objectCount==firstObject || objectCount==secondObject);
          objectCount++;
      }
      firstObject = Random.Range(0, player2InventoryObjects.Length);
      secondObject = Random.Range(0, player2InventoryObjects.Length);
      while (firstObject == secondObject) 
      {
          secondObject = Random.Range(0, player2InventoryObjects.Length);
      }
      objectCount = 0;
      foreach (GameObject targetObject in player2InventoryObjects)
      {
          targetObject.SetActive(objectCount==firstObject || objectCount==secondObject);
          objectCount++;
      }
  }

  public void UpdateScoreForSagasCompleted()
  {
      ScoreKeeper.getInstance().UpdateScoreForSagasCompleted(
          CountOfInventory(player1InventoryObjects), CountOfInventory(player2InventoryObjects));
  }
  public void Start()
  {
      SpawnNextRandom();
  }

  public static TargetController getSingleton()
  {
      return singleton;
  }

  /**
   * Called by this.UpdateScoreForSagasCompleted which is called by Awake InvokeRepeating 
   */
  public int CountOfInventory(GameObject[] gameObjects)
  {
      int inventoryCount = 0;
      foreach (GameObject targetObject in gameObjects)
      {
          if (targetObject.activeInHierarchy) inventoryCount++;
      }
      return inventoryCount;
  }

  public void SpawnNextRandom()
  {
      while (randomNumber == lastRandomNumber) //never two of the same in a row
      {
          randomNumber = Random.Range(0, targetGameObjects.Length);
      }
      lastRandomNumber = randomNumber;
      targetGameObjects[randomNumber].gameObject.SetActive(true);
    }
  
  
}

