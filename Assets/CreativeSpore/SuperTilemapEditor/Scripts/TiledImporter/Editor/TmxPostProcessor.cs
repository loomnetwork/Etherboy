using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace CreativeSpore.TiledImporter
{
    /* Not working because AssetDatabase.Refresh() is not creating the metadata for the atlas texture when it's called inside OnPostprocessAllAssets
    class TmxPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (Path.GetExtension(assetPath).ToLower() == ".tmx")
                {
                    try
                    {
                        TmxImporter.CreateTilesetFromTmx(assetPath, Path.GetDirectoryName(assetPath));
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        void OnPreprocessTexture()
        {
            //if (assetPath.Contains("_bumpmap"))
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                //Debug.Log("asset " + assetPath);
            }
        }
    }*/
}