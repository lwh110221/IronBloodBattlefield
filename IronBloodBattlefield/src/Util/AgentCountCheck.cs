using TaleWorlds.MountAndBlade;
using IronBloodBattlefield.Settings;
using TaleWorlds.Core;
using System.Linq;
using System.Diagnostics;

namespace IronBloodBattlefield.Util
{
    /// <summary>
    /// 战场部队数量检查工具类
    /// </summary>
    public static class AgentCountCheck
    {
        // 缓存的数量
        private static int _cachedAttackerCount;
        private static int _cachedDefenderCount;
        private static int _cachedPlayerCount;
        private static int _cachedEnemyCount;
        
        // 缓存的初始总兵力
        private static int _initialAttackerTotalCount;
        private static int _initialDefenderTotalCount;
        private static bool _hasInitialCounts;
        
        // 上次更新时间
        private static float _lastUpdateTime;
        
        // 更新间隔（游戏秒）
        private const float UPDATE_INTERVAL = 1f;

        // 延迟检查相关
        private static int _enemyBelowThresholdCount;    // 敌方连续低于阈值的次数
        private static int _playerBelowThresholdCount;   // 玩家方连续低于阈值的次数
        private static int _lastEnemyCount;              // 上次检查的敌方数量
        private static int _lastPlayerCount;             // 上次检查的玩家方数量
        private static bool _isEnemyConfirmed;           // 敌方是否已确认可以撤退
        private static bool _isPlayerConfirmed;          // 玩家方是否已确认可以撤退

        private static Mission _lastMission;

        private static void DebugLog(string message)
        {
#if DEBUG
            Debug.WriteLine($"[IronBloodBattlefield] {message}");
#endif
        }

        /// <summary>
        /// 重置检查状态
        /// </summary>
        private static void ResetCheckStatus()
        {
            _enemyBelowThresholdCount = 0;
            _playerBelowThresholdCount = 0;
            _lastEnemyCount = 0;
            _lastPlayerCount = 0;
            _isEnemyConfirmed = false;
            _isPlayerConfirmed = false;
            _cachedAttackerCount = 0;
            _cachedDefenderCount = 0;
            _cachedPlayerCount = 0;
            _cachedEnemyCount = 0;
            _lastUpdateTime = 0f;
            _hasInitialCounts = false;
            _initialAttackerTotalCount = 0;
            _initialDefenderTotalCount = 0;
#if DEBUG
            DebugLog("重置所有检查状态");
#endif
        }

        /// <summary>
        /// 初始化总兵力数量
        /// </summary>
        private static void InitializeTotalCounts(Mission mission)
        {
            if (_hasInitialCounts || mission == null) return;

            var spawnLogic = mission.GetMissionBehavior<MissionAgentSpawnLogic>();
            if (spawnLogic == null) return;

            // 检查是否在部署阶段
            if (!spawnLogic.IsInitialSpawnOver)
            {
#if DEBUG
                DebugLog("部署阶段，跳过初始化总兵力");
#endif
                return;
            }

            // 记录初始总兵力
            int activeAttackers = spawnLogic.NumberOfActiveAttackerTroops;
            int remainingAttackers = spawnLogic.NumberOfRemainingAttackerTroops;
            int activeDefenders = mission.DefenderTeam.ActiveAgents.Where(agent => !agent.IsMount).Count();
            int remainingDefenders = spawnLogic.NumberOfRemainingDefenderTroops;

            // 只有当活跃部队数量大于0时才初始化
            if (activeAttackers > 0 && activeDefenders > 0)
            {
                _initialAttackerTotalCount = activeAttackers + remainingAttackers;
                _initialDefenderTotalCount = activeDefenders + remainingDefenders;
                _hasInitialCounts = true;

#if DEBUG
                DebugLog($"初始化总兵力 - 进攻方：{_initialAttackerTotalCount}（当前{activeAttackers} + 增援{remainingAttackers}），" +
                        $"防守方：{_initialDefenderTotalCount}（当前{activeDefenders} + 增援{remainingDefenders}）");
#endif
            }
#if DEBUG
            else
            {
                DebugLog($"等待部队生成 - 进攻方当前：{activeAttackers}，防守方当前：{activeDefenders}");
            }
#endif
        }

        /// <summary>
        /// 更新缓存的数量
        /// </summary>
        private static void UpdateCachedCounts(Mission mission)
        {
            if (mission == null) 
            {
#if DEBUG
                DebugLog("Mission为空，跳过更新");
#endif
                return;
            }

            // 检查是否是新的战斗
            if (_lastMission != mission)
            {
#if DEBUG
                DebugLog($"检测到新战斗 - 当前战斗类型：{(mission.IsSiegeBattle ? "攻城战" : mission.IsFieldBattle ? "野战" : "其他")}");
#endif
                _lastMission = mission;
                ResetCheckStatus();
            }

            float currentTime = mission.CurrentTime;
            if (currentTime - _lastUpdateTime < UPDATE_INTERVAL) return;

            _lastUpdateTime = currentTime;

            // 更新进攻方和防守方数量
            if (mission.AttackerTeam != null && mission.DefenderTeam != null)
            {
                _cachedAttackerCount = mission.AttackerTeam.ActiveAgents.Count;
                _cachedDefenderCount = mission.DefenderTeam.ActiveAgents.Count;
#if DEBUG
                DebugLog($"当前战场状态 - 进攻方：{_cachedAttackerCount} 防守方：{_cachedDefenderCount}");
#endif
            }
            else
            {
#if DEBUG
                DebugLog("无法获取进攻方或防守方数量");
#endif
                _cachedAttackerCount = 0;
                _cachedDefenderCount = 0;
            }

            // 更新玩家方和敌方数量
            if (mission.PlayerTeam != null && mission.PlayerEnemyTeam != null)
            {
                _cachedPlayerCount = mission.PlayerTeam.ActiveAgents.Count;
                _cachedEnemyCount = mission.PlayerEnemyTeam.ActiveAgents.Count;
#if DEBUG
                DebugLog($"当前战场状态 - 玩家方：{_cachedPlayerCount} 敌方：{_cachedEnemyCount}");
#endif

                // 更新敌方检查状态
                UpdateEnemyCheckStatus();
                // 更新玩家方检查状态
                UpdatePlayerCheckStatus();
            }
            else
            {
#if DEBUG
                DebugLog("无法获取玩家方或敌方数量");
#endif
                _cachedPlayerCount = 0;
                _cachedEnemyCount = 0;
            }
        }

        /// <summary>
        /// 更新敌方检查状态
        /// </summary>
        private static void UpdateEnemyCheckStatus()
        {
            // 如果mod未启用，重置所有状态
            if (!ModSettings.IsEnabled)
            {
#if DEBUG
                DebugLog("Mod未启用，重置敌方状态");
#endif
                _enemyBelowThresholdCount = 0;
                _isEnemyConfirmed = false;
                _lastEnemyCount = 0;
                return;
            }

            // 如果是第一次检测
            if (_enemyBelowThresholdCount == 0)
            {
                if (_cachedEnemyCount < ModSettings.EnemyRetreatThreshold.Value)
                {
#if DEBUG
                    DebugLog($"首次检测到敌方数量低于阈值：{_cachedEnemyCount} < {ModSettings.EnemyRetreatThreshold.Value}");
#endif
                    _enemyBelowThresholdCount = 1;
                }
                else
                {
#if DEBUG
                    DebugLog($"敌方数量正常：{_cachedEnemyCount} >= {ModSettings.EnemyRetreatThreshold.Value}");
#endif
                }
                _lastEnemyCount = _cachedEnemyCount;
                return;
            }

            // 检查数量变化
            if (_cachedEnemyCount > _lastEnemyCount)
            {
#if DEBUG
                DebugLog($"敌方数量增加：{_lastEnemyCount} -> {_cachedEnemyCount}，重置计数");
#endif
                _enemyBelowThresholdCount = 0;
                _isEnemyConfirmed = false;
            }
            else if (_cachedEnemyCount < ModSettings.EnemyRetreatThreshold.Value)
            {
                // 只有在未确认状态下才增加计数
                if (!_isEnemyConfirmed)
                {
                    _enemyBelowThresholdCount++;
#if DEBUG
                    DebugLog($"敌方持续低于阈值：{_cachedEnemyCount}，当前计数：{_enemyBelowThresholdCount}/{ModSettings.RetreatConfirmationCount.Value}");
#endif
                }
                
                if (_enemyBelowThresholdCount >= ModSettings.RetreatConfirmationCount.Value && !_isEnemyConfirmed)
                {
                    _isEnemyConfirmed = true;
#if DEBUG
                    DebugLog("敌方确认可以撤退！");
#endif
                }
            }
            else
            {
                if (_enemyBelowThresholdCount > 0)
                {
#if DEBUG
                    DebugLog($"敌方数量恢复正常：{_cachedEnemyCount} >= {ModSettings.EnemyRetreatThreshold.Value}，重置计数");
#endif
                }
                _enemyBelowThresholdCount = 0;
                _isEnemyConfirmed = false;
            }

            _lastEnemyCount = _cachedEnemyCount;
        }

        /// <summary>
        /// 更新玩家方检查状态
        /// </summary>
        private static void UpdatePlayerCheckStatus()
        {
            // 如果mod未启用或不影响玩家方，重置所有状态
            if (!ModSettings.IsEnabled || !ModSettings.ApplyToPlayer.Value)
            {
#if DEBUG
                DebugLog("Mod未启用或不影响玩家方，重置玩家方状态");
#endif
                _playerBelowThresholdCount = 0;
                _isPlayerConfirmed = false;
                _lastPlayerCount = 0;
                return;
            }

            // 如果是第一次检测
            if (_playerBelowThresholdCount == 0)
            {
                if (_cachedPlayerCount < ModSettings.PlayerRetreatThreshold.Value)
                {
#if DEBUG
                    DebugLog($"首次检测到玩家方数量低于阈值：{_cachedPlayerCount} < {ModSettings.PlayerRetreatThreshold.Value}");
#endif
                    _playerBelowThresholdCount = 1;
                }
                _lastPlayerCount = _cachedPlayerCount;
                return;
            }

            // 检查数量变化
            if (_cachedPlayerCount > _lastPlayerCount)
            {
#if DEBUG
                DebugLog($"玩家方数量增加：{_lastPlayerCount} -> {_cachedPlayerCount}，重置计数");
#endif
                _playerBelowThresholdCount = 0;
                _isPlayerConfirmed = false;
            }
            else if (_cachedPlayerCount < ModSettings.PlayerRetreatThreshold.Value)
            {
                // 只有在未确认状态下才增加计数
                if (!_isPlayerConfirmed)
                {
                    _playerBelowThresholdCount++;
#if DEBUG
                    DebugLog($"玩家方持续低于阈值：{_cachedPlayerCount}，当前计数：{_playerBelowThresholdCount}/{ModSettings.RetreatConfirmationCount.Value}");
#endif
                }
                
                if (_playerBelowThresholdCount >= ModSettings.RetreatConfirmationCount.Value && !_isPlayerConfirmed)
                {
                    _isPlayerConfirmed = true;
#if DEBUG
                    DebugLog("玩家方确认可以撤退！");
#endif
                }
            }
            else
            {
                _playerBelowThresholdCount = 0;
                _isPlayerConfirmed = false;
            }

            _lastPlayerCount = _cachedPlayerCount;
        }

        /// <summary>
        /// 获取战场双方的部队数量
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>返回格式化的字符串，包含进攻方和防守方的数量</returns>
        public static (int attackerCount, int defenderCount) GetAgentCount(Mission mission)
        {
            if (mission?.AttackerTeam == null || mission.DefenderTeam == null)
            {
                return (0, 0);
            }

            UpdateCachedCounts(mission);
            return (_cachedAttackerCount, _cachedDefenderCount);
        }

        /// <summary>
        /// 获取当前战场双方的部队数量
        /// </summary>
        /// <returns>返回当前战场双方的部队数量</returns>
        public static (int attackerCount, int defenderCount) GetCurrentAgentCount()
        {
            return GetAgentCount(Mission.Current);
        }

        /// <summary>
        /// 获取玩家方和敌方的部队数量
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>返回玩家方和敌方的数量，如果无法确定则返回(0,0)</returns>
        public static (int playerCount, int enemyCount) GetPlayerAndEnemyCount(Mission mission)
        {
            if (mission?.PlayerTeam == null || mission.PlayerEnemyTeam == null)
            {
                return (0, 0);
            }

            UpdateCachedCounts(mission);
            return (_cachedPlayerCount, _cachedEnemyCount);
        }

        /// <summary>
        /// 获取当前战场玩家方和敌方的部队数量
        /// </summary>
        /// <returns>返回当前战场玩家方和敌方的数量</returns>
        public static (int playerCount, int enemyCount) GetCurrentPlayerAndEnemyCount()
        {
            return GetPlayerAndEnemyCount(Mission.Current);
        }

        /// <summary>
        /// 检查玩家是否为进攻方
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>如果玩家是进攻方返回true，否则返回false</returns>
        public static bool IsPlayerAttacker(Mission mission)
        {
            if (mission?.PlayerTeam == null || mission.AttackerTeam == null)
            {
                return false;
            }

            return mission.PlayerTeam == mission.AttackerTeam;
        }

        /// <summary>
        /// 检查敌方是否可以恢复原有游戏机制
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>如果敌方可以恢复原有游戏机制返回true，否则返回false</returns>
        public static bool IsEnemyBelowThreshold(Mission mission)
        {
            if (mission == null) return false;

            var team = mission.PlayerEnemyTeam;
            if (team == null) return false;

            // 如果已经确认可以撤退，直接返回true，避免重复计算
            if (_isEnemyConfirmed && !ModSettings.UsePercentageMode.Value)
            {
#if DEBUG
                DebugLog("敌方已确认可以撤退，跳过计算");
#endif
                return true;
            }

            if (ModSettings.UsePercentageMode.Value)
            {
                // 获取总伤亡比例
                int totalTroops = GetTotalTroopCount(team);
                int casualties = GetCasualtiesCount(team);
                
                if (totalTroops == 0) return false;
                
                float casualtyPercentage = (float)casualties / totalTroops * 100f;
#if DEBUG
                DebugLog($"敌方总兵力：{totalTroops}，伤亡：{casualties}，伤亡比例：{casualtyPercentage}%，阈值：{ModSettings.EnemyRetreatPercentage.Value}%");
#endif
                bool canRetreat = casualtyPercentage >= ModSettings.EnemyRetreatPercentage.Value;
                if (canRetreat)
                {
                    _isEnemyConfirmed = true; 
                }
                return canRetreat;
            }
            else
            {
                var (_, enemyCount) = GetPlayerAndEnemyCount(mission);
                return _isEnemyConfirmed;
            }
        }

        /// <summary>
        /// 检查玩家方是否可以恢复原有游戏机制
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>如果玩家方可以恢复原有游戏机制返回true，否则返回false</returns>
        public static bool IsPlayerBelowThreshold(Mission mission)
        {
            if (mission == null) return false;

            var team = mission.PlayerTeam;
            if (team == null) return false;

            // 如果已经确认可以撤退，直接返回true，避免重复计算
            if (_isPlayerConfirmed && !ModSettings.UsePercentageMode.Value)
            {
#if DEBUG
                DebugLog("玩家方已确认可以撤退，跳过计算");
#endif
                return true;
            }

            if (ModSettings.UsePercentageMode.Value)
            {
                // 获取总伤亡比例
                int totalTroops = GetTotalTroopCount(team);
                int casualties = GetCasualtiesCount(team);
                
                if (totalTroops == 0) return false;
                
                float casualtyPercentage = (float)casualties / totalTroops * 100f;
#if DEBUG
                DebugLog($"玩家方总兵力：{totalTroops}，伤亡：{casualties}，伤亡比例：{casualtyPercentage}%，阈值：{ModSettings.PlayerRetreatPercentage.Value}%");
#endif
                bool canRetreat = casualtyPercentage >= ModSettings.PlayerRetreatPercentage.Value;
                if (canRetreat)
                {
                    _isPlayerConfirmed = true;
                }
                return canRetreat;
            }
            else
            {
                var (playerCount, _) = GetPlayerAndEnemyCount(mission);
                return _isPlayerConfirmed;
            }
        }

        /// <summary>
        /// 检查当前战场敌方是否可以恢复原有游戏机制
        /// </summary>
        /// <returns>如果敌方可以恢复原有游戏机制返回true，否则返回false</returns>
        public static bool IsCurrentEnemyBelowThreshold()
        {
            return IsEnemyBelowThreshold(Mission.Current);
        }

        /// <summary>
        /// 检查当前战场玩家方是否可以恢复原有游戏机制
        /// </summary>
        /// <returns>如果玩家方可以恢复原有游戏机制返回true，否则返回false</returns>
        public static bool IsCurrentPlayerBelowThreshold()
        {
            return IsPlayerBelowThreshold(Mission.Current);
        }

        private static int GetTotalTroopCount(Team team)
        {
            if (team == null) return 0;
            
            var mission = Mission.Current;
            if (mission == null) return 0;

            // 初始化总兵力（如果还未初始化）
            InitializeTotalCounts(mission);

            var battleSide = team.Side;
            bool isPlayerTeam = (team == mission.PlayerTeam);
            bool isPlayerAttacker = IsPlayerAttacker(mission);

#if DEBUG
            DebugLog($"获取总兵力 - 是否玩家队伍：{isPlayerTeam}，玩家是否进攻方：{isPlayerAttacker}，当前队伍阵营：{battleSide}");
#endif

            // 返回缓存的初始总兵力
            if (battleSide == BattleSideEnum.Attacker)
            {
                return _initialAttackerTotalCount;
            }
            else if (battleSide == BattleSideEnum.Defender)
            {
                return _initialDefenderTotalCount;
            }

            return 0;
        }

        /// <summary>
        /// 获取队伍的伤亡数量
        /// </summary>
        private static int GetCasualtiesCount(Team team)
        {
            if (team == null || Mission.Current == null) return 0;
            
            int casualties = 0;
            
            var casualtyHandler = Mission.Current.GetMissionBehavior<CasualtyHandler>();
            if (casualtyHandler != null)
            {
                foreach (Formation formation in team.FormationsIncludingEmpty)
                {
                    if (formation != null)
                    {
                        casualties += casualtyHandler.GetCasualtyCountOfFormation(formation);
                    }
                }
#if DEBUG
                DebugLog($"队伍 {team.Side} 的伤亡数量：{casualties}");
#endif
            }
            else
            {
                casualties = team.QuerySystem.DeathCount;
#if DEBUG
                DebugLog($"使用QuerySystem获取队伍 {team.Side} 的死亡数量：{casualties}");
#endif
            }
            
            return casualties;
        }
    }
}
