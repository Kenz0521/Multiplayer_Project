using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettingsData", menuName = "Scriptable Objects/PlayerSettingsData")]
public class PlayerSettingsData : ScriptableObject
{
    public float moveSpeed;
    public float knockbackForce;
}
