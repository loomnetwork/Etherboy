using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Etherboy.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.U2D;

public class SetPlatformSpriteAtlases : IPreprocessBuild, IPostprocessBuild, IActiveBuildTargetChanged {
    public void OnPreprocessBuild(BuildTarget target, string path) {
        SetSpriteAtlasesForPlatform(target);
    }

    public void OnPostprocessBuild(BuildTarget target, string path) {
    }

    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget) {
        SetSpriteAtlasesForPlatform(newTarget);
    }

    private void SetSpriteAtlasesForPlatform(BuildTarget target) {
        StringBuilder messageBuilder = new StringBuilder();
        SpriteAtlas[] spriteAtlases = Resources.FindObjectsOfTypeAll<SpriteAtlas>();
        foreach (SpriteAtlas spriteAtlas in spriteAtlases) {
            AssetImporter spriteAtlasImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(spriteAtlas));

            // Always include assets with platform-independent resolution
            if (String.IsNullOrWhiteSpace(spriteAtlasImporter.assetBundleName)) {
                SetAtlasIncluded(target, messageBuilder, spriteAtlas, true);
                continue;
            }

            // Use SD assets for WebGL, HD for everything else
            bool isHd = spriteAtlasImporter.assetBundleVariant == "hd";
            bool includeInBuild;
            if (target == BuildTarget.WebGL) {
                includeInBuild = !isHd;
            } else {
                includeInBuild = isHd;
            }

            SetAtlasIncluded(target, messageBuilder, spriteAtlas, includeInBuild);
        }

        Debug.Log(messageBuilder.ToString());
    }

    public int callbackOrder { get; }

    private static void SetAtlasIncluded(BuildTarget target, StringBuilder messageBuilder, SpriteAtlas spriteAtlas, bool includeInBuild) {
        messageBuilder.AppendFormat("SpriteAtlas: {0}, Platform: {1}, Included: {2}\n", spriteAtlas, target, includeInBuild);
        spriteAtlas.SetIncludeInBuild(includeInBuild);
    }
}
