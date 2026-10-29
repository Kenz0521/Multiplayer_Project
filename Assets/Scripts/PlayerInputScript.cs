using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PlayerInputScript : MonoBehaviour
{
    Rigidbody myRB;
    MeshRenderer[] myMeshes;
    public Transform cameraFollow;
    Vector3 moveDirection;

    [Header("Player Settings")]
    [SerializeField] PlayerSettingsData playerSettingsData;
    [SerializeField] int playerIndex = 1;
    [SerializeField] float lookSensitivity;
    [SerializeField] float yMin, yMax;
    [SerializeField] float xRotate, yRotate;

    [Header("Multiplayer Settings")]
    [SerializeField] PlayerMaterialData materialData;

    [Header("Input Variables")]
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector2 lookInput;

    //to connect to the director
    [SerializeField] DirectorAI director;
    bool eliminated;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (myRB == null) 
            myRB = GetComponent<Rigidbody>();
        if (myMeshes == null)
            myMeshes = GetComponentsInChildren<MeshRenderer>();

        playerIndex = FindObjectsByType<PlayerInputScript>(FindObjectsSortMode.None).Length;

        foreach (MeshRenderer mesh in myMeshes)
            mesh.material = materialData.materials[playerIndex - 1];

        //register players
        if (director == null)
        {
            director = FindObjectOfType<DirectorAI>();
        }

        if (director != null)
        {
            director.AddPlayer(this);
        }
    }
    private void OnEnable()
    {
        eliminated = false;
    }
    private void Update()
    {
        xRotate += lookInput.x;
        yRotate = Mathf.Clamp(yRotate - lookInput.y * lookSensitivity, yMin, yMax);

        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
    }
    private void FixedUpdate()
    {
        moveDirection.y = 0;
        if (moveInput.sqrMagnitude > 0.001f)
        {
            Vector3 currentMoveVelocity = new Vector3(myRB.linearVelocity.x, 0f, myRB.linearVelocity.z);

            if (currentMoveVelocity.magnitude < playerSettingsData.maxVelocity)
                myRB.AddForce(moveDirection * playerSettingsData.moveSpeed, ForceMode.Acceleration);
        }
    }
    private void LateUpdate()
    {
        // Handles the camera controls
        cameraFollow.localEulerAngles = new Vector3(yRotate, cameraFollow.localEulerAngles.y, cameraFollow.localEulerAngles.z);
        transform.rotation = Quaternion.Euler(0, xRotate, 0);
    }
    public void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    public int GetPlayerIndex() => playerIndex;
    private void OnCollisionEnter(Collision other)
    {
        // Handles colliding with other players
        if (other.gameObject.tag == "Player")
        {
            //only report the collision once
            if (director != null && gameObject.GetInstanceID()<other.gameObject.GetInstanceID())
            {
                director.RegisterHit();
            }

            // Gets the direction from our position to the other players position
            Vector3 directionToOtherPlayer = (other.gameObject.transform.position - transform.position).normalized;

            // Recieves the relative velocity at the moment of impact
            Vector3 forceDirection = other.relativeVelocity;

            // Uses Dot product to get the velocity in the direction to the other Player
            float speedAlongImpactLine = Mathf.Abs(Vector3.Dot(forceDirection, directionToOtherPlayer));

            if (speedAlongImpactLine > 0.1f)
            {
                float finalForce = playerSettingsData.knockbackForce * speedAlongImpactLine;
                Debug.Log($"Speed into collision: {speedAlongImpactLine} | Final Force: {finalForce}");

                Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();

                // Adds an impulse force in the direction of the current movement multiplied by our velocity and knockbackForce
                otherRB.AddForce(directionToOtherPlayer * finalForce, ForceMode.Impulse);

                // Equal and opposite reaction to the knockback force
                myRB.AddForce(-directionToOtherPlayer * finalForce, ForceMode.Impulse);
            }
        }
    }

    //check if players eliminated
    public bool IsEliminated()
    {
        return eliminated;//return t/f
    }

    //eliminate player stop movement
    public void EliminatePlayer()
    {
        //return if already eliminated 
        if (eliminated)
        {
            return;
        }

        eliminated = true;

        //stop recieving movement 
        moveInput = Vector2.zero;
        lookInput = Vector2.zero;

        //stop car movement 
        if (myRB != null)
        {
            myRB.linearVelocity = Vector3.zero;
            myRB.angularVelocity = Vector3.zero;
        }
        this.enabled = false;
    }
    private void OnDestroy()
    {
        if (director != null)
        {
            director.RemovePlayer(this);
        }
    }
}
