using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#if IL2CPP
using AssetBundle = UnityEngine.Il2CppAssetBundle;
#elif MONO
using AssetBundle = UnityEngine.AssetBundle;
#endif

namespace Hoverboard.TemplateUtils
{
    public static class AssetBundleUtils
    {
        static Core mod = MelonAssembly.FindMelonInstance<Core>();
        static MelonAssembly melonAssembly = mod.MelonAssembly;
        static Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

        private static AssetBundle loadedBundle;
        private static readonly Dictionary<string, UnityEngine.Object> loadedAssets = new Dictionary<string, UnityEngine.Object>();

        private static Task bundleLoadTask;

        public static AssetBundle LoadAssetBundle(string bundleFileName)
        {
            if (assetBundles.ContainsKey(bundleFileName)) { return assetBundles[bundleFileName]; }
            try
            {
                string streamPath = $"{typeof(Core).Namespace}.Assets.{bundleFileName}";
                Stream bundleStream = melonAssembly.Assembly.GetManifestResourceStream($"{streamPath}");
                if (bundleStream == null)
                {
                    mod.Unregister($"AssetBundle: '{streamPath}' not found. \nOpen .csproj file and search for '{bundleFileName}'.\nIf it doesn't exist,\nCopy your asset to Assets/ folder then look for 'your.assetbundle' in .csproj file.");
                    return null;
                }
                Utility.Log($"Loading AssetBundle: {bundleFileName}");
#if IL2CPP
                byte[] bundleData;
                using (MemoryStream ms = new MemoryStream())
                {
                    bundleStream.CopyTo(ms);
                    bundleData = ms.ToArray();
                }
                Il2CppSystem.IO.Stream stream = new Il2CppSystem.IO.MemoryStream(bundleData);
                
                AssetBundle ab = Il2CppAssetBundleManager.LoadFromStream(stream);
#elif MONO
                AssetBundle ab = AssetBundle.LoadFromStream(bundleStream);
#endif
                assetBundles.Add(bundleFileName, ab);
                return ab;
            }
            catch (Exception e)
            {
                mod.Unregister($"Failed to load AssetBundle. Please report to dev: {e}");
                return null;
            }
        }

        public static async Task<T> LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            if (loadedAssets.TryGetValue(assetName, out UnityEngine.Object asset) && asset is T) return (T)asset;

            if (loadedBundle is null)
            {
                bundleLoadTask ??= LoadAssetBundle();
                await bundleLoadTask;
            }

            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();

            AssetBundleRequest request = loadedBundle.LoadAssetAsync<T>(assetName);
            request.completed += _ => completionSource.SetResult(request.asset is UnityEngine.Object asset ? (T)asset : null);

            T result = await completionSource.Task;
            loadedAssets.Add(assetName, result);

            return result;
        }

        public static async void TestBundleLoading()
        {
            try {
                bundleLoadTask ??= LoadAssetBundle();
                await bundleLoadTask;
                Utility.Log("Asset bundle loaded successfully.");
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error loading asset bundle: {e}");
            }
        }

        private static async Task LoadAssetBundle()
        {
            TaskCompletionSource<AssetBundle> completionSource = new TaskCompletionSource<AssetBundle>();

            // The path here is the namespace of your mod, followed by the folder path to the asset bundle, with dots instead of slashes.
            // So if your mod's namespace is GorillaTagMLModExample, and you put your asset bundle in a folder called Content in your project, the path would be "GorillaTagMLModExample.Content.bundle".
            // If you change the name of your mod's namespace, make sure to change it here as well.
            // Also, make sure to make it so the bundle is compiled in with the mod in the .csproj file. An example of how to do this can be found in the .csproj file in this template.

            string streamPath = $"{typeof(Core).Namespace}.Assets.hoverboard";

            Stream stream = typeof(Core).Assembly.GetManifestResourceStream(streamPath);

            AssetBundleCreateRequest request = AssetBundle.LoadFromStreamAsync(stream);
            request.completed += _ => completionSource.SetResult(request.assetBundle);

            loadedBundle = await completionSource.Task;
        }

        public static AssetBundle GetLoadedAssetBundle(string bundleName)
        {
            if (assetBundles.ContainsKey(bundleName))
            {
                return assetBundles[bundleName];
            }
            else
            {
                mod.Unregister($"Failed to get {bundleName}");
                throw new Exception($"Asset '{bundleName}' has not been loaded in yet");
            }
        }

        public static T LoadAssetFromBundle<T>(string assetName, string bundleName) where T : UnityEngine.Object
        {
            var bundle = GetLoadedAssetBundle(bundleName);
            if (bundle == null)
            {
                throw new Exception($"Bundle not found for asset: {assetName}");
            }

            var asset = bundle.LoadAsset<T>(assetName);
            if (asset == null)
            {
                throw new Exception($"{assetName} not found in bundle {bundleName}");
            }

            return asset;
        }
    }
}