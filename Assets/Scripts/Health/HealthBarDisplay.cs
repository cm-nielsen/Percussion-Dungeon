using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthBarDisplay : HealthDisplay
{
    public SpriteRenderer backFill;
    public int backFillHoldFrames = 0;
    public float lerp = 0.05f;
    public bool canDrain = false;

    private SpriteRenderer fill;
    private SegmentedHealthBarDisplay segBar;
    private BoxParticles particles;

    private float maxWidth, rat = 1, backRat = 1;
    private int timer = 0, index;
    // Start is called before the first frame update
    void Start()
    {
        fill = GetComponent<SpriteRenderer>();
        fill.drawMode = SpriteDrawMode.Tiled;
        maxWidth = fill.size.x;

        backFill.drawMode = SpriteDrawMode.Tiled;
        particles = new BoxParticles(gameObject);
    }

    private void FixedUpdate()
    {
        if (backRat > rat)
        {
            if (canDrain)
            {
                if (timer > backFillHoldFrames)
                {
                    backRat = Mathf.Lerp(backRat, rat, lerp);
                    backFill.size = new Vector2(backRat * maxWidth, backFill.size.y);
                }

                if (backRat <= rat + .01)
                {
                    backRat = rat;
                    backFill.size = new Vector2(rat * maxWidth, fill.size.y);
                    timer = 0;
                    if (rat == 0 && segBar)
                    {
                        segBar.NotifyDrain(index);
                        canDrain = false;
                    }
                }
            }
            timer++;
        }
    }

    public override void UpdateDisplay(float ratio)
    {
        float diff = rat - ratio;
        rat = ratio;
        if (rat > backRat)
        {
            backRat = rat;
            backFill.size = new Vector2(backRat * maxWidth, backFill.size.y);
            timer = 0;

            segBar.NotifyFill(index);
        }
        if (fill)
        {
            if (diff > 0)
            {
                float width = diff * maxWidth;
                particles.Trigger(new Vector2(width / 2 + rat * maxWidth,
                    -fill.sprite.pivot.y / 32 + fill.size.y / 2),
                    new Vector2(width, fill.size.y), diff);
            }
            fill.size = new Vector2(ratio * maxWidth, fill.size.y);
        }
    }

    public void Initialize(SegmentedHealthBarDisplay seg,  int n) { segBar = seg; index = n; }
}

public class BoxParticles
{
    private ParticleSystem pSystem;
    private float particles = 80;

    public BoxParticles(GameObject g)
    {
        pSystem = g.GetComponentInChildren<ParticleSystem>();
        if (pSystem)
        {
            ParticleSystem.EmissionModule em = pSystem.emission;
            particles = em.GetBurst(0).maxCount;
        }
    }

    public void Trigger(Vector2 pos, Vector2 size, float rat = 1)
    {
        if (!pSystem)
            return;

        ParticleSystem.ShapeModule shape = pSystem.shape;
        shape.position = pos;
        shape.scale = size;

        ParticleSystem.Burst burst = pSystem.emission.GetBurst(0);
        burst.count = Mathf.Pow(particles * rat, 4);

        pSystem.Emit((int)(particles * rat));
    }
}
