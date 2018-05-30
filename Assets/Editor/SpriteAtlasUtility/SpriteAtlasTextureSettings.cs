using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Etherboy.Editor {
    public class SpriteAtlasTextureSettings {
        private static Type _type = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.U2D.SpriteAtlasTextureSettings");
        private object _instance;

        public SpriteAtlasTextureSettings() {
            _instance = Activator.CreateInstance(_type);
        }

        public object Instance => _instance;

        /*public uint anisoLevel
        {
            get
            {
                return this.m_AnisoLevel;
            }
            set
            {
                this.m_AnisoLevel = value;
            }
        }*/

        public uint compressionQuality {
            get {
                return (uint) _type.GetProperty("compressionQuality").GetValue(_instance);
            }
            set {
                _type.GetProperty("compressionQuality").SetValue(_instance, value);
            }
        }

        public uint maxTextureSize {
            get {
                return (uint) _type.GetProperty("maxTextureSize").GetValue(_instance);
            }
            set {
                _type.GetProperty("maxTextureSize").SetValue(_instance, value);
            }
        }

        public TextureImporterCompression textureCompression {
            get {
                return (TextureImporterCompression) _type.GetProperty("textureCompression").GetValue(_instance);
            }
            set {
                _type.GetProperty("textureCompression").SetValue(_instance, value);
            }
        }

        public FilterMode filterMode
        {
            get {
                return (FilterMode) _type.GetProperty("filterMode").GetValue(_instance);
            }
            set {
                _type.GetProperty("filterMode").SetValue(_instance, value);
            }
        }

        public bool generateMipMaps {
            get {
                return (bool) _type.GetProperty("generateMipMaps").GetValue(_instance);
            }
            set {
                _type.GetProperty("generateMipMaps").SetValue(_instance, value);
            }
        }

        /*
        public bool readable
        {
            get
            {
                return this.m_Readable != 0;
            }
            set
            {
                this.m_Readable = ((!value) ? 0 : 1);
            }
        }
        */

        public bool crunchedCompression {
            get {
                return (bool) _type.GetProperty("crunchedCompression").GetValue(_instance);
            }
            set {
                _type.GetProperty("crunchedCompression").SetValue(_instance, value);
            }
        }

        /*public bool sRGB
        {
            get
            {
                return this.m_sRGB != 0;
            }
            set
            {
                this.m_sRGB = ((!value) ? 0 : 1);
            }
        }*/
    }
}