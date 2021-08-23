using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween : MonoBehaviour
{
    public PositionTween position;
    public SizeTween size;
    public RotationTween rotation;
    public bool playOnAwake;

    private Vector2 startPos;
    private bool playing = false;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        transform.localPosition = startPos + position.start;
        if (playOnAwake)
            Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            Lerp(Time.time - startTime);
        }
    }

    public void Play()
    {
        playing = true;
        startTime = Time.time;
    }

    public void Lerp(float f)
    {
        transform.localPosition = startPos + position.Lerp(f);
        transform.localScale = size.Lerp(f);
        transform.rotation = Quaternion.Euler(Vector3.forward * rotation.Lerp(f));
    }

    [System.Serializable]
    public class PositionTween: TweenBase<Vector2>
    {
        PositionTween()
        {
            start = Vector2.zero;
            end = Vector2.zero;
        }
        new public Vector2 Lerp(float f)
        {
            return Vector2.Lerp(start, end, curve.Evaluate(f));
        }
    }

    [System.Serializable]
    public class SizeTween : TweenBase<Vector2>
    {
        SizeTween()
        {
            start = Vector2.one;
            end = Vector2.one;
        }
        new public Vector2 Lerp(float f)
        {
            return Vector2.Lerp(start, end, curve.Evaluate(f));
        }
    }

    [System.Serializable]
    public class RotationTween : TweenBase<float>
    {
        new public float Lerp(float f)
        {
            return Mathf.Lerp(start, end, curve.Evaluate(f));
        }
    }
}

public class TweenBase<T>
{
    public T start, end;
    public AnimationCurve curve;

    public virtual T Lerp(float f) { return start; }
    public virtual void Apply(Transform t) { }
}
