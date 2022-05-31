using BepInEx;
using HarmonyLib;
using UnityEngine;
namespace SmokeCollision;
[BepInPlugin("valheim.jere.projectile_collision", "ProjectileCollision", "1.0.0.0")]
public class SmokeCollision : BaseUnityPlugin {
  ServerSync.ConfigSync ConfigSync = new("valheim.jere.projectile_collision")
  {
    DisplayName = "ProjectileCollision",
    CurrentVersion = "1.0.0",
    MinimumRequiredVersion = "1.0.0"
  };
  public void Awake() {
    Configuration.Init(ConfigSync, Config);
    Harmony harmony = new("valheim.jere.projectile_collision");
    harmony.PatchAll();
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
