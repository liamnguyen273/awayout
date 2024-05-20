using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperties : ScriptableObject
{
    public float maxHealth = 100;
    public float moveSpeed = 5;
    public float jumpForce = 0;
    public float rangeFindPlayer = 10;
    public float rangeRunAwayFromPlayer;
    public float rangeUseSecondWeapon;
    public float animationSpeed = 1f;
}
