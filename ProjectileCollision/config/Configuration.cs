using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BepInEx.Configuration;
using ServerSync;
using Service;
namespace SmokeCollision;
public class Configuration {
#nullable disable
  public static ConfigEntry<bool> configLocked;
  public static ConfigEntry<string> configIgnoredIds;
  public static ConfigEntry<bool> configPlayerOnly;
  public static ConfigEntry<bool> configDebugPrint;
  public static ConfigEntry<string> configMaxSize;
#nullable enable
  public static bool PlayerOnly => configPlayerOnly.Value;
  public static bool DebugPrint => configDebugPrint.Value;
  public static float MaxSize = 0f;
  public static HashSet<string> IgnoredIds = new();
  private static void ParseIds() {
    IgnoredIds = configIgnoredIds.Value.Split(',').Select(s => s.Trim().ToLower()).ToHashSet();
  }
  public static float ParseFloat(string value, float defaultValue = 0) {
    if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) return result;
    return defaultValue;
  }
  public static void Init(ConfigSync configSync, ConfigFile configFile) {
    ConfigWrapper wrapper = new("projectile_config", configFile, configSync);
    var section = "General";
    configLocked = wrapper.BindLocking(section, "Config locked", false, "When true, server sets the config values.");
    configIgnoredIds = wrapper.Bind(section, "Ignored object ids", "iron_floor_1x1,iron_floor_2x2,iron_grate,iron_wall_1x1,iron_wall_2x2", "Object ids separated by , that are ignored by the smoke.");
    configIgnoredIds.SettingChanged += (s, e) => ParseIds();
    ParseIds();
    configDebugPrint = wrapper.Bind(section, "Debug print", false, "Prints projectile information to the console.");
    configPlayerOnly = wrapper.Bind(section, "Player only", true, "Only ignore collision for player projectiles.");
    configMaxSize = wrapper.Bind(section, "Max size", "", "Max size for the projectile to ignore collision.");
    configMaxSize.SettingChanged += (s, e) => {
      MaxSize = ParseFloat(configMaxSize.Value, 0f);
    };
    MaxSize = ParseFloat(configMaxSize.Value, 0f);
  }
}
