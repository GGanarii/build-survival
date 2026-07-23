using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class PlayerHealth : MonoBehaviour
{
    private bool isDead;

    private void Awake()
    {
        Collider2D hitbox = GetComponent<Collider2D>();
        hitbox.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || !other.TryGetComponent(out NormalBullet bullet))
            return;

        isDead = true;

        bullet.Release();
        GameManager.Instance?.EndRun();

        gameObject.SetActive(false);
    }
}
