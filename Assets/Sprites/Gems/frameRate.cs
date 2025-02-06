using UnityEngine;

public class SpriteAnimationController : MonoBehaviour
{
    public Sprite[] sprites;
    public float frameRate = 0.25f;

    private float timer = 0f;
    private int currentSpriteIndex = 0;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on GameObject.");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer = 0f;
            currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[currentSpriteIndex];
        }
    }
}
