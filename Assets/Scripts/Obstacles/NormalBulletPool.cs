using System.Collections.Generic;
using UnityEngine;

public sealed class NormalBulletPool : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int initialCapacity = 24;

    [SerializeField, Min(0.01f)]
    private float bulletRadius = 0.16f;

    private readonly Queue<NormalBullet> available = new();
    private Sprite bulletSprite;

    private void Awake()
    {
        bulletSprite = CreateCircleSprite();

        for (int i = 0; i < initialCapacity; i++)
            available.Enqueue(CreateBullet());
    }

    public NormalBullet Get()
    {
        NormalBullet bullet =
            available.Count > 0
                ? available.Dequeue()
                : CreateBullet();

        return bullet;
    }

    public void Return(NormalBullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(transform, false);
        available.Enqueue(bullet);
    }

    private NormalBullet CreateBullet()
    {
        GameObject bulletObject = new("NormalBullet");
        bulletObject.transform.SetParent(transform, false);
        bulletObject.transform.localScale =
            Vector3.one * bulletRadius * 2f;

        SpriteRenderer renderer =
            bulletObject.AddComponent<SpriteRenderer>();

        renderer.sprite = bulletSprite;
        renderer.color = new Color(1f, 0.18f, 0.55f, 1f);
        renderer.sortingOrder = 10;

        CircleCollider2D collider =
            bulletObject.AddComponent<CircleCollider2D>();

        collider.isTrigger = true;
        collider.radius = 0.5f;

        Rigidbody2D body =
            bulletObject.AddComponent<Rigidbody2D>();

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;
        body.simulated = true;

        NormalBullet bullet =
            bulletObject.AddComponent<NormalBullet>();

        bulletObject.SetActive(false);

        return bullet;
    }

    private static Sprite CreateCircleSprite()
    {
        const int size = 64;

        Texture2D texture = new(size, size, TextureFormat.RGBA32, false)
        {
            name = "RuntimeBulletCircle",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color clear = Color.clear;
        Color white = Color.white;

        float center = (size - 1) * 0.5f;
        float radius = center - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(center, center)
                );

                texture.SetPixel(
                    x,
                    y,
                    distance <= radius ? white : clear
                );
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, size, size),
            new Vector2(0.5f, 0.5f),
            size
        );
    }
}
