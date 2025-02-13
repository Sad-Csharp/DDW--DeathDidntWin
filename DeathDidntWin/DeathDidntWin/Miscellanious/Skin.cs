using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DeathDidntWin.Miscellanious;

public static class Skin
{
    public static GUISkin WhiteSkin;


    public static void TryLoadSkin()
    {
        var data = TryGetSkin("whiteskin");
        var assetBundle = AssetBundle.LoadFromMemory(data);
        WhiteSkin = assetBundle.LoadAsset<GUISkin>("WhiteSkin");
    }

    private static byte[] TryGetSkin(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(resourceName)));
        
        if (stream == null)
            return null;
        
        var data = new byte[stream.Length];
        _ = stream.Read(data, 0, data.Length);
        return data;
    }
}