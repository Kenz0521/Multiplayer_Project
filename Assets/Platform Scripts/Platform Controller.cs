using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] Vector3 startingSize = new Vector3(20f, 1f, 20f); //starting size of platform at begining of each game

    [SerializeField] float minimumSize = 5f;//minimum size platform will get to 
    [SerializeField] float shrinkSpeed = 0f;//speed platform shrinks at (start at 0)

    float currentSize;//store current platform size 
    float platformHeight;//store platform height (this won't shrink)
    bool shrinkingStopped;// true = update wont shrink platform 

    void Start()
    {
        currentSize = startingSize.x;//current size will be starting size at start
        platformHeight = startingSize.y;//save height so wont shrink 
        transform.localScale = startingSize;//set platform scale to start size
    }

    void Update()
    {
        //dont run if shrinking's stopped
        if (shrinkingStopped || shrinkSpeed <= 0f)
        {
            return;
        }

        currentSize -= shrinkSpeed * Time.deltaTime;//shrink based on shrink speed
        currentSize = Mathf.Max(minimumSize, currentSize);//stop from shrinking smaller than minimum size allowed
        transform.localScale = new Vector3(currentSize, platformHeight, currentSize);//set updated size to the platform
    }

    //the following will be for the director ai to call and control
    public void SetShrinkSpeed(float speed) //director will set shrink speed
    {
        shrinkSpeed = Mathf.Max(0f, speed);//not less than 0 
    }

    public void StopShrinking()//director will caal to stop shrink 
    {
        shrinkingStopped = true;//mark as stopped so update wont shrink 
        shrinkSpeed = 0f;//set shrink speed to 0 
    }

    public float GetCurrentSize()//gives director current platform size
    {
        return currentSize;
    }

    public float GetMinimumSize()//gives director minimum size allowed
    {
        return minimumSize;
    }

    //called by game reset
    public void ResetPlatform()
    {
        //reset platform and shrink settings
        currentSize = startingSize.x;
        transform.localScale = startingSize;
        shrinkSpeed = 0f;
        shrinkingStopped = false;
    }
}
