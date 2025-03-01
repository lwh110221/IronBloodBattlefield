using System;
using HarmonyLib;
using SandBox.GameComponents;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using IronBloodBattlefield.Settings;
using IronBloodBattlefield.Util;

namespace IronBloodBattlefield
{
    /// <summary>
    /// 士气调整
    /// </summary>
    [HarmonyPatch(typeof(SandboxBattleMoraleModel))]
    public static class CombatMoralePatch
    {
        private static void DebugLog(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[IronBloodBattlefield] {message}");
#endif
        }

        /// <summary>
        /// 检查是否应该应用士气效果
        /// </summary>
        private static bool ShouldApplyMoraleEffect(Team agentTeam)
        {
            if (!ModSettings.IsEnabled) return false;
            if (agentTeam == null || Mission.Current?.PlayerTeam == null) return false;
            
            // 如果是敌方
            if (agentTeam.IsEnemyOf(Mission.Current.PlayerTeam))
            {
                // 检查敌方是否已确认可以撤退
                return !AgentCountCheck.IsCurrentEnemyBelowThreshold();
            }
            
            // 如果是玩家方且设置为对玩家生效
            if (ModSettings.ApplyToPlayer.Value)
            {
                // 检查玩家方是否已确认可以撤退
                return !AgentCountCheck.IsCurrentPlayerBelowThreshold();
            }

            return false;
        }

        [HarmonyPatch("CanPanicDueToMorale")]
        [HarmonyPostfix]
        public static void PostfixCanPanic(Agent agent, ref bool __result)
        {
            try
            {
                if (!BattleCheck.IsValidFieldBattle(Mission.Current))
                {
#if DEBUG
                    DebugLog("不是野战，跳过");
#endif
                    return;
                }

                if (ModSettings.IsEnabled && agent?.Team != null && ShouldApplyMoraleEffect(agent.Team))
                {
                    __result = false;
#if DEBUG
                    DebugLog($"已阻止士兵恐慌：{agent.Name}");
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                DebugLog($"士气系统错误(恐慌): {e.Message}");
#endif
            }
        }

        [HarmonyPatch("CalculateMaxMoraleChangeDueToAgentIncapacitated")]
        [HarmonyPrefix]
        public static bool PrefixMoraleChangeIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow, ref ValueTuple<float, float> __result)
        {
            bool result;
            try
            {
                if (!BattleCheck.IsValidFieldBattle(Mission.Current))
                {
                    return true;
                }

                if (ModSettings.IsEnabled && affectedAgent?.Team != null && ShouldApplyMoraleEffect(affectedAgent.Team))
                {
                    __result = new ValueTuple<float, float>(0f, 0f);
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                DebugLog($"士气系统错误(阵亡): {e.Message}");
#endif
                result = true;
            }
            return result;
        }

        [HarmonyPatch("CalculateCasualtiesFactor")]
        [HarmonyPostfix]
        public static void PostfixCasualtiesFactor(BattleSideEnum battleSide, ref float __result)
        {
            try
            {
                if (!BattleCheck.IsValidFieldBattle(Mission.Current))
                {
                    return;
                }

                Team team = Mission.Current?.Teams.Find(t => t.Side == battleSide);
                if (ModSettings.IsEnabled && team != null && ShouldApplyMoraleEffect(team))
                {
                    __result = 0f;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                DebugLog($"士气系统错误(伤亡): {e.Message}");
#endif
            }
        }

        [HarmonyPatch("CalculateMoraleChangeToCharacter")]
        [HarmonyPostfix]
        public static void PostfixMoraleChange(Agent agent, float maxMoraleChange, ref float __result)
        {
            try
            {
                if (!BattleCheck.IsValidFieldBattle(Mission.Current))
                {
                    return;
                }

                if (ModSettings.IsEnabled && agent?.Team != null && ShouldApplyMoraleEffect(agent.Team))
                {
                    __result = 0f;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                DebugLog($"士气系统错误(士气变化): {e.Message}");
#endif
            }
        } 
    }
}
