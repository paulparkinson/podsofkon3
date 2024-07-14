using UnityEngine;

public class DiscMove : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private float discSpeed;
    [SerializeField] private bool isMirror;
    [SerializeField] private bool isPlayer1;
    [SerializeField] public  GameObject oppositeSoundGameObject; // The GameObject whose position you want to be the opposite of 
    private Vector3 lastVelocity;
    [SerializeField] GameObject[] diskOnDiskCollisionSound;
    [SerializeField] GameObject diskCollisionSound;
    void Start()
    {
        if (!isMirror)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.velocity = transform.forward * discSpeed;
        }
    }

    void Update()
    {
        lastVelocity =   _rigidbody.velocity;
        if (isMirror)
        {
            Vector3 thisObjectsPosition = transform.position;
            // Invert the x and y coordinates of the target GameObject’s position
            float inverseX = -thisObjectsPosition.x;
            //    float inverseY = -targetPosition.y;
            float inverseY = thisObjectsPosition.y;
        //    Debug.Log("thisObjectsPosition.x:" + thisObjectsPosition.x );
            // Debug.Log("oppositeSoundGameObject.transform.position.y:" + oppositeSoundGameObject.transform.position.y );
            // Debug.Log("oppositeSoundGameObject.transform.position.z:" + oppositeSoundGameObject.transform.position.z );
            
            // Set the position of the current GameObject to the inverted position
           transform.position = new Vector3(thisObjectsPosition.x, oppositeSoundGameObject.transform.position.y, -oppositeSoundGameObject.transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isMirror)
        {
     //       Debug.Log("DiscMove OnCollisionEnter other:" + other );
            Debug.Log("DiscMove OnCollisionEnter other.gameObject:" + other.gameObject );
     //       Debug.Log("DiscMove OnCollisionEnter other.gameObject.GetComponent<CollisionAction>(:" + other.gameObject.GetComponent<CollisionAction>());
            var speed = lastVelocity.magnitude;
            var direction = Vector3.Reflect(lastVelocity.normalized, other.contacts[0].normal);
            _rigidbody.velocity = direction * Mathf.Max(speed, 0f);
            _rigidbody.velocity = -transform.forward * discSpeed;
            //todo 1. cache this 2. make selection random
        //    Debug.Log("DiscMove OnCollisionEnter other.gameObject.GetComponent<DiscMove>():" + other.gameObject.GetComponent<DiscMove>());
       //     Debug.Log("DiscMove OnCollisionEnter diskOnDiskCollisionSound.Length > 0:" + diskOnDiskCollisionSound.Length );
            if(other.gameObject.GetComponent<DiscMove>()!=null) {
                if (diskOnDiskCollisionSound.Length > 0) {
                    diskOnDiskCollisionSound[0].GetComponent<AudioSource>().Play();
                    other.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                }
            }
            else
            {
                string actoindoneMessage =
                    other.gameObject.GetComponent<CollisionAction>().DoAction(gameObject, isPlayer1);
                Debug.Log("actoindoneMessage:" + actoindoneMessage);
            }

            if (!other.gameObject.CompareTag("explosion"))
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("!other.gameObject.CompareTag explosion :" + !other.gameObject.CompareTag("explosion"));
            }
        }
    }
}
