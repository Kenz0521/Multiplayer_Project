using UnityEngine;

public class PlayerFallDetect : MonoBehaviour
{
    [SerializeField] float fallHeight = -5f;//min height before elimination 
    PlayerInputScript playerInputScript;

    private void Awake()
    {
        playerInputScript = GetComponent<PlayerInputScript>();//get PlayerInputScript
    }

    // Update is called once per frame
    void Update()
    {
        //check if player is lowere than allowed height
        if (transform.position.y < fallHeight)
        {
            //make sure player hasnt been eliminated yet
            if (playerInputScript != null && !playerInputScript.IsEliminated())
            {
                playerInputScript.EliminatePlayer();//eliminate player
            }
        }
    }
}
