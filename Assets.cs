using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using RiftOfTheNecroManager;
using UnityEngine.UIElements.Collections;

namespace Modifiers;

public enum SpriteType {
    GloomOff,
    GloomOn,
    EnigmaOff,
    EnigmaOn,
    GlassOff,
    GlassOn,
    PerfectionistOff,
    PerfectionistOn,
    RemixOff,
    RemixOn
}

public static class Assets {
    private static readonly Dictionary<SpriteType, Sprite> sprites = [];
    
    public static Sprite GetSprite(SpriteType key) => sprites.Get(key);
    
    private static Sprite MakeSprite(byte[] data) {
        Texture2D tex = new(0, 0);
        if(!tex.LoadImage(data)) {
            return null;
        }
        return Sprite.Create(tex,
            new(0, 0, tex.width, tex.height),
            new(0.5f, 0.5f),
            100
        );
    }
    
    static Assets() {
        var path = PluginData.DataPath;
        var usingCustom = Directory.Exists(path);
        if(!usingCustom) {
            Log.Info($"No custom icons directory found. The folder should be named '{PluginData.Name}' and located in the same directory as the game executable. No custom icons will be loaded.");
        }
        
        var spriteTypes = Enum.GetValues(typeof(SpriteType)) as SpriteType[];
        foreach(var type in spriteTypes) {
            var filename = $"{Enum.GetName(typeof(SpriteType), type)}.png";
            
            // first try to load custom sprites from the filesystem
            if(usingCustom) {
                var file = Path.Combine(path, filename);
                if(File.Exists(file)) {
                    var data = File.ReadAllBytes(file);
                    sprites[type] = MakeSprite(data);
                    if(sprites[type]) {
                        Log.Info($"Loaded custom shadow sprite for {type}.");
                    } else {
                        Log.Warning($"Failed to load custom shadow sprite for {type}. The file may not be a valid PNG image.");
                    }
                } else {
                    Log.Info($"No custom icon sprite found for {type}. The file should be named '{filename}' and located in the '{PluginData.Name}' directory.");
                }
            }
            
            // load default sprites from resources
            if(!sprites[type]) {
                using var manifest = PluginData.Assembly.GetManifestResourceStream(typeof(Assets), $"{Enum.GetName(typeof(SpriteType), type)}.png");
                using var stream = new MemoryStream();
                manifest.CopyTo(stream);
                var data = stream.ToArray();
                sprites[type] = MakeSprite(data);
            }
        }
    }
}
