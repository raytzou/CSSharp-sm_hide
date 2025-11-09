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
        public override string ModuleVersion => "0.87";
        public override string ModuleAuthor => "cynicat";
        public override string ModuleDescription => "A plugin that registers command to hide player from the same team";
        #endregion plugin info

        #region fields
        private static bool[] _isVisible = new bool[Server.MaxPlayers];
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
        }

        [ConsoleCommand("css_hide", "Hides player from the same team")]
        public void OnHideCommand(CCSPlayerController? player, CommandInfo info)
        {
            if (player is null || !player.IsValid || player.PlayerPawn.Value is null) return;

            var playerSlot = player.Slot;

            // here we don't set visibility, but only maintain the table for player's visibility
            _isVisible[playerSlot] = !_isVisible[playerSlot];
        }

        private void OnCheckTransmit(CCheckTransmitInfoList infoList)
        {
            foreach ((var info, var player) in infoList)
            {
                if (player is null || !player.IsValid || player.PlayerPawn.Value is null) continue;

                var teamNum = player.PlayerPawn.Value.TeamNum;
                var teammates = Utilities.GetPlayers()
                    .Where(p => p is not null
                        && p.IsValid
                        && p.PawnIsAlive
                        && p.PlayerPawn.Value is not null
                        && p.PlayerPawn.Value.TeamNum == teamNum
                        && p != player)
                    .ToList();
                var playerSlot = player.Slot;

                if (!_isVisible[playerSlot])
                {
                    foreach (var teammate in teammates)
                    {
                        if (teammate is null || !teammate.IsValid || teammate.PlayerPawn.Value is null) continue;

                        info.TransmitEntities.Remove(teammate.PlayerPawn.Value); // remove player while transmiting => invisible
                    }
                }
            }
        }
    }
}
