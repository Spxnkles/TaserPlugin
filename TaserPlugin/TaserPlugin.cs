using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Core.Plugins;
using Rocket.Core;
using Rocket.API;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using UnityEngine;
using SDG.Unturned;
using Steamworks;
using System.Collections;
using Rocket.Unturned.Chat;

namespace TaserPlugin
{
    public class TaserPlugin : RocketPlugin<TaserPluginConfig>
    {
        public static TaserPlugin instance;
        public static List<CSteamID> Tased;



        protected override void Load()
        {
            instance = this;

            Tased = new List<CSteamID>();

            UnturnedEvents.OnPlayerDamaged += OnPlayerDamaged;

            Logger.Log($"Loaded {name} version {Assembly.GetName().Version} by Spinkles");
        }

        protected override void Unload()
        {
            Tased = null;

            UnturnedEvents.OnPlayerDamaged -= OnPlayerDamaged;

            Logger.Log($"Loaded {name} version {Assembly.GetName().Version} by Spinkles");
        }



        private void OnPlayerDamaged(UnturnedPlayer player, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage)
        {
            if (killer.HasPermission(Configuration.Instance.TaserPerm))
            {
                if (Configuration.Instance.Debug) Logger.Log($"{killer.CharacterName} has taser permission.");

                if (killer.Player.equipment.itemID == Configuration.Instance.Taser && killer.Player.equipment.isEquipped)
                {
                    if (Configuration.Instance.Debug) Logger.Log($"{killer.CharacterName} is using a taser.");

                    if (!Tased.Contains(player.CSteamID))
                    {
                        UnturnedChat.Say(player, "You have been tased!", Color.yellow);
                        UnturnedChat.Say(killer, $"You have tased {player.CharacterName}.", Color.yellow);
                    }

                    Tased.Add(player.CSteamID);
                    player.Player.equipment.dequip();
                    player.Player.stance.stance = EPlayerStance.PRONE;
                    player.Player.stance.checkStance(EPlayerStance.PRONE);
                    player.Player.movement.sendPluginSpeedMultiplier(0f);
                    player.Player.movement.sendPluginJumpMultiplier(0f);
                    StartCoroutine(nameof(TaseEnd), player);
                    StartCoroutine(TaseCheck(player));

                    canDamage = false;
                    damage = 0;

                    return;

                }


            }
        }

        private IEnumerator TaseEnd(UnturnedPlayer player)
        {
            yield return new WaitForSeconds(Configuration.Instance.TaseDuration);

            player.Player.movement.sendPluginSpeedMultiplier(1f);
            player.Player.movement.sendPluginJumpMultiplier(1f);

            Tased.Remove(player.CSteamID);
        }

        private IEnumerator TaseCheck(UnturnedPlayer player)
        {
            while (Tased.Contains(player.CSteamID))
            {
                player.Player.equipment.dequip();
                player.Player.stance.stance = EPlayerStance.PRONE;
                player.Player.stance.checkStance(EPlayerStance.PRONE);

                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
