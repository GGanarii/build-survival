#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CombatPrototypeSetup
{
    [MenuItem(
        "Tools/Graze Overdrive/Create Bullet Prototype"
    )]
    public static void CreatePrototype()
    {
        if (Object.FindFirstObjectByType<GameManager>() != null)
        {
            Debug.LogWarning(
                "탄 프로토타입 시스템이 이미 존재합니다."
            );
            return;
        }

        PlayerController player =
            Object.FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            Debug.LogError(
                "PlayerController가 있는 Player를 먼저 생성하세요."
            );
            return;
        }

        PreparePlayer(player.gameObject);
        CreateGameSystems();
        PrepareCanvas();

        EditorSceneManager.MarkSceneDirty(
            EditorSceneManager.GetActiveScene()
        );

        Debug.Log(
            "일반 탄, 충돌, 사망, 재시작 시스템을 생성했습니다."
        );
    }

    private static void PreparePlayer(GameObject player)
    {
        CircleCollider2D collider =
            player.GetComponent<CircleCollider2D>();

        if (collider == null)
            collider = Undo.AddComponent<CircleCollider2D>(player);

        collider.isTrigger = true;
        collider.radius = 0.5f;

        Rigidbody2D body =
            player.GetComponent<Rigidbody2D>();

        if (body == null)
            body = Undo.AddComponent<Rigidbody2D>(player);

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        if (player.GetComponent<PlayerHealth>() == null)
            Undo.AddComponent<PlayerHealth>(player);
    }

    private static void CreateGameSystems()
    {
        GameObject root = new("GameSystems");
        Undo.RegisterCreatedObjectUndo(
            root,
            "Create game systems"
        );

        root.AddComponent<GameManager>();

        GameObject poolObject = new("NormalBulletPool");
        poolObject.transform.SetParent(root.transform, false);
        NormalBulletPool pool =
            poolObject.AddComponent<NormalBulletPool>();

        GameObject spawnerObject =
            new("NormalBulletSpawner");

        spawnerObject.transform.SetParent(
            root.transform,
            false
        );

        NormalBulletSpawner spawner =
            spawnerObject.AddComponent<NormalBulletSpawner>();

        SerializedObject serialized =
            new SerializedObject(spawner);

        serialized.FindProperty("pool")
            .objectReferenceValue = pool;

        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void PrepareCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError(
                "GameCanvas가 없습니다. 방향 UI를 먼저 생성하세요."
            );
            return;
        }

        if (canvas.GetComponent<GameOverUI>() == null)
            Undo.AddComponent<GameOverUI>(canvas.gameObject);
    }
}

#endif
