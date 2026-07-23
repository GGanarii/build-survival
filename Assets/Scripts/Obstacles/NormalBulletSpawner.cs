using UnityEngine;

public sealed class NormalBulletSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NormalBulletPool pool;

    [Header("Playfield")]
    [SerializeField]
    private Vector2 playfieldSize =
        new(9f, 16f);

    [SerializeField, Min(0.01f)]
    private float spawnPadding = 0.4f;

    [SerializeField, Min(0.01f)]
    private float despawnPadding = 0.8f;

    [Header("Bullet")]
    [SerializeField, Min(0.01f)]
    private float bulletSpeed = 2.5f;

    [Header("Timing")]
    [SerializeField, Min(0.1f)]
    private float initialDelay = 1f;

    [SerializeField, Min(0.1f)]
    private float spawnInterval = 0.85f;

    private float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Time.time + initialDelay;
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            !GameManager.Instance.IsPlaying)
        {
            return;
        }

        if (Time.time < nextSpawnTime)
            return;

        SpawnBullet();
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void SpawnBullet()
    {
        if (pool == null)
            return;

        Vector2 spawnPosition = GetRandomOutsidePosition();

        Vector2 targetPosition = new(
            Random.Range(
                -playfieldSize.x * 0.35f,
                playfieldSize.x * 0.35f
            ),
            Random.Range(
                -playfieldSize.y * 0.35f,
                playfieldSize.y * 0.35f
            )
        );

        Vector2 direction =
            (targetPosition - spawnPosition).normalized;

        NormalBullet bullet = pool.Get();
        bullet.transform.position = spawnPosition;

        bullet.Initialize(
            pool,
            direction,
            bulletSpeed,
            playfieldSize,
            despawnPadding
        );
    }

    private Vector2 GetRandomOutsidePosition()
    {
        float halfWidth = playfieldSize.x * 0.5f;
        float halfHeight = playfieldSize.y * 0.5f;

        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector2(
                Random.Range(-halfWidth, halfWidth),
                halfHeight + spawnPadding
            ),

            1 => new Vector2(
                halfWidth + spawnPadding,
                Random.Range(-halfHeight, halfHeight)
            ),

            2 => new Vector2(
                Random.Range(-halfWidth, halfWidth),
                -halfHeight - spawnPadding
            ),

            _ => new Vector2(
                -halfWidth - spawnPadding,
                Random.Range(-halfHeight, halfHeight)
            )
        };
    }
}
