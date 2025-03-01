using System;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using IronBloodBattlefield.Settings;
using IronBloodBattlefield.Util;

namespace IronBloodBattlefield
{
    /// <summary>
    /// 战术调整（野战撤退）
    /// </summary>
    [HarmonyPatch(typeof(TacticCoordinatedRetreat))]
    public static class TacticWeightPatch
    {
        private static void DebugLog(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[IronBloodBattlefield] [战术] {message}");
#endif
        }

        [HarmonyPatch("GetTacticWeight")]
        [HarmonyPrefix]
        public static bool PrefixGetTacticWeight(ref TacticCoordinatedRetreat __instance, ref float __result)
        {
            bool result;
            try
            {
                if (!ModSettings.IsEnabled)
                {
                    return true;
                }

                if (!BattleCheck.IsValidFieldBattle(Mission.Current))
                {
                    return true;
                }

                if (Mission.Current.PlayerTeam.Side == __instance.Team.Side)
                {
                    return true;
                }

                if (AgentCountCheck.IsCurrentEnemyBelowThreshold())
                {
#if DEBUG
                    DebugLog($"敌方可以撤退");
#endif
                    return true;
                }

                __result = 0f;
                result = false;
#if DEBUG
                DebugLog($"阻止敌方撤退");
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                DebugLog($"战术系统错误: {e.Message}");
#endif
                result = true;
            }
            return result;
        }
    }
}
