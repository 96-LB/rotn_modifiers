using System.Collections.Generic;
using System.IO;
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

    public static Image gloomImage = null;
    private static Sprite gloomOn = null;
    private static Sprite gloomOff = null;

    public static Image enigmaImage = null;
    private static Sprite enigmaOn = null;
    private static Sprite enigmaOff = null;

    public static Image glassImage = null;
    private static Sprite glassOn = null;
    private static Sprite glassOff = null;

    public static Image perfImage = null;
    private static Sprite perfOn = null;
    private static Sprite perfOff = null;

    public static Image remixImage = null;
    private static Sprite remixOn = null;
    private static Sprite remixOff = null;


    private Sprite load_sprite( string path )
    {
        byte[] gloom_on_bytes = File.ReadAllBytes(PluginData.Info.Location + path);
        Texture2D gloom_tex = new Texture2D(2,2);
        ImageConversion.LoadImage( gloom_tex, gloom_on_bytes  );
        return Sprite.Create( gloom_tex, new Rect(0,0, gloom_tex.width, gloom_tex.height), new Vector2(0.5f,0.5f), 100.0f );
    }

    private void Awake()
    {

        Logger.LogInfo(PluginData.Info.Location + "\\icons\\GloomOn.png");

        gloomOn = load_sprite("\\icons\\GloomOn.png");
        gloomOff = load_sprite("\\icons\\GloomOff.png");

        enigmaOn = load_sprite("\\icons\\EnigmaOn.png");
        enigmaOff = load_sprite("\\icons\\EnigmaOff.png");

        glassOn = load_sprite("\\icons\\GlassGuitarOn.png");
        glassOff = load_sprite("\\icons\\GlassGuitarOff.png");

        perfOn = load_sprite("\\icons\\PerfectsOn.png");
        perfOff = load_sprite("\\icons\\PerfectsOff.png");

        remixOn = load_sprite("\\icons\\RemixOn.png");
        remixOff = load_sprite("\\icons\\RemixOff.png");
    }




    private static Image createIcon(RectTransform screen, float top, float step, float idx, string name)
    {
        GameObject icon = new GameObject(name, typeof(RectTransform));
        icon.transform.SetParent(screen);

        Image img = icon.AddComponent<UnityEngine.UI.Image>();
        img.preserveAspect = true;
        
        RectTransform trans = (RectTransform)icon.transform;
        trans.anchorMin = new Vector2(0.0f,0.0f);
        trans.anchorMax = new Vector2(.0f,.0f);
        trans.anchoredPosition = new Vector2(64, top - step * idx );
        trans.sizeDelta = new Vector2(64,64);

        return img;
    }


    public static void ModifierKeybinds()
    {
        if( UnityInput.Current.GetKeyDown(KeyCode.F1)) {
            pin_overrides["Gloom"] = !pin_overrides["Gloom"];
            if( gloomImage is not null ) gloomImage.sprite = pin_overrides["Gloom"] ? gloomOn : gloomOff;
            
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F2)) {
            pin_overrides["Enigma"] = !pin_overrides["Enigma"];
            if( enigmaImage is not null ) enigmaImage.sprite = pin_overrides["Enigma"] ? enigmaOn : enigmaOff;
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F3)) {
            pin_overrides["GlassGuitar"] = !pin_overrides["GlassGuitar"];
            if( glassImage is not null ) glassImage.sprite = pin_overrides["GlassGuitar"] ? glassOn : glassOff;
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F4)){
             pin_overrides["Perfectionist"] = !pin_overrides["Perfectionist"];
             if( perfImage is not null ) perfImage.sprite = pin_overrides["Perfectionist"] ? perfOn : perfOff;
        }
        if(UnityInput.Current.GetKeyDown(KeyCode.F5))
        {
            customRemix = !customRemix;
            if( remixImage is not null ) remixImage.sprite = customRemix ? remixOn : remixOff;
        }

        for( int i = 0; i < 7; i++)
        {
            if( UnityInput.Current.GetKeyDown(KeyCode.Alpha0 + i)) GloomRowOverride = i+1;
        }
    }
}
