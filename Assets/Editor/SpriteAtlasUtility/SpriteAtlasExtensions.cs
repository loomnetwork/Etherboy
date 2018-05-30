using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Etherboy.Editor {
    public static class SpriteAtlasExtensions {
        private static Type _type = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.U2D.SpriteAtlasExtensions");

        public static void Add(this SpriteAtlas spriteAtlas, UnityEngine.Object[] objects) {
            _type.GetMethod("Add").Invoke(null, new object[] { spriteAtlas, objects });
        }

        //public static void Remove(this SpriteAtlas spriteAtlas, UnityEngine.Object[] objects);

        //public static void RemoveAt(this SpriteAtlas spriteAtlas, int index);

        //public static UnityEngine.Object[] GetPackables(this SpriteAtlas spriteAtlas);

        public static void CopyTextureSettingsTo(this SpriteAtlas spriteAtlas, SpriteAtlasTextureSettings dest) {
            _type.GetMethod("SetTextureSettings").Invoke(null, new object[] { spriteAtlas, dest.Instance });
        }

        public static void SetTextureSettings(this SpriteAtlas spriteAtlas, SpriteAtlasTextureSettings src) {
            _type.GetMethod("SetTextureSettings").Invoke(null, new object[] { spriteAtlas, src.Instance });
        }

        public static bool CopyPlatformSettingsIfAvailable(this SpriteAtlas spriteAtlas, string buildTarget, TextureImporterPlatformSettings dest) {
            return (bool) _type.GetMethod("CopyPlatformSettingsIfAvailable").Invoke(null, new object[] { spriteAtlas, buildTarget, dest });
        }

        public static void SetPlatformSettings(this SpriteAtlas spriteAtlas, TextureImporterPlatformSettings src) {
            _type.GetMethod("SetPlatformSettings").Invoke(null, new object[] { spriteAtlas, src });
        }

        public static void CopyPackingParametersTo(this SpriteAtlas spriteAtlas, SpriteAtlasPackingParameters dest) {
            _type.GetMethod("CopyPackingParametersTo").Invoke(null, new object[] { spriteAtlas, dest.Instance });
        }

        public static void SetPackingParameters(this SpriteAtlas spriteAtlas, SpriteAtlasPackingParameters src) {
            _type.GetMethod("SetPackingParameters").Invoke(null, new object[] { spriteAtlas, src.Instance });
        }

        public static void SetIncludeInBuild(this SpriteAtlas spriteAtlas, bool value) {
            _type.GetMethod("SetIncludeInBuild").Invoke(null, new object[] { spriteAtlas, value });
        }

        public static void SetIsVariant(this SpriteAtlas spriteAtlas, bool value) {
            _type.GetMethod("SetIsVariant").Invoke(null, new object[] { spriteAtlas, value });
        }

        public static void SetMasterAtlas(this SpriteAtlas spriteAtlas, SpriteAtlas value) {
            _type.GetMethod("SetMasterAtlas").Invoke(null, new object[] { spriteAtlas, value });
        }

        //public static void CopyMasterAtlasSettings(this SpriteAtlas spriteAtlas);

        public static void SetVariantMultiplier(this SpriteAtlas spriteAtlas, float value) {
            _type.GetMethod("SetVariantMultiplier").Invoke(null, new object[] { spriteAtlas, value });
        }

        //public static string GetHashString(this SpriteAtlas spriteAtlas);

        //public static Texture2D[] GetPreviewTextures(this SpriteAtlas spriteAtlas);

        //public static TextureImporterFormat FormatDetermineByAtlasSettings(this SpriteAtlas spriteAtlas, BuildTarget target);
    }
}