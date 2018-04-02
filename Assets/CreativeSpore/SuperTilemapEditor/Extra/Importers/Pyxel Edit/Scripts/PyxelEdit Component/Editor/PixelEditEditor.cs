using UnityEngine;
using UnityEditor;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;
using CubesTeam.Boomlagoon.JSON;
using AnimationImporter;

namespace CubesTeam
{
    [CustomEditor(typeof(PyxelEdit))]
    public class PixelEditEditor : Editor
    {        

        private PyxelEditTilemap pyxelTilemap;
        private Tileset tileset;

        private PyxelEdit pyxelEdit;
        void OnEnable()
        {
            pyxelEdit = target as PyxelEdit;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Generate PyxelEdit Tilemap"))
            {
                GenerateTilemap();
            }
        }
        
        public void GenerateTilemap()
        {
            if (this.PrepareTilset())
            {
                this.PrepareLayersGroup();
                this.DrawTileToLayers();
            }
        }

        private PyxelEditTilemap DeserializeJSON()
        {
            PyxelEditTilemap tilemap = new PyxelEditTilemap();

            JSONObject jsonObject = JSONObject.Parse(pyxelEdit.tilemapJSON.ToString());
            tilemap.tileheight = (int)jsonObject["tileheight"].Number;
            tilemap.tileshigh = (int)jsonObject["tileshigh"].Number;
            tilemap.tilewidth = (int)jsonObject["tilewidth"].Number;
            tilemap.tileswide = (int)jsonObject["tileswide"].Number;
            JSONArray layersData = jsonObject["layers"].Array;
            int layersCount = layersData.Length;
            tilemap.layers = new LayerRefs[layersCount];
            for (int i = 0; i < layersCount; i++)
            {
                tilemap.layers[i] = new LayerRefs();
                tilemap.layers[i].name = layersData[i].Obj["name"].Str;
                tilemap.layers[i].number = (int)layersData[i].Obj["number"].Number;

                JSONArray tilesData = layersData[i].Obj["tiles"].Array;
                int tilesCount = tilesData.Length;
                tilemap.layers[i].tiles = new TileRefs[tilesCount];

                for (int t = 0; t < tilesCount; t++)
                {
                    tilemap.layers[i].tiles[t] = new TileRefs();

                    tilemap.layers[i].tiles[t].flipX = tilesData[t].Obj["flipX"].Boolean;
                    tilemap.layers[i].tiles[t].index = (int)tilesData[t].Obj["flipX"].Number;
                    tilemap.layers[i].tiles[t].rot = (int)tilesData[t].Obj["rot"].Number;
                    tilemap.layers[i].tiles[t].tile = (int)tilesData[t].Obj["tile"].Number;
                    tilemap.layers[i].tiles[t].x = (int)tilesData[t].Obj["x"].Number;
                    tilemap.layers[i].tiles[t].y = (int)tilesData[t].Obj["y"].Number;
                }
            }
            return tilemap;
        }

        private bool PrepareTilset()
        {
            // use with UNITY JSON
            //this.pyxelTilemap = JsonUtility.FromJson<PyxelEditTilemap>(this.tilemapJSON.text);

            // use with JSONObject class (http://wiki.unity3d.com/index.php?title=JSONObject)
            this.pyxelTilemap = DeserializeJSON();

            string path = AssetDatabase.GetAssetPath(pyxelEdit.tilemapJSON.GetInstanceID());
            string assetPath = System.IO.Path.GetDirectoryName(path) + "/" + pyxelEdit.tilemapJSON.name + "_tileset.asset";

            this.tileset = AssetDatabase.LoadAssetAtPath<Tileset>(assetPath);
            bool createNewTileset = !this.tileset;
            if (createNewTileset)
                this.tileset = ScriptableObject.CreateInstance<Tileset>();

            string pathToTexture = System.IO.Path.GetDirectoryName(path) + "/" + pyxelEdit.tilemapJSON.name + ".png";

            Texture2D tilesetTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(pathToTexture, typeof(Texture2D));

            if (tilesetTexture == null)
            {
                Debug.LogError("PyxelEdit ERROR: missing texture at " + pathToTexture + " !");
                return false;
            }
            else
            {
                tileset.PixelsPerUnit = this.pyxelTilemap.tilewidth;
                tileset.TilePxSize = new Vector2(this.pyxelTilemap.tilewidth, this.pyxelTilemap.tileheight);
                tileset.AtlasTexture = tilesetTexture;
                tileset.SliceOffset = pyxelEdit.sliceOffset;
                tileset.SlicePadding = pyxelEdit.slicePadding;
                tileset.Slice();
                TilesetEditor.OptimizeTextureImportSettings(tileset.AtlasTexture);
                if (createNewTileset)
                    AssetDatabaseUtility.CreateAssetAndDirectories(tileset, assetPath);
                EditorUtility.SetDirty(this.tileset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }
        }

        private void PrepareLayersGroup()
        {
            if (pyxelEdit.GetComponent<TilemapGroup>() == null)
            {
                pyxelEdit.gameObject.AddComponent<TilemapGroup>();
            }

            for (int i = 0; i < this.pyxelTilemap.layers.Length; i++)
            {
                GameObject goLayer = this.AddLayer(this.pyxelTilemap.layers[i].name);

                STETilemap map = goLayer.GetComponent<STETilemap>();
                if (map == null) map = goLayer.AddComponent<STETilemap>();

                map.Tileset = this.tileset;
                map.OrderInLayer = pyxelEdit.orderInLayer + (-i * pyxelEdit.layerDistance);
                goLayer.transform.parent = pyxelEdit.gameObject.transform;
                EditorUtility.SetDirty(goLayer);
            }
        }

        private GameObject AddLayer(string _name)
        {
            Transform obj = pyxelEdit.transform.Find(_name);

            if (obj != null) DestroyImmediate(obj.gameObject);
            return new GameObject(_name);
        }

        private void DrawTileToLayers()
        {
            bool fx = false;
            bool fy = false;
            int rot = 0;

            for (int i = 0; i < this.pyxelTilemap.layers.Length; i++)
            {
                STETilemap map = pyxelEdit.transform.Find(this.pyxelTilemap.layers[i].name).GetComponent<STETilemap>();

                for (int t = 0; t < this.pyxelTilemap.layers[i].tiles.Length; t++)
                {
                    int tileID = this.pyxelTilemap.layers[i].tiles[t].tile;

                    rot = this.pyxelTilemap.layers[i].tiles[t].rot;
                    fx = this.pyxelTilemap.layers[i].tiles[t].flipX;
                    fy = (rot == 2) ? true : false;
                    if (rot == 2) fx = !fx;

                    int x = this.pyxelTilemap.layers[i].tiles[t].x;
                    int y = (this.pyxelTilemap.tileshigh - this.pyxelTilemap.layers[i].tiles[t].y) - 1;

                    if (tileID < 0)
                    {
                        map.Erase(x, y);
                    }
                    else
                    {
                        TileData tileData = new TileData();
                        tileData.brushId = 0;
                        tileData.tileId = this.pyxelTilemap.layers[i].tiles[t].tile;

                        tileData.flipHorizontal = fx;
                        tileData.flipVertical = fy;

                        map.SetTileData(x, y, tileData.BuildData());
                    }
                }

                map.UpdateMesh();
            }
        }
    }
}