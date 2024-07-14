using UnityEngine;
using Rewired;
using UnityEngine.Serialization;

public class MyCharacter : MonoBehaviour
{
    [FormerlySerializedAs("directionsScene1")]
    public GameObject directionsScene1;

    [FormerlySerializedAs("directionsScene2")]
    public GameObject directionsScene2;

    [FormerlySerializedAs("architectureScene")]
    public GameObject architectureScene;

    [FormerlySerializedAs("architectureScene")]
    public GameObject thanksScene;

    [FormerlySerializedAs("architectureScene")]
    public GameObject[] architectureAudioOptions;

    private int currentArchitectureSceneAudio;
    private Animator animator;
    public GameObject gamePlayScene;
    public GameObject bonusRoundScene;
    public GameObject firebuttonDuringDirectionsSound;
    public GameObject firebuttonDuringGameAndBonusSound;
    static private bool isPlayerOneFireButtonHit;
    static private bool isPlayerTwoFireButtonHit;

    static public int screenNumber;

    // 0 = arch , 1 = directions1 , 2 = directions2, 3 = game, 4 = bonus, 5 = thanks
    public int playerId;
    private static int playerConfirmCount;
    public string sceneName;
    public float moveSpeed = 2.0f;
    public float bulletSpeed = 15.0f;
    public GameObject discGameObject;
    public Transform discSpawnPosition;
    public Transform discSpawnPositionLeft;
    public Transform discSpawnPositionRight;
    private Player player;
    private Vector3 moveVector;
    private bool fire;
    private bool isKeyboardMove;
    private float moveVectorX;

