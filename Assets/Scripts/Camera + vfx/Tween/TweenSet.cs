using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenSet : MonoBehaviour
{
    public TweenList[] set = new TweenList[0];

    // Start is called before the first frame update
    void Start()
    {
        foreach (TweenList l in set)
            foreach (Tween t in l.set)
                t.Initialize(transform);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (TweenList l in set)
            l.Update();
    }

    public void Play(int i)
    {
        if(i >= set.Length)
        {   
            print("Attempted to play a tween that does not exist");
            return;
        }
        StopAll();
        set[i].Play();
    }

    public void Play(string s)
    {
        bool b = true;
        foreach (TweenList l in set)
            if (l.name == s)
            {
                StopAllEffecting(l.AllTargets());
                l.Play();
                b = false;
            }
        if(b)
            print("Attempted to play a tween that does not exist");
    }

    public void StopAll()
    {
        foreach (TweenList l in set)
            l.Stop();
    }

    public void StopAllEffecting(List<Transform> ls)
    {
        foreach (TweenList tl in set)
            tl.StopAllEffecting(ls);
    }

    [System.Serializable]
    public class TweenList
    {
        public string name;
        public Tween[] set = new Tween[0];
        public AnimationTrigger[] animationTriggers = new AnimationTrigger[0];

        public void Initialize(Transform transform)
        {
            foreach (Tween t in set) t.Initialize(transform);
        }

        public void Update() {
            foreach (Tween t in set) t.Run();
            foreach (AnimationTrigger a in animationTriggers) a.Update();
        }
        public void Play() {
            foreach (Tween t in set) t.Play();
            foreach (AnimationTrigger a in animationTriggers) a.Play();
        }
        public void Stop() {
            foreach (Tween t in set) t.Stop();
            foreach (AnimationTrigger a in animationTriggers) a.Stop();
        }

        public List<Transform> AllTargets()
        {
            List<Transform> ls = new List<Transform>();
            foreach (Tween t in set)
                ls.Add(t.target);
            return ls;
        }

        public void StopAllEffecting(List<Transform> ls)
        {
            foreach (Tween t in set)
                if (ls.Contains(t.target))
                    t.Stop();
        }

        [System.Serializable]
        public class AnimationTrigger
        {
            public Animator animator;
            public string parameterName;
            public float delay = 0;
            public bool isBool;

            private float startTime = Mathf.Infinity;

            public void Play() { startTime = Time.time; }
            public void Stop()
            {
                startTime = Mathf.Infinity;
                if (isBool)
                    animator.SetBool(parameterName, false);
                else
                    animator.ResetTrigger(parameterName);
            }

            public void Update()
            {
                if(Time.time > startTime + delay)
                {
                    if (isBool)
                        animator.SetBool(parameterName, true);
                    else
                        animator.SetTrigger(parameterName);
                }
            }
        }
    }
}
