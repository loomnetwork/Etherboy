using System;
using UnityEngine;

namespace Etherboy.Editor {
    public class SpriteAtlasPackingParameters {
        private static Type _type = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.U2D.SpriteAtlasPackingParameters");

        private object _instance;

        public SpriteAtlasPackingParameters() {
            _instance = Activator.CreateInstance(_type);
        }

        public object Instance => _instance;

        public uint blockOffset {
            get {
                return (uint) _type.GetProperty("blockOffset").GetValue(_instance);
            }
            set {
                _type.GetProperty("blockOffset").SetValue(_instance, value);
            }
        }

        public uint padding {
            get {
                return (uint) _type.GetProperty("padding").GetValue(_instance);
            }
            set {
                _type.GetProperty("padding").SetValue(_instance, value);
            }
        }

        public bool allowAlphaSplitting {
            get {
                return (bool) _type.GetProperty("allowAlphaSplitting").GetValue(_instance);
            }
            set {
                _type.GetProperty("allowAlphaSplitting").SetValue(_instance, value);
            }
        }

        public bool enableRotation {
            get {
                return (bool) _type.GetProperty("enableRotation").GetValue(_instance);
            }
            set {
                _type.GetProperty("enableRotation").SetValue(_instance, value);
            }
        }

        public bool enableTightPacking {
            get {
                return (bool) _type.GetProperty("enableTightPacking").GetValue(_instance);
            }
            set {
                _type.GetProperty("enableTightPacking").SetValue(_instance, value);
            }
        }
    }
}
