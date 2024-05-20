using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProperties : ScriptableObject
{
    public int magazineSize = 30;
    public float reloadTime = 2f;
    public int ammoConsumedPerShot = 1;
    public float damageCaused = 10;
    public float shootDistance = 10f;
    public float blockOnUseDuration = 0;
}
