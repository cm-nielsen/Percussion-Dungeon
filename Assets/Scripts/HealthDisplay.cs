using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    public virtual void UpdateDisplay(float ratio)
    {
        //Debug.Log(gameObject.name + "'s new health: " + ratio * 100 + "%");
    }
}
