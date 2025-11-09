# HidePlayer
<a href="http://www.youtube.com/watch?feature=player_embedded&v=M-UwAzYuawc" target="_blank">
 <img src="http://img.youtube.com/vi/M-UwAzYuawc/mqdefault.jpg" alt="Watch the video" width="240" height="180" border="10" />
</a>

A CounterStrikeSharp plugin that allows players to individually toggle the visibility of their teammates.

## Description

HidePlayer is a server-side plugin for Counter-Strike 2 that gives each player the ability to hide their teammates from their own view. This is particularly useful for players who want to reduce visual clutter or focus on gameplay without teammate interference. Each player's visibility settings are independent and do not affect other players' views.

## Features

- **Individual Control**: Each player can independently toggle teammate visibility
- **Team-Based**: Only affects teammates from the same team
- **Per-Player Settings**: One player's settings don't affect others
- **Real-Time Toggle**: Instant visibility changes without requiring respawn
- **Lightweight**: Uses efficient entity transmission control

## Requirements

- Counter-Strike 2 Dedicated Server
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) v1.0.346 or higher
- .NET 8.0 Runtime

## Installation

1. Download the latest release or build from source
2. Place the `HidePlayer.dll` file in your CounterStrikeSharp plugins directory:
   ```
   csgo/addons/counterstrikesharp/plugins/HidePlayer/
   ```
3. Restart your server or use the plugin reload command

## Usage

### Commands

| Command | Description | Permission |
|---------|-------------|------------|
| `css_hide` | Toggle teammate visibility on/off | All players |
| `!hide` | Chat version of the hide command | All players |

### How it Works

1. Player types `css_hide` or `!hide` in console/chat
2. Plugin toggles the visibility state for that player
3. When enabled, teammates become invisible to that specific player
4. Other players are unaffected and continue to see all teammates normally
5. The setting persists until the player toggles it again or disconnects

## Technical Details

The plugin uses CounterStrikeSharp's `CheckTransmit` event to control entity visibility on a per-client basis. Instead of modifying render colors (which affects all players), it removes teammate entities from the transmission list for specific clients, achieving true per-player visibility control.

### Key Components

- **Entity Transmission Control**: Uses `OnCheckTransmit` to filter entities
- **Player Validation**: Robust validation for player and pawn validity
- **State Management**: Tracks visibility preferences per player slot
- **Team Detection**: Automatically identifies teammates based on team number

## Building from Source

### Prerequisites

- .NET 8.0 SDK
- CounterStrikeSharp.API NuGet package

### Build Steps

```bash
# Clone the repository
git clone <repository-url>
cd HidePlayer

# Restore dependencies
dotnet restore

# Build the plugin
dotnet build --configuration Release
```

The compiled plugin will be available in `bin/Release/net8.0/HidePlayer.dll`

## Configuration

This plugin currently does not require any configuration files. All settings are managed through in-game commands.

## Compatibility

- **CounterStrikeSharp**: v1.0.346+
- **Counter-Strike 2**: Latest version
- **Server OS**: Windows/Linux
- **.NET**: 8.0

## Version History

- **v0.87** - Current version
  - Implemented teammate hiding using CheckTransmit event
  - Added per-player visibility control
  - Refactored code for better maintainability

## To Do
- Bullets can pass through hidden players
- Control player transparency through ARGB instead of blocking Transmit which makes hidden players completely disappear

## Support

For support, bug reports, or feature requests, please open an issue on the project repository.

---

*This plugin enhances the Counter-Strike 2 experience by providing players with more control over their visual environment while maintaining fair gameplay for all participants.*