using MelonLoader;
using UnityEngine;
using MelonLoader.Utils;

// Conditional compilation example for IL2CPP and MONO
// #if <Build config> is used to check the build configuration
#if IL2CPP
using Il2CppScheduleOne.NPCs; // IL2Cpp using directive
#elif MONO
using ScheduleOne.NPCs; // Mono using directive
#else
// Other build configs
#endif

[assembly: MelonInfo(typeof(ExampleNameSpace.Core), ExampleNameSpace.BuildInfo.Name, ExampleNameSpace.BuildInfo.Version, ExampleNameSpace.BuildInfo.Author, ExampleNameSpace.BuildInfo.DownloadLink)]
[assembly: MelonColor(255, 191, 0, 255)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ExampleNameSpace
{
    public static class BuildInfo
    {
        public const string Name = "Example";
        public const string Description = "";
        public const string Author = "OverweightUnicorn";
        public const string Company = "UnicornsCanMod";
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }
		
    public class Core : MelonMod
	{
		public override void OnInitializeMelon()
		{
			LoggerInstance.Msg("Initialized.");
		}

		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			base.OnSceneWasLoaded(buildIndex, sceneName);
			if (sceneName == "Main")
			{
				MelonCoroutines.Start(this.DelayedStart(5f));
			}
		}

		private System.Collections.IEnumerator DelayedStart(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			// Conditional compilation example for IL2CPP and MONO
			NPC npc = GameObject.FindObjectsOfType<NPC>()
#if IL2CPP
                .FirstOrDefault(new Func<NPC, bool>(npc => npc.FirstName == "Beth"), null);
#elif MONO
                .FirstOrDefault((npc) => npc.FirstName == "Beth");
#endif
						string currentBackend = MelonEnvironment.IsMonoRuntime ? "MONO" : "IL2CPP";
			if (npc != null)
				npc.SendTextMessage($"It works in {currentBackend}");
		}
	}
}