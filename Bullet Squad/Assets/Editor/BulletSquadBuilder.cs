using System.IO;
using BulletSquad;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BulletSquadBuilder
{
    private const string Root = "Assets";
    private const string ArtPath = "Assets/Art/Generated";
    private const string PrefabPath = "Assets/Prefabs";
    private const string ScenePath = "Assets/Scenes";

    private static Sprite playerSprite;
    private static Sprite enemySprite;
    private static Sprite bossSprite;
    private static Sprite bulletSprite;
    private static Sprite grenadeSprite;
    private static Sprite platformSprite;
    private static Sprite pickupSprite;
    private static Sprite crateSprite;
    private static GameObject playerPrefab;
    private static GameObject enemyPrefab;
    private static GameObject bossPrefab;
    private static GameObject playerBulletPrefab;
    private static GameObject enemyBulletPrefab;
    private static GameObject grenadePrefab;
    private static GameObject healthPickupPrefab;
    private static GameObject ammoPickupPrefab;
    private static GameObject grenadePickupPrefab;
    private static Font uiFont;

    [MenuItem("Bullet Squad/Build Playable Prototype")]
    public static void BuildPlayablePrototype()
    {
        EnsureFolders();
        EnsureLayers();
        CreateSprites();
        CreatePrefabs();
        CreateMenuScene();
        for (int i = 1; i <= 4; i++)
        {
            CreateLevelScene(i);
        }
        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Bullet Squad playable prototype generated.");
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory(ArtPath);
        Directory.CreateDirectory(PrefabPath);
        Directory.CreateDirectory(ScenePath);
    }

    private static void EnsureLayers()
    {
        AddLayer("Player");
        AddLayer("Enemy");
    }

    private static void AddLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        for (int i = 0; i < layers.arraySize; i++)
        {
            if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
            {
                return;
            }
        }

        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return;
            }
        }
    }

    private static void CreateSprites()
    {
        playerSprite = CreateSprite("bs_player", 32, 32, new Color32(238, 188, 76, 255), new Color32(74, 45, 34, 255));
        enemySprite = CreateSprite("bs_enemy", 28, 30, new Color32(112, 166, 82, 255), new Color32(40, 70, 45, 255));
        bossSprite = CreateSprite("bs_boss_tank", 96, 54, new Color32(112, 119, 111, 255), new Color32(40, 48, 50, 255));
        bulletSprite = CreateSprite("bs_bullet", 12, 4, new Color32(255, 219, 74, 255), new Color32(255, 122, 36, 255));
        grenadeSprite = CreateSprite("bs_grenade", 10, 10, new Color32(92, 118, 74, 255), new Color32(28, 43, 35, 255));
        platformSprite = CreateSprite("bs_platform", 64, 16, new Color32(116, 78, 44, 255), new Color32(61, 38, 23, 255));
        pickupSprite = CreateSprite("bs_pickup", 18, 18, new Color32(82, 189, 255, 255), new Color32(252, 242, 119, 255));
        crateSprite = CreateSprite("bs_crate", 24, 24, new Color32(145, 93, 48, 255), new Color32(75, 45, 28, 255));
    }

    private static Sprite CreateSprite(string name, int width, int height, Color32 primary, Color32 secondary)
    {
        string pngPath = $"{ArtPath}/{name}.png";
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool border = x == 0 || y == 0 || x == width - 1 || y == height - 1;
                bool stripe = (x / 4 + y / 4) % 2 == 0;
                texture.SetPixel(x, y, border || stripe ? secondary : primary);
            }
        }

        texture.Apply();
        File.WriteAllBytes(pngPath, texture.EncodeToPNG());
        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 32;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
    }

    private static void CreatePrefabs()
    {
        playerBulletPrefab = CreateProjectilePrefab("PlayerBullet", bulletSprite, new Vector2(0.35f, 0.12f), "Default");
        enemyBulletPrefab = CreateProjectilePrefab("EnemyBullet", bulletSprite, new Vector2(0.28f, 0.12f), "Default");
        grenadePrefab = CreateProjectilePrefab("Grenade", grenadeSprite, new Vector2(0.32f, 0.32f), "Default");
        playerPrefab = CreatePlayerPrefab();
        enemyPrefab = CreateEnemyPrefab();
        bossPrefab = CreateBossPrefab();
        healthPickupPrefab = CreatePickupPrefab("HealthPickup", Pickup.PickupType.Health, 1);
        ammoPickupPrefab = CreatePickupPrefab("AmmoPickup", Pickup.PickupType.Ammo, 35);
        grenadePickupPrefab = CreatePickupPrefab("GrenadePickup", Pickup.PickupType.Grenade, 2);
    }

    private static GameObject CreateProjectilePrefab(string name, Sprite sprite, Vector2 colliderSize, string layerName)
    {
        GameObject go = new GameObject(name);
        go.layer = LayerMask.NameToLayer(layerName);
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 20;
        BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = colliderSize;
        go.AddComponent<Projectile>();
        return SavePrefab(go, name);
    }

    private static GameObject CreatePlayerPrefab()
    {
        GameObject player = new GameObject("Player");
        player.layer = LayerMask.NameToLayer("Player");
        SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
        renderer.sprite = playerSprite;
        renderer.sortingOrder = 10;
        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.freezeRotation = true;
        BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.72f, 0.95f);
        Health health = player.AddComponent<Health>();
        SetField(health, "maxHealth", 6);
        PlayerController controller = player.AddComponent<PlayerController>();
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform);
        firePoint.transform.localPosition = new Vector3(0.55f, 0.08f, 0f);
        SetField(controller, "firePoint", firePoint.transform);
        SetField(controller, "bulletPrefab", playerBulletPrefab);
        SetField(controller, "grenadePrefab", grenadePrefab);
        SetField(controller, "enemyLayers", LayerMask.GetMask("Enemy"));
        return SavePrefab(player, "Player");
    }

    private static GameObject CreateEnemyPrefab()
    {
        GameObject enemy = new GameObject("EnemySoldier");
        enemy.layer = LayerMask.NameToLayer("Enemy");
        SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
        renderer.sprite = enemySprite;
        renderer.sortingOrder = 10;
        Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
        body.freezeRotation = true;
        BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.65f, 0.9f);
        Health health = enemy.AddComponent<Health>();
        SetField(health, "maxHealth", 2);
        EnemyController controller = enemy.AddComponent<EnemyController>();
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(enemy.transform);
        firePoint.transform.localPosition = new Vector3(0.5f, 0.05f, 0f);
        SetField(controller, "firePoint", firePoint.transform);
        SetField(controller, "bulletPrefab", enemyBulletPrefab);
        SetField(controller, "playerLayers", LayerMask.GetMask("Player"));
        return SavePrefab(enemy, "EnemySoldier");
    }

    private static GameObject CreateBossPrefab()
    {
        GameObject boss = new GameObject("BossTank");
        boss.layer = LayerMask.NameToLayer("Enemy");
        SpriteRenderer renderer = boss.AddComponent<SpriteRenderer>();
        renderer.sprite = bossSprite;
        renderer.sortingOrder = 10;
        BoxCollider2D collider = boss.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(2.8f, 1.55f);
        Health health = boss.AddComponent<Health>();
        SetField(health, "maxHealth", 22);
        BossController controller = boss.AddComponent<BossController>();
        Transform[] points = new Transform[3];
        for (int i = 0; i < points.Length; i++)
        {
            GameObject firePoint = new GameObject($"FirePoint{i + 1}");
            firePoint.transform.SetParent(boss.transform);
            firePoint.transform.localPosition = new Vector3(-1f + i, 0.35f, 0f);
            points[i] = firePoint.transform;
        }
        SetField(controller, "firePoints", points);
        SetField(controller, "projectilePrefab", enemyBulletPrefab);
        SetField(controller, "playerLayers", LayerMask.GetMask("Player"));
        return SavePrefab(boss, "BossTank");
    }

    private static GameObject CreatePickupPrefab(string name, Pickup.PickupType type, int amount)
    {
        GameObject pickup = new GameObject(name);
        SpriteRenderer renderer = pickup.AddComponent<SpriteRenderer>();
        renderer.sprite = pickupSprite;
        renderer.sortingOrder = 8;
        CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.3f;
        Pickup pickupScript = pickup.AddComponent<Pickup>();
        SetField(pickupScript, "type", type);
        SetField(pickupScript, "amount", amount);
        return SavePrefab(pickup, name);
    }

    private static GameObject SavePrefab(GameObject source, string name)
    {
        string path = $"{PrefabPath}/{name}.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(source, path);
        Object.DestroyImmediate(source);
        return prefab;
    }

    private static void CreateMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Camera camera = CreateCamera(new Color32(21, 24, 31, 255));
        camera.orthographicSize = 5f;
        CreateMenuBackground();
        Canvas canvas = CreateCanvas();
        Text title = CreateText(canvas.transform, "BULLET SQUAD", 56, new Vector2(0, 160), new Vector2(720, 90));
        title.color = new Color32(255, 213, 79, 255);
        CreateText(canvas.transform, "Run. Jump. Shoot. Clear four hostile zones.", 22, new Vector2(0, 90), new Vector2(760, 50));

        MainMenuController menu = new GameObject("MainMenuController").AddComponent<MainMenuController>();
        CreateButton(canvas.transform, "START MISSION", new Vector2(0, 20), menu.StartGame);
        CreateButton(canvas.transform, "LEVEL 1", new Vector2(-270, -70), menu.StartLevel1);
        CreateButton(canvas.transform, "LEVEL 2", new Vector2(-90, -70), menu.StartLevel2);
        CreateButton(canvas.transform, "LEVEL 3", new Vector2(90, -70), menu.StartLevel3);
        CreateButton(canvas.transform, "LEVEL 4", new Vector2(270, -70), menu.StartLevel4);
        CreateText(canvas.transform, "Controls: A/D move | W/Space jump | J or Ctrl shoot | K grenade", 18, new Vector2(0, -170), new Vector2(900, 45));
        EditorSceneManager.SaveScene(scene, $"{ScenePath}/MainMenu.unity");
    }

    private static void CreateLevelScene(int level)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Color32 sky = level switch
        {
            1 => new Color32(76, 112, 126, 255),
            2 => new Color32(100, 82, 74, 255),
            3 => new Color32(58, 66, 88, 255),
            _ => new Color32(54, 47, 61, 255)
        };
        Camera camera = CreateCamera(sky);
        camera.gameObject.AddComponent<CameraFollow>();

        CreateBackground(level);
        CreatePlatform(new Vector2(18f, -3.1f), new Vector2(40f, 1f), "Ground");
        CreatePlatform(new Vector2(12f, -0.7f), new Vector2(4.5f, 0.45f), "UpperPlatform");
        CreatePlatform(new Vector2(26f, 0.4f), new Vector2(5f, 0.45f), "UpperPlatform");
        CreatePlatform(new Vector2(34f, -1.1f), new Vector2(5f, 0.45f), "UpperPlatform");

        GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        player.transform.position = new Vector3(-6f, -1.8f, 0f);

        for (int i = 0; i < 5 + level; i++)
        {
            GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);
            enemy.transform.position = new Vector3(3f + i * 4.2f, -1.85f + (i % 3 == 1 ? 2.4f : 0f), 0f);
        }

        CreatePickup(level % 2 == 0 ? ammoPickupPrefab : healthPickupPrefab, new Vector2(10f, 0.05f));
        CreatePickup(grenadePickupPrefab, new Vector2(24f, 1.15f));

        GameObject boss = (GameObject)PrefabUtility.InstantiatePrefab(bossPrefab);
        boss.transform.position = new Vector3(39f, -1.72f, 0f);
        boss.name = $"Mission{level}Boss";

        Canvas canvas = CreateCanvas();
        Text messageText = CreateText(canvas.transform, string.Empty, 32, new Vector2(0, 140), new Vector2(860, 70));
        HudController hud = new GameObject("HUD").AddComponent<HudController>();
        SetField(hud, "scoreText", CreateText(canvas.transform, "SCORE", 20, new Vector2(-385, 245), new Vector2(240, 40)));
        SetField(hud, "healthText", CreateText(canvas.transform, "LIFE", 20, new Vector2(-150, 245), new Vector2(190, 40)));
        SetField(hud, "ammoText", CreateText(canvas.transform, "ARMS", 20, new Vector2(80, 245), new Vector2(190, 40)));
        SetField(hud, "grenadeText", CreateText(canvas.transform, "BOMB", 20, new Vector2(285, 245), new Vector2(150, 40)));
        SetField(hud, "objectiveText", CreateText(canvas.transform, "MISSION", 20, new Vector2(410, 210), new Vector2(200, 40)));
        SetField(hud, "player", player.GetComponent<PlayerController>());

        LevelController controller = new GameObject("LevelController").AddComponent<LevelController>();
        SetField(controller, "levelNumber", level);
        SetField(controller, "nextSceneName", level < 4 ? $"Level{level + 1:00}" : "MainMenu");
        SetField(controller, "messageText", messageText);

        EditorSceneManager.SaveScene(scene, $"{ScenePath}/Level{level:00}.unity");
    }

    private static Camera CreateCamera(Color background)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.orthographic = true;
        camera.orthographicSize = 4.6f;
        camera.backgroundColor = background;
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        cameraObject.AddComponent<AudioListener>();
        return camera;
    }

    private static void CreateBackground(int level)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject block = new GameObject($"Backdrop_{i}");
            SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = crateSprite;
            renderer.color = level % 2 == 0 ? new Color32(78, 70, 68, 255) : new Color32(62, 82, 91, 255);
            renderer.sortingOrder = -10;
            block.transform.position = new Vector3(-8f + i * 5.4f, -1.2f + (i % 3) * 0.65f, 3f);
            block.transform.localScale = new Vector3(3.6f, 2.2f + (i % 2), 1f);
        }
    }

    private static void CreateMenuBackground()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject block = new GameObject($"MenuPlate_{i}");
            SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = platformSprite;
            renderer.color = new Color32(62, 74, 84, 255);
            renderer.sortingOrder = -5;
            block.transform.position = new Vector3(-7f + i * 2.1f, -3.4f + (i % 2) * 0.45f, 0f);
            block.transform.localScale = new Vector3(2f, 1f, 1f);
        }
    }

    private static void CreatePlatform(Vector2 position, Vector2 scale, string name)
    {
        GameObject platform = new GameObject(name);
        SpriteRenderer renderer = platform.AddComponent<SpriteRenderer>();
        renderer.sprite = platformSprite;
        renderer.sortingOrder = 1;
        platform.transform.position = position;
        platform.transform.localScale = new Vector3(scale.x, scale.y, 1f);
        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 1f);
    }

    private static void CreatePickup(GameObject prefab, Vector2 position)
    {
        GameObject pickup = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        pickup.transform.position = position;
    }

    private static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 540);
        canvasObject.AddComponent<GraphicRaycaster>();
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }
        return canvas;
    }

    private static Text CreateText(Transform parent, string content, int size, Vector2 position, Vector2 boxSize)
    {
        uiFont ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject textObject = new GameObject(content.Length > 16 ? "Text" : content);
        textObject.transform.SetParent(parent, false);
        Text text = textObject.AddComponent<Text>();
        text.text = content;
        text.font = uiFont;
        text.fontSize = size;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = boxSize;
        return text;
    }

    private static void CreateButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(label);
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color32(43, 55, 72, 255);
        Button button = buttonObject.AddComponent<Button>();
        UnityEventTools.AddPersistentListener(button.onClick, action);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(250, 54);

        Text text = CreateText(buttonObject.transform, label, 22, Vector2.zero, rect.sizeDelta);
        text.color = new Color32(255, 231, 133, 255);
    }

    private static void UpdateBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene($"{ScenePath}/MainMenu.unity", true),
            new EditorBuildSettingsScene($"{ScenePath}/Level01.unity", true),
            new EditorBuildSettingsScene($"{ScenePath}/Level02.unity", true),
            new EditorBuildSettingsScene($"{ScenePath}/Level03.unity", true),
            new EditorBuildSettingsScene($"{ScenePath}/Level04.unity", true)
        };
    }

    private static void SetField(Object target, string fieldName, object value)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        SerializedProperty property = serializedObject.FindProperty(fieldName);
        if (property == null)
        {
            Debug.LogWarning($"Field {fieldName} not found on {target.name}.");
            return;
        }

        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                property.intValue = (int)value;
                break;
            case SerializedPropertyType.Float:
                property.floatValue = (float)value;
                break;
            case SerializedPropertyType.String:
                property.stringValue = (string)value;
                break;
            case SerializedPropertyType.ObjectReference:
                property.objectReferenceValue = value as Object;
                break;
            case SerializedPropertyType.LayerMask:
                property.intValue = value is LayerMask mask ? mask.value : System.Convert.ToInt32(value);
                break;
            case SerializedPropertyType.Enum:
                property.enumValueIndex = System.Convert.ToInt32(value);
                break;
            default:
                if (property.isArray && value is Transform[] transforms)
                {
                    property.arraySize = transforms.Length;
                    for (int i = 0; i < transforms.Length; i++)
                    {
                        property.GetArrayElementAtIndex(i).objectReferenceValue = transforms[i];
                    }
                }
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
