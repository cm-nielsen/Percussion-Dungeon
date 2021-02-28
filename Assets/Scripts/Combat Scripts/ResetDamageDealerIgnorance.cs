using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDamageDealerIgnorance : MonoBehaviour
{
    private DamageDealer dealer;
    // Start is called before the first frame update
    void Start()
    {
        dealer = GetComponentInChildren<DamageDealer>();
    }

    private void StartSwing()
    {
        dealer.StartSwing();
    }
}
