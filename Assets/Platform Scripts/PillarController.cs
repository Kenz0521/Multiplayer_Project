using UnityEngine;

public class PillarController : MonoBehaviour
{
    [SerializeField] PlatformController platformController; //connect platform 
    [SerializeField] float fallDelayTime = 0.5f;//delay before falling off edge 
    [SerializeField] float destroyHeight = -10f;//destroy pillar past neg height 
    Rigidbody rb;//pillar 
    float pillarRadius;//pillar radius for platform check 
    bool isFalling;//check if pillar is falling 
    float outsidePlatformTimer;//timer for pillar out of platform 

    void Start()
    {
        rb = GetComponent<Rigidbody>();//get rb

        rb.useGravity = false;//dont fall automatically 
        rb.isKinematic = true;//make kinematic so other things dont move ti 

        Collider pillarCollider = GetComponent<Collider>();//get collider to get size

        //caclulate radius only if colliders found 
        if (pillarCollider != null)
        {
            pillarRadius = Mathf.Max(pillarCollider.bounds.extents.x, pillarCollider.bounds.extents.z);
        }
    }

    void Update()
    {
        //dont check if the platform ref is missing.
        if (platformController == null)
        {
            return;
        }

        //check if pillar is/has fallen 
        if (isFalling)
        {
            //destroy if pillar falls below set height 
            if (transform.position.y < destroyHeight)
            {
                Destroy(gameObject);
            }

            return;
        }

        float platformSize = platformController.GetCurrentSize();//get platforms current  size
        float platformHalfSize = platformSize / 2f;//half of its size for center 

        Vector3 relativePosition = transform.position - platformController.transform.position;//calculate pillars position based on center 
        bool outsidePlatform = Mathf.Abs(relativePosition.x) + pillarRadius > platformHalfSize || Mathf.Abs(relativePosition.z) + pillarRadius > platformHalfSize;//check if pillar is out of the platform 

        //if it is out of platform count how long pillar has been out the platform
        if (outsidePlatform)
        {
            outsidePlatformTimer += Time.deltaTime;

            // begin falling after the delay time
            if (outsidePlatformTimer >= fallDelayTime)
            {
                StartFalling();
            }
        }

        else
        {
            //reset timer if the pillar is still supported 
            outsidePlatformTimer = 0f;
        }
    }

    void StartFalling()
    {
        // avoid the pillar from falling if already doing so 
        if (isFalling)
        {
            return;
        }

        isFalling = true;//mark that its falling 

        rb.isKinematic = false;//no longer kinematic so other things can move it
        rb.useGravity = true;//fall down with gravity
    }
}
