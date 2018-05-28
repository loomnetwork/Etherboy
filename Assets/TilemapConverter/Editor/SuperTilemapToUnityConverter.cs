using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CreativeSpore.SuperTilemapEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using STileset = CreativeSpore.SuperTilemapEditor.Tileset;
using STile = CreativeSpore.SuperTilemapEditor.Tile;
using Tile = UnityEngine.Tilemaps.Tile;

public class SuperTilemapToUnityConverter : EditorWindow {
    [SerializeField]
    private STileset[] _tilesets;

    [SerializeField]
    private string _outDir = "Assets/TilemapAtlasesUnity";

    [SerializeField]
    private Vector2 _tileSize = Vector2.one * 0.65f;

    [SerializeField]
    private bool _deactivateSteTileGroup = true;

    [MenuItem("Tools/Converter - Supertilamp to native Unity tilemap")]
    private static void OpenWindow() {
        GetWindow<SuperTilemapToUnityConverter>(false);
    }

    private void OnGUI() {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty stringsProperty = so.FindProperty("_tilesets");
            EditorGUILayout.PropertyField(stringsProperty, new GUIContent("SuperTilemap Tilesets"), true);
            so.ApplyModifiedProperties();

            _outDir = EditorGUILayout.TextField("Out dir", _outDir);
            _tileSize = EditorGUILayout.Vector2Field("Tile Size", _tileSize);

            if (_tilesets != null) {
                if (GUILayout.Button("Convert Tileset")) {
                    EditorApplication.delayCall += () => {
                        foreach (Tileset tileset in _tilesets) {
                            ConvertTilesetAtlas(tileset);
                        }
                    };
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            _deactivateSteTileGroup = EditorGUILayout.ToggleLeft("Disable STE TilemapGroups ", _deactivateSteTileGroup);

            if (GUILayout.Button("Convert all STETilemap to Unity Tilemap")) {
                ConvertSteTilemaps();
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void ConvertSteTilemaps() {
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject tilemapRootGo = null;
            STETilemap[] steTilemaps = FindObjectsOfTypeAllInScene<STETilemap>(scene);
            foreach (STETilemap steTilemap in steTilemaps) {
                // in-editor
                if ((steTilemap.hideFlags & HideFlags.HideInHierarchy) != 0)
                    continue;

                GameObject convertSteTilemap = ConvertSteTilemap(steTilemap);
                if (tilemapRootGo == null) {
                    tilemapRootGo = new GameObject(steTilemap.ParentTilemapGroup.name + "_Unity");
                    tilemapRootGo.transform.parent = steTilemap.ParentTilemapGroup.gameObject.transform.parent;
                    tilemapRootGo.transform.localPosition = steTilemap.ParentTilemapGroup.gameObject.transform.localPosition;
                    tilemapRootGo.transform.localScale = steTilemap.ParentTilemapGroup.gameObject.transform.localScale;
                }

                convertSteTilemap.transform.parent = tilemapRootGo.transform;

                if (_deactivateSteTileGroup) {
                    Undo.RecordObject(steTilemap.ParentTilemapGroup.gameObject, "");
                    steTilemap.ParentTilemapGroup.gameObject.SetActive(false);
                }
            }

            //EditorGUIUtility.PingObject(tilemapRootGo);
        }
    }

    private GameObject ConvertSteTilemap(STETilemap steTilemap) {
        string outDir = _outDir + "/" + steTilemap.Tileset.AtlasTexture.name + "/";
        string unityPalettePath = outDir + steTilemap.Tileset.AtlasTexture.name + ".prefab";
        string tileIndexMapPath = outDir + steTilemap.Tileset.AtlasTexture.name + "IndexMap" + ".asset";
        GameObject palette = AssetDatabase.LoadAssetAtPath<GameObject>(unityPalettePath);
        if (palette == null)
            throw new Exception("Unity palette not found");

        TileIndexMap tileIndexMap = AssetDatabase.LoadAssetAtPath<TileIndexMap>(tileIndexMapPath);
        if (tileIndexMap == null)
            throw new Exception("tileIndexMap not found");

        GameObject tilemapGo = (GameObject) PrefabUtility.InstantiatePrefab(palette);
        Undo.RegisterCreatedObjectUndo(tilemapGo, "Create tilemap");
        PrefabUtility.DisconnectPrefabInstance(tilemapGo);
        tilemapGo.name = steTilemap.gameObject.name;
        SceneManager.MoveGameObjectToScene(tilemapGo, steTilemap.gameObject.scene);

        TilemapRenderer tilemapRenderer = tilemapGo.GetComponentInChildren<TilemapRenderer>();
        tilemapRenderer.enabled = true;
        tilemapRenderer.sortingLayerID = steTilemap.SortingLayerID;
        tilemapRenderer.sortingOrder = steTilemap.OrderInLayer;

        Tilemap tilemap = tilemapGo.GetComponentInChildren<Tilemap>();
        tilemap.ClearAllTiles();

        for (int i = steTilemap.MinGridX; i < steTilemap.MaxGridX; i++) {
            for (int j = steTilemap.MinGridY; j < steTilemap.MaxGridY; j++) {
                STile steTile = steTilemap.GetTile(i, j);
                if (steTile == null)
                    continue;

                int steTileIndex = steTilemap.Tileset.Tiles.IndexOf(steTile);
                if (steTileIndex == -1)
                    throw new Exception("steTileIndex == -1");

                uint steTileData = steTilemap.GetTileData(i, j);
                TileColor32 steTileColor32 = steTilemap.GetTileColor(i, j);

                // average color
                Color tileColor = ((Color) steTileColor32.c0) + ((Color) steTileColor32.c1) + ((Color) steTileColor32.c2) + ((Color) steTileColor32.c3);
                tileColor *= 0.25f;

                Vector3Int tilePosition = new Vector3Int(i, j, 0);
                tilemap.SetTile(tilePosition, tileIndexMap.Tiles[steTileIndex]);

                Matrix4x4 tileMatrix;
                Vector3 tileScale = Vector3.one;
                Vector3 tileRotation = Vector3.zero;
                if ((steTileData & Tileset.k_TileFlag_FlipV) != 0) {
                    tileScale.y = -1;
                }

                if ((steTileData & Tileset.k_TileFlag_FlipH) != 0) {
                    tileScale.x = -1;
                }

                if ((steTileData & Tileset.k_TileFlag_Rot90) != 0) {
                    tileRotation.z = -90;
                }

                tileMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(tileRotation), tileScale);
                tilemap.SetTransformMatrix(tilePosition, tileMatrix);
                tilemap.SetColor(tilePosition, tileColor);
            }
        }

        tilemapGo.transform.parent = steTilemap.transform.parent.parent;
        tilemapGo.transform.position = steTilemap.transform.position;

        return tilemapGo;
    }

    private void ConvertTilesetAtlas(STileset tileset) {
        Texture atlasTexture = tileset.AtlasTexture;
        Debug.Log(atlasTexture, atlasTexture);
        TextureImporter atlasTextureImporter = (TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(atlasTexture));
        atlasTextureImporter.spriteImportMode = SpriteImportMode.Multiple;
        SpriteMetaData[] spritesMetaData = new SpriteMetaData[tileset.Tiles.Count];
        for (int i = 0; i < tileset.Tiles.Count; i++) {
            STile tilesetTile = tileset.Tiles[i];

            spritesMetaData[i].name = atlasTexture.name + "_Tile_" + i;

            Rect rect = tilesetTile.uv;
            rect.x *= atlasTexture.width;
            rect.width *= atlasTexture.width;
            rect.y *= atlasTexture.height;
            rect.height *= atlasTexture.height;
            spritesMetaData[i].rect = rect;
        }

        atlasTextureImporter.spritesheet = spritesMetaData;
        EditorUtility.SetDirty(atlasTextureImporter);
        atlasTextureImporter.SaveAndReimport();

        Sprite[] sprites =
            AssetDatabase.LoadAllAssetsAtPath(atlasTextureImporter.assetPath)
                .Where(o => o is Sprite)
                .Cast<Sprite>()
                .OrderBy(sprite => Array.FindIndex(spritesMetaData, md => md.name == sprite.name))
                .ToArray();

        string outDir = _outDir + "/" + atlasTexture.name + "/";
        string tileOutDir = outDir + "Tiles/";
        AssetDatabase.StartAssetEditing();

        try {
            TileBase[] tiles = new TileBase[tileset.Tiles.Count];
            for (int i = 0; i < tileset.Tiles.Count; i++) {
                Tile tile = CreateInstance<Tile>();
                tile.colliderType = Tile.ColliderType.None;
                tile.sprite = sprites[i];
                tile.flags &= ~TileFlags.LockColor;

                string tilePath = Path.Combine(tileOutDir, tile.sprite.name + ".asset");
                Directory.CreateDirectory(Path.GetDirectoryName(tilePath));
                AssetDatabase.CreateAsset(tile, tilePath);

                tiles[i] = tile;
            }

            // Palette
            GameObject paletteGo = new GameObject(atlasTexture.name);
            Grid paletteGrid = paletteGo.AddComponent<Grid>();
            paletteGrid.cellSize = Vector3.one;
            GameObject paletteLayerGo = new GameObject(atlasTexture.name);
            paletteLayerGo.transform.parent = paletteGo.transform;
            paletteLayerGo.transform.localPosition = Vector3.zero;
            Tilemap tilemap = paletteLayerGo.AddComponent<Tilemap>();
            TilemapRenderer tilemapRenderer = paletteLayerGo.AddComponent<TilemapRenderer>();
            tilemapRenderer.enabled = false;

            TileIndexMap tileIndexMap = CreateInstance<TileIndexMap>();
            tileIndexMap.Tiles = new TileBase[tileset.Tiles.Count];

            int tileCounter = 0;
            int rowCounter = 0;

            Vector3Int[] tilePositions = new Vector3Int[tileset.Tiles.Count];
            while (tileCounter < tileset.Tiles.Count) {
                for (int i = 0; i < tileset.TileRowLength; i++) {
                    tilePositions[tileCounter] = new Vector3Int(i, -rowCounter, 0);
                    tileCounter++;
                }

                rowCounter++;
            }

            for (int i = 0; i < tileset.Tiles.Count; i++) {
                tileIndexMap.Tiles[i] = tiles[i];
            }

            tilemap.SetTiles(tilePositions, tiles);

            paletteGo.transform.localScale = new Vector3(_tileSize.x, _tileSize.y, 1f);

            string palettePath = Path.Combine(outDir, paletteGo.name + ".prefab");
            PrefabUtility.CreatePrefab(palettePath, paletteGo);
            GridPalette gridPalette = CreateInstance<GridPalette>();
            gridPalette.name = "Palette Settings";
            gridPalette.cellSizing = GridPalette.CellSizing.Automatic;
            AssetDatabase.AddObjectToAsset(gridPalette, palettePath);
            DestroyImmediate(paletteGo);

            string tileIndexMapPath = Path.Combine(outDir, atlasTexture.name + "IndexMap" + ".asset");
            AssetDatabase.CreateAsset(tileIndexMap, tileIndexMapPath);
        } finally {
            AssetDatabase.StopAssetEditing();
        }
    }

    public static T[] FindObjectsOfTypeAllInScene<T>(Scene scene) {
        List<T> results = new List<T>();
        if (scene.isLoaded) {
            GameObject[] allGameObjects = scene.GetRootGameObjects();
            for (int j = 0; j < allGameObjects.Length; j++) {
                GameObject go = allGameObjects[j];
                results.AddRange(go.GetComponentsInChildren<T>(true));
            }
        }

        return results.ToArray();
    }
}
