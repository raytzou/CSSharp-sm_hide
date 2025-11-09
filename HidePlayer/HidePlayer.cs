using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace HidePlayer
{
    public class HidePlayer : BasePlugin
    {
        #region plugin info
        public override string ModuleName => "Hide Player";
        public override string ModuleVersion => "0.87.1";
        public override string ModuleAuthor => "cynicat";
        public override string ModuleDescription => "A plugin that registers command to hide player from the same team";
        #endregion plugin info

        #region fields
        private static bool[] _isVisible = new bool[Server.MaxPlayers];
        
        // Performance optimization: Cache valid players and team mappings
        private static List<CCSPlayerController> _cachedValidPlayers = new();
        private static Dictionary<int, List<CCSPlayerController>> _teamCache = new();
        private static int _lastCacheUpdate = 0;
        private static readonly int CacheDurationTicks = 5; // Update cache every 5 ticks
        #endregion fields

        public override void Load(bool hotReload)
        {
            RegisterListener<Listeners.OnMapStart>(OnMapStart);
            RegisterListener<Listeners.CheckTransmit>(OnCheckTransmit);
        }

        private void OnMapStart(string mapName)
        {
            _isVisible = new bool[Server.MaxPlayers];
            for (int i = 0; i < _isVisible.Length; i++)
                _isVisible[i] = true;
                
            // Clear caches on map start
            ClearCaches();
        }

        [ConsoleCommand("css_hide", "Hides player from the same team")]
        public void OnHideCommand(CCSPlayerController? player, CommandInfo info)
        {
            if (!IsPlayerValid(player)) return;

            var playerSlot = player!.Slot;

            // here we don't set visibility, but only maintain the table for player's visibility
            _isVisible[playerSlot] = !_isVisible[playerSlot];
            
            // Optionally clear cache when player settings change
            // This ensures immediate effect but slightly reduces performance benefit
            InvalidateTeamCache(player.PlayerPawn.Value!.TeamNum);
        }

        private void OnCheckTransmit(CCheckTransmitInfoList infoList)
        {
            // Update cache periodically instead of every tick
            UpdateCacheIfNeeded();
            
            foreach ((var info, var player) in infoList)
            {
                if (!IsPlayerValid(player) || _isVisible[player!.Slot]) continue;

                var teamNum = player.PlayerPawn.Value!.TeamNum;
                
                // Use cached team data instead of filtering every time
                if (_teamCache.TryGetValue(teamNum, out var teammates))
                {
                    foreach (var teammate in teammates)
                    {
                        if (teammate == player) continue; // Skip self
                        
                        // Double-check validity before removing (players can disconnect)
                        if (IsPlayerValid(teammate))
                        {
                            info.TransmitEntities.Remove(teammate.PlayerPawn.Value!);
                        }
                    }
                }
            }
        }

        private void UpdateCacheIfNeeded()
        {
            var currentTick = Server.TickCount;
            if (currentTick - _lastCacheUpdate < CacheDurationTicks) return;
            
            _lastCacheUpdate = currentTick;
            
            // Get all valid players once
            _cachedValidPlayers = Utilities.GetPlayers()
                .Where(IsPlayerValid)
                .ToList();
            
            // Group players by team
            _teamCache.Clear();
            foreach (var player in _cachedValidPlayers)
            {
                var teamNum = player.PlayerPawn.Value!.TeamNum;
                if (!_teamCache.ContainsKey(teamNum))
                {
                    _teamCache[teamNum] = new List<CCSPlayerController>();
                }
                _teamCache[teamNum].Add(player);
            }
        }
        
        private void InvalidateTeamCache(int teamNum)
        {
            _teamCache.Remove(teamNum);
        }
        
        private void ClearCaches()
        {
            _cachedValidPlayers.Clear();
            _teamCache.Clear();
            _lastCacheUpdate = 0;
        }

        private static bool IsPlayerValid(CCSPlayerController? player) => 
            player is not null && player.IsValid && player.PlayerPawn.Value is not null;
    }
}
