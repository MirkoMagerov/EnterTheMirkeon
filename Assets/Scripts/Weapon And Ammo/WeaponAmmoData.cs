using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmoData
{
    public int currentMag;
    public int totalBullets;

    public WeaponAmmoData(int currentMag, int totalBullets)
    {
        this.currentMag = currentMag;
        this.totalBullets = totalBullets;
    }
}
