using System.Collections.Generic;
using BepInEx;
using RiftOfTheNecroManager;
using UnityEngine;
using UnityEngine.UI;

namespace ModMod;


[BepInPlugin("rotn.katie.mod.mod", "ModMod", "0.0.1")]
[NecroManagerInfo(isBeta: true)]
public class ModifiersPlugin : RiftPlugin {
    public static float timer = 0;

    public static Dictionary<string, bool> pin_overrides = new() {
        ["Gloom"] = false,
        ["Enigma"] = false,
        ["GlassGuitar"] = false,
        ["Perfectionist"] = false
    };
    
    public static bool customRemix = false;
    public static int GloomRowOverride = 3;
    
    private static Image CreateIcon(RectTransform screen, float top, float step, float idx, string name) {
        var icon = new GameObject(name, typeof(RectTransform));
        
        var trans/*rights*/ = icon.GetComponent<RectTransform>();
        trans.SetParent(screen);
        trans.anchorMin = new(0, 0);
        trans.anchorMax = new(0, 0);
        trans.anchoredPosition = new(64, top - step * idx );
        trans.sizeDelta = new(64, 64);
        
        var img = icon.AddComponent<Image>();
        img.preserveAspect = true;
        
        return img;
    }
    
    public static void ModifierKeybinds()
    {
        if( UnityInput.Current.GetKeyDown(KeyCode.F1)) {
            pin_overrides["Gloom"] = !pin_overrides["Gloom"];
            gloomImage?.sprite = pin_overrides["Gloom"] ? gloomOn : gloomOff;
            
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F2)) {
            pin_overrides["Enigma"] = !pin_overrides["Enigma"];
            enigmaImage?.sprite = pin_overrides["Enigma"] ? enigmaOn : enigmaOff;
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F3)) {
            pin_overrides["GlassGuitar"] = !pin_overrides["GlassGuitar"];
            glassImage?.sprite = pin_overrides["GlassGuitar"] ? glassOn : glassOff;
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F4)){
             pin_overrides["Perfectionist"] = !pin_overrides["Perfectionist"];
             perfImage?.sprite = pin_overrides["Perfectionist"] ? perfOn : perfOff;
        }
        if(UnityInput.Current.GetKeyDown(KeyCode.F5))
        {
            customRemix = !customRemix;
            remixImage?.sprite = customRemix ? remixOn : remixOff;
        }

        for( int i = 0; i < 7; i++)
        {
            if( UnityInput.Current.GetKeyDown(KeyCode.Alpha0 + i)) GloomRowOverride = i+1;
        }
    }
}