    void OnEnable()
    {
    //    Debug.Log("~~~OnEnable  sceneName:" + sceneName + "playerId:" + playerId);
        player = ReInput.players.GetPlayer(playerId);
        if (sceneName == "directions1")
        {
            ScoreKeeper.getInstance().SetPlayerNames();
        }
        if (sceneName == "thanks")
        {
            ScoreKeeper.getInstance().Reset();
        }
        if (sceneName == "architecture")
        {
            gamePlayScene.SetActive(false);
            ScoreKeeper.getInstance().ResetScore();
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
   //     Debug.Log("~~~OnEnable  sceneName == \"architecture\":" + (sceneName == "architecture"));
   if (sceneName == "architecture")
   {
       InvokeRepeating("PerformTask", 0f, 4f);
   }
  //      Debug.Log("animator:" + animator);
    }
    

    void PerformTask()
    {
        ScoreKeeper.getInstance().SetPlayerNames();
    }

    void Update()
    {
        GetInput();
        ProcessInput();
    }

    private void GetInput()
    {
        // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
        // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.
        moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
        moveVector.y = player.GetAxis("Move Vertical");
        fire = player.GetButtonDown("Fire");
        isKeyboardMove = Input.GetKeyDown(KeyCode.A) 
                         || Input.GetKeyDown(KeyCode.D) 
                         || Input.GetKeyDown(KeyCode.LeftArrow) 
                         || Input.GetKeyDown(KeyCode.RightArrow);
        // if (!fire && Input.GetKeyDown("a"))
    }

    private void ProcessInput()
    {
        if ((screenNumber == 3 || screenNumber == 4) && (moveVector.x != 0.0f || isKeyboardMove))
        {
            if (playerId == 1)
            {
                if (moveVector.x > 0.0f)
                {
              //      Debug.Log("moveVector.x:" + moveVector.x);
                    if (transform.position.x > 5) return;
                    animator.Play("Left");
                    moveVectorX = transform.position.x + ((moveVector.x / 100) * moveSpeed);
                }
                else if (moveVector.x < 0.0f)
                {
         //           Debug.Log("moveVector.x:" + moveVector.x);
                    if (transform.position.x < -5) return;
                    animator.Play("Right");
                    moveVectorX = transform.position.x + ((moveVector.x / 100) * moveSpeed);
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A))
                {
          //          Debug.Log("A key transform.position.x:" + transform.position.x);
                    if (transform.position.x < -4) return;
                    animator.Play("Left");
                    moveVectorX = transform.position.x + ((-15.0f / 100) * moveSpeed);
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.D))
                {
         //            Debug.Log("D key transform.position.x:" + transform.position.x);
                    if (transform.position.x > 4) return;
                    animator.Play("Right");
                    moveVectorX = transform.position.x + ((15.0f / 100) * moveSpeed);
                }

            }
            else if (playerId == 0)
            {
                if (moveVector.x < 0.0f)
                {
                    if (transform.position.x > 4.1) return;
                    animator.Play("Left");
                    moveVectorX = transform.position.x - ((moveVector.x / 100) * moveSpeed);
                }
                else if (moveVector.x > 0.0f)
                {
                    
                    if (transform.position.x < -4.1) return;
                    animator.Play("Right");
                    moveVectorX = transform.position.x - ((moveVector.x / 100) * moveSpeed);
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    if (transform.position.x > 4.1) return;
                    animator.Play("Left");
                    moveVectorX = transform.position.x - ((-13.0f  / 100) * moveSpeed);
                }
                else if (Input.GetKey(KeyCode.RightArrow) )
                {
                    if (transform.position.x < -4.1) return;
                    animator.Play("Right");
                    moveVectorX = transform.position.x - ((13.0f  / 100) * moveSpeed);
                }
            }
            transform.position = new Vector3(moveVectorX, transform.position.y, transform.position.z);
        }
        if (fire || (playerId == 1 && Input.GetKeyDown(KeyCode.Tab)) || (playerId == 0 && Input.GetKeyDown(KeyCode.Return)))
        {
            if (screenNumber == 3 || screenNumber == 4)
            {
                if (animator != null)
                {
                    animator.Play("Throw");
                }
                firebuttonDuringGameAndBonusSound.GetComponent<AudioSource>().Play();
                if (moveVector.y != 0.0f)
                {
                    GameObject disc = Instantiate(discGameObject, discSpawnPositionLeft.position,
                        discSpawnPositionLeft.rotation);
                    disc.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
                }
                else
                {
                    GameObject disc = Instantiate(discGameObject, discSpawnPosition.position,
                        discSpawnPosition.rotation);
                    disc.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
                }
            }
            else if (screenNumber == 0 || screenNumber == 1 || screenNumber == 2 || screenNumber == 5)
            {
                firebuttonDuringDirectionsSound.GetComponent<AudioSource>().Play();
                if (playerId == 0) isPlayerTwoFireButtonHit = true;
                else if (playerId == 1) isPlayerOneFireButtonHit = true;
                if (isPlayerTwoFireButtonHit && isPlayerOneFireButtonHit)
                {
                    if (screenNumber == 0) //arch
                    {
                        architectureScene.SetActive(false);
                        directionsScene1.SetActive(true);
                        screenNumber = 1;
                    }
                    else if (screenNumber == 1) //directions 1
                    {
                        directionsScene1.SetActive(false);
                        directionsScene2.SetActive(true);
                        screenNumber = 2;
                    }
                    else if (screenNumber == 2) //directions 2
                    {
                        directionsScene2.SetActive(false);
                        gamePlayScene.SetActive(true);
                        screenNumber = 3;
                    }
                    else if (screenNumber == 5) //thanks
                    {
                        thanksScene.SetActive(false);
                        architectureScene.SetActive(true);
                        screenNumber = 0;
                    }
                    isPlayerOneFireButtonHit = false;
                    isPlayerTwoFireButtonHit = false;
                }
            }
        }
        if (screenNumber == 0 && (moveVector.y != 0.0f))
        {
            if (moveVector.y < 0.0f) //up
            {
                Debug.Log("setting player names moveVector.y:" + moveVector.y);
                ScoreKeeper.getInstance().SetPlayerNames();
            }
            else //down
            {
                Debug.Log("changing archscene audio currentArchitectureSceneAudio:" + currentArchitectureSceneAudio);
                gameObject.GetComponent<AudioSource>().Stop();
                architectureAudioOptions[currentArchitectureSceneAudio].GetComponent<AudioSource>().Stop();
                if (currentArchitectureSceneAudio++ > architectureAudioOptions.Length - 1)
                    currentArchitectureSceneAudio = 0;
                architectureAudioOptions[currentArchitectureSceneAudio].GetComponent<AudioSource>().Play();
            }
        }
    }
}