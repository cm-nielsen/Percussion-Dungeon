using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceBarDisplay : MonoBehaviour
{
    public SpriteRenderer fill, frontFill;
    public float backFillHoldTime;
    public int collectParticles, levelParticles;

    private ParticleSystem pSystem;
    private float maxWidth, rat, frontRat, timer = 0;
    private int level = 0, pCount;
    // Start is called before the first frame update
    void Start()
    {
        frontFill.drawMode = SpriteDrawMode.Tiled;
        fill.drawMode = SpriteDrawMode.Tiled;
        maxWidth = fill.size.x;

        frontRat = rat = 0;
        frontFill.size = new Vector2(0, frontFill.size.y);
        fill.size = new Vector2(0, fill.size.y);
        GameController.GainExp(0);
        pSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(frontRat < rat)
        {
            if (timer > backFillHoldTime)
            {
                frontRat = Mathf.Lerp(frontRat, rat, 0.5f) + 0.001f;
                frontFill.size = new Vector2(frontRat * maxWidth, frontFill.size.y);
            }

            if (frontRat >= rat)
            {
                frontRat = rat;
                frontFill.size = new Vector2(rat * maxWidth, frontFill.size.y);
                timer = 0;
            }
            timer += Time.deltaTime;
        }
    }

    public void UpdateDisplay(float ratio, int level)
    {
        rat = ratio;
        if (rat < frontRat)
        {
            frontRat = 0;
            frontFill.size = new Vector2(frontRat * maxWidth, frontFill.size.y);
            timer = 0;
        }
        fill.size = new Vector2(ratio * maxWidth, fill.size.y);

        pSystem?.Emit(collectParticles);
        if (level != this.level)
        {
            this.level = level;
            pSystem?.Emit(levelParticles);
        }
    }
}
