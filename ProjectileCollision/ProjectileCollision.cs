using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
namespace SmokeCollision;
[BepInPlugin(GUID, NAME, VERSION)]
public class SmokeCollision : BaseUnityPlugin {
  public const string LEGACY_GUID = "valheim.jere.projectile_collision";
  public const string GUID = "projectile_collision";
  public const string NAME = "Projectile Collision";
  public const string VERSION = "1.1";
  ServerSync.ConfigSync ConfigSync = new(GUID)
  {
    DisplayName = NAME,
    CurrentVersion = VERSION,
    MinimumRequiredVersion = VERSION
  };
  private void MigrateConfig() {
    var legacyConfig = Path.Combine(Path.GetDirectoryName(Config.ConfigFilePath), $"{LEGACY_GUID}.cfg");
    if (!File.Exists(legacyConfig)) return;
    var config = Path.Combine(Path.GetDirectoryName(Config.ConfigFilePath), $"{GUID}.cfg");
    if (File.Exists(config))
      File.Delete(legacyConfig);
    else
      File.Move(legacyConfig, config);
  }
  public void Awake() {
    MigrateConfig();
    Configuration.Init(ConfigSync, Config);
    new Harmony(GUID).PatchAll();
  }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.OnHit))]
public class Projectile_OnHit {
  static bool Prefix(Collider collider, Projectile __instance) {
    if (!collider) return true;
    var owner = __instance.m_owner;
    if (Configuration.PlayerOnly && (!owner || !owner.IsPlayer())) return true;
    if (Configuration.MaxSize > 0f && __instance.m_rayRadius > Configuration.MaxSize) return true;
    var name = Utils.GetPrefabName(collider.gameObject.transform.root.gameObject).ToLower();
    if (Configuration.IgnoredIds.Contains(name)) return false;
    return true;
  }

  static void Finalizer(Collider collider, Projectile __instance) {
    if (!Configuration.DebugPrint) return;
    var obj = Utils.GetPrefabName(collider.gameObject.transform.root.gameObject);
    var hitting = __instance.m_didHit ? "hitting" : "ignoring";
    var name = Utils.GetPrefabName(__instance.gameObject);
    Console.instance.Print($"Projectile \"{name}\" (size {__instance.m_rayRadius}) {hitting} \"{obj}\"");
  }
}
