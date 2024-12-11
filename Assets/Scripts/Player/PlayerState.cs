using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static bool IsDashing { get; private set; }
    public static bool IsInvulnerable { get; private set; }

    public static void StartDash() { IsDashing = true; }
    public static void StopDash() { IsDashing = false; }

    public static void StartInvulnerability() { IsInvulnerable = true; }
    public static void StopInvulnerability() { IsInvulnerable = false; }
}
