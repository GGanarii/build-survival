using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public sealed class NormalBullet : MonoBehaviour
{
    private NormalBulletPool owner;
    private Vector2 velocity;
    private Vector2 fieldHalfSize;
    private float despawnPadding;
    private bool active;

    public void Initialize(
        NormalBulletPool pool,
        Vector2 direction,
        float speed,
        Vector2 playfieldSize,
        float outsidePadding)
    {
        owner = pool;
        velocity = direction.normalized * speed;
        fieldHalfSize = playfieldSize * 0.5f;
        despawnPadding = outsidePadding;
        active = true;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!active)
            return;

        if (GameManager.Instance != null &&
            !GameManager.Instance.IsPlaying)
        {
            return;
        }

        transform.position +=
            (Vector3)(velocity * Time.deltaTime);

        Vector3 position = transform.position;

        bool outside =
            Mathf.Abs(position.x) >
                fieldHalfSize.x + despawnPadding ||
            Mathf.Abs(position.y) >
                fieldHalfSize.y + despawnPadding;

        if (outside)
            Release();
    }

    public void Release()
    {
        if (!active)
            return;

        active = false;
        velocity = Vector2.zero;

        if (owner != null)
            owner.Return(this);
        else
            gameObject.SetActive(false);
    }
}
