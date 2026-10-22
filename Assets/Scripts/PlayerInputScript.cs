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
    private void Awake()
    {
        if (myRB == null) 
            myRB = GetComponent<Rigidbody>();
        if (myMeshes == null)
            myMeshes = GetComponentsInChildren<MeshRenderer>();

        playerIndex = FindObjectsByType<PlayerInputScript>(FindObjectsSortMode.None).Length;

        foreach (MeshRenderer mesh in myMeshes)
            mesh.material = materialData.materials[playerIndex - 1];
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
        myRB.AddForce(moveDirection * playerSettingsData.moveSpeed, ForceMode.Acceleration);
    }
    private void LateUpdate()
    {
        cameraFollow.localEulerAngles = new Vector3(yRotate, cameraFollow.localEulerAngles.y, cameraFollow.localEulerAngles.z);
        transform.rotation = Quaternion.Euler(0, xRotate, 0);
    }
    public void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    public int GetPlayerIndex() => playerIndex;
}
