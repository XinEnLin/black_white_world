using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.IO;

public static class Day1Setup
{
    [MenuItem("BlackWhiteWorld/Day1 Setup Scene")]
    public static void SetupDay1()
    {
        // --- New scene ---
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // --- Main Camera ---
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";       
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.165f, 0.165f, 0.165f, 1f); // #2A2A2A
        cam.clearFlags = CameraClearFlags.SolidColor;
        camGO.transform.position = new Vector3(0f, 0f, -10f);

        var uaData = camGO.AddComponent<UniversalAdditionalCameraData>();
        uaData.renderPostProcessing = true;

        camGO.AddComponent<AudioListener>();

        // --- Global Volume ---
        var volGO = new GameObject("GlobalVolume");
        var vol = volGO.AddComponent<Volume>();
        vol.isGlobal = true;
        vol.priority = 0;

        // Create Volume Profile asset
        string profileDir = "Assets/Art";
        if (!Directory.Exists(profileDir))
            Directory.CreateDirectory(profileDir);

        string profilePath = profileDir + "/GlobalVolumeProfile.asset";
        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, profilePath);

        var colorAdj = profile.Add<ColorAdjustments>(true);
        colorAdj.saturation.value = -100f;
        colorAdj.saturation.overrideState = true;

        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();

        vol.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);

        // --- Player ---
        var playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        playerGO.transform.position = Vector3.zero;

        var sr = playerGO.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
        sr.color = Color.white;

        var rb = playerGO.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        var col = playerGO.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;

        // Add PlayerController
        string controllerPath = "Assets/Scripts/Player/PlayerController.cs";
        var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(controllerPath);
        if (monoScript != null)
        {
            var t = monoScript.GetClass();
            if (t != null)
                playerGO.AddComponent(t);
        }

        // --- Colored reference sprites ---
        CreateColorSquare("RedBox",   Color.red,     new Vector3(-3f, 2f, 0f));
        CreateColorSquare("BlueBox",  Color.blue,    new Vector3( 0f, 2f, 0f));
        CreateColorSquare("YellowBox",Color.yellow,  new Vector3( 3f, 2f, 0f));

        // --- Save scene ---
        string scenesDir = "Assets/Scenes";
        if (!Directory.Exists(scenesDir))
            Directory.CreateDirectory(scenesDir);

        string scenePath = scenesDir + "/Main.unity";
        EditorSceneManager.SaveScene(scene, scenePath);

        // Add to build settings
        var buildScenes = EditorBuildSettings.scenes;
        bool alreadyAdded = System.Array.Exists(buildScenes, s => s.path == scenePath);
        if (!alreadyAdded)
        {
            var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(buildScenes)
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
            EditorBuildSettings.scenes = list.ToArray();
        }

        AssetDatabase.Refresh();
        Debug.Log("[Day1Setup] Done! Main.unity created with GlobalVolume + Player + color boxes.");
    }

    static void CreateColorSquare(string name, Color color, Vector3 pos)
    {
        var go = new GameObject(name);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color = color;
    }
}