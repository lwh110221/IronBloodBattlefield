using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.Core;
using System;
using IronBloodBattlefield.Settings;

namespace IronBloodBattlefield
{
    public class SubModule : MBSubModuleBase
    {
        private static bool PatchesApplied = false;
        
        private static void DebugLog(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[IronBloodBattlefield] {message}");
#endif
        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try 
            {
                var _ = McmSettings.Instance;
#if DEBUG
                DebugLog("MCM设置已初始化");
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                DebugLog($"MCM设置初始化失败: {e.Message}");
#endif
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            try
            {
                if (!PatchesApplied)
                {
                    Harmony harmony = new Harmony("com.ahao.ironbloodbattlefield");
                    harmony.PatchAll();
                    PatchesApplied = true;
#if DEBUG
                    DebugLog("补丁已在OnGameInitializationFinished中应用");
#endif
                }
                var _ = McmSettings.Instance;
#if DEBUG
                DebugLog($"游戏初始化完成，当前设置状态：");
                DebugLog($"全局启用：{ModSettings.IronBloodBattlefield.Value}");
                DebugLog($"对玩家生效：{ModSettings.ApplyToPlayer.Value}");
                DebugLog($"敌人撤退阈值：{ModSettings.EnemyRetreatThreshold.Value}");
                DebugLog($"玩家撤退阈值：{ModSettings.PlayerRetreatThreshold.Value}");
                DebugLog($"确认次数：{ModSettings.RetreatConfirmationCount.Value}");
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                DebugLog($"补丁应用或设置确认失败: {e.Message}");
#endif
            }
        }
    }
}
