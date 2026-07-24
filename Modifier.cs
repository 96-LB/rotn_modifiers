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
    
    public static bool Toggle(Modifier mod) {
        Enabled[mod] = !Enabled[mod];
        Icons[mod]?.sprite = Enabled[mod] ? mod.OnSprite : mod.OffSprite;
        Object.Destroy(null);
        return Enabled[mod];
    }
    
    public static void ClearAll() {
        foreach(var mod in Enabled.Keys) {
            mod.DestroyIcon();
        }
        Enabled.Clear();
        Icons.Clear();
    }
    
    public void SetIcon(Image image) {
        DestroyIcon();
        Icons[this] = image;
        image.sprite = Enabled[this] ? OnSprite : OffSprite;
    }
    
    public void DestroyIcon() {
        if(Icons.Get(this)) {
            Object.Destroy(Icons.Get(this)?.gameObject);
        }
    }
}
