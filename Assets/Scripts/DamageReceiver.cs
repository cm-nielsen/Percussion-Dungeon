using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { light, heavy};

public abstract class DamageReceiver : MonoBehaviour
{
    public abstract void TakeDamage(DamageType dtype, float amount, Vector2 point);
}