using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenPlayer : MonoBehaviour
{
    public TweenSet tweenSet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTweenByIndex(int i) { if (tweenSet) tweenSet.Play(i); }
    public void PlayTween(string s){ if (tweenSet) tweenSet.Play(s); }
}
