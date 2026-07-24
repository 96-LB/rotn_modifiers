using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Collections;
namespace ModMod;

public record Modifier(string Name, Sprite OnSprite, Sprite OffSprite, int Index) {
    private static Dictionary<Modifier, bool> Enabled { get; } = [];
    private static Dictionary<Modifier, Image?> Icons { get; } = [];
    public static int GloomRowOverride { get; set; }
    
    public static Modifier[] GetAll() {
        return [.. Enabled.Keys];
    }
    
    public static Modifier Create(string name, Sprite offSprite, Sprite onSprite) {
        var mod = new Modifier(name, onSprite, offSprite, Enabled.Count);
        Enabled[mod] = false;
        Icons[mod] = null;
        return mod;
    }
    
    public static bool IsEnabled(string name) {
        foreach(var kvp in Enabled) {
            if(kvp.Key.Name == name) {
                return kvp.Value;
            }
        }
        return false;
    }
    
    public static void SetIcon(Modifier mod, Image image) {
        Object.Destroy(Icons.Get(mod)?.gameObject);
        Icons[mod] = image;
        image.sprite = Enabled[mod] ? mod.OnSprite : mod.OffSprite;
    }
    
    public static bool Toggle(Modifier mod) {
        Enabled[mod] = !Enabled[mod];
        Icons[mod]?.sprite = Enabled[mod] ? mod.OnSprite : mod.OffSprite;
        return Enabled[mod];
    }
    
    public static void ClearAll() {
        foreach(var icon in Icons.Values) {
            Object.Destroy(icon?.gameObject);
        }
        Enabled.Clear();
        Icons.Clear();
    }
}
