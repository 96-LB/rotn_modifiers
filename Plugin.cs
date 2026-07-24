using BepInEx;
using RiftOfTheNecroManager;
using UnityEngine;


namespace ModMod;


[BepInPlugin("rotn.katie.mod.mod", "ModMod", "1.1.0")]
public class Plugin : RiftPlugin {
    protected override void OnInit() {
        Modifier.Create("Gloom", Assets.GetSprite(SpriteType.GloomOff), Assets.GetSprite(SpriteType.GloomOn));
        Modifier.Create("Enigma", Assets.GetSprite(SpriteType.EnigmaOff), Assets.GetSprite(SpriteType.EnigmaOn));
        Modifier.Create("GlassGuitar", Assets.GetSprite(SpriteType.GlassGuitarOff), Assets.GetSprite(SpriteType.GlassGuitarOn));
        Modifier.Create("Perfectionist", Assets.GetSprite(SpriteType.PerfectsOff), Assets.GetSprite(SpriteType.PerfectsOn));
        Modifier.Create("Remix", Assets.GetSprite(SpriteType.RemixOff), Assets.GetSprite(SpriteType.RemixOn));
        base.OnInit();
    }
    
    public void Update() {
        foreach(var mod in Modifier.GetAll()) {
            if(UnityInput.Current.GetKeyDown(KeyCode.F1 + mod.Index)) {
                Modifier.Toggle(mod);
            }
        }
        for(int i = 0; i < 7; i++) {
            if(UnityInput.Current.GetKeyDown(KeyCode.Alpha0 + i)) {
                Modifier.GloomRowOverride = i + 1;
            }
        }
    }
    
    protected override void OnUnload() {
        Modifier.ClearAll();
    }
}
