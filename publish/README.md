# Projectile collision

Client side mod that makes projectiles to ignore certain structures.

The configuration can synced by also installing on the server.

# Manual Installation:

1. Install the [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/).
2. Download the latest zip.
3. Extract it in the \<GameDirectory\>\BepInEx\plugins\ folder.

# Configuration

The config file has a list of object ids that are ignored by projectiles. Check [here](https://valheim.fandom.com/wiki/Item_IDs) for object ids.

By default following structures let player projectiles pass:

- Cage Floor 1x1
- Cage Floor 2x2
- Cage Wall 1x1
- Cage Wall 2x2
- Iron gate

Additional settings:

- Debug print: If enabled, debug information is printed to the console when projectiles hit something (projectile id, projectile size, target id).
- Max size: If set, only smaller projectiles can ignore collision. Useful if you want only arrows to ignore collision.
- Player only: If disabled, enemy projectiles can also ignore collision.

# Changelog

- v1.0
	- Initial release.
