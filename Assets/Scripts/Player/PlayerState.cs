using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsDashing { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public void StartDash() { IsDashing = true; }
    public void StopDash() { IsDashing = false; }

    public void StartInvulnerability() { IsInvulnerable = true; }
    public void StopInvulnerability() { IsInvulnerable = false; }
}
