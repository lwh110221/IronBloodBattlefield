using TaleWorlds.MountAndBlade;

namespace IronBloodBattlefield.Util
{
    /// <summary>
    /// 战斗类型检查工具类
    /// </summary>
    public static class BattleCheck
    {
        /// <summary>
        /// 检查当前战斗是否为野战（非攻城战）
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>如果是野战返回true，否则返回false</returns>
        public static bool IsFieldBattle(Mission mission)
        {
            if (mission == null) return false;
            
            // 直接判断是否为野战
            return mission.IsFieldBattle;
        }

        /// <summary>
        /// 检查当前战斗是否为攻城战（包括攻城战和出城战）
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>如果是攻城战相关返回true，否则返回false</returns>
        public static bool IsSiegeBattle(Mission mission)
        {
            if (mission == null) return false;
            
            // 判断是否为攻城战或出城战
            return mission.IsSiegeBattle || mission.IsSallyOutBattle;
        }

        /// <summary>
        /// 检查当前战斗是否为非攻城战斗
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>如果是非攻城战斗返回true，否则返回false</returns>
        public static bool IsNonSiegeBattle(Mission mission)
        {
            if (mission == null) return false;

            // 如果不是攻城战也不是出城战，就是野战
            return !IsSiegeBattle(mission);
        }

        /// <summary>
        /// 检查当前战斗是否已初始化
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>如果战斗已初始化返回true，否则返回false</returns>
        public static bool IsBattleInitialized(Mission mission)
        {
            if (mission == null) return false;
            
            return mission.HasSpawnPath;
        }

        /// <summary>
        /// 综合检查当前战斗是否为有效的野战
        /// 多个条件严格判断是否为非攻城战斗场景
        /// </summary>
        /// <param name="mission">当前任务实例</param>
        /// <returns>
        /// 如果满足以下所有条件则返回true：
        /// 1. 战斗已经初始化
        /// 2. 明确是野战（IsFieldBattle为true）
        /// 3. 明确不是攻城战或出城战
        /// 否则返回false
        /// </returns>
        public static bool IsValidFieldBattle(Mission mission)
        {
            if (mission == null) return false;
            if (!IsBattleInitialized(mission)) return false;
            if (!IsFieldBattle(mission)) return false;
            if (!IsNonSiegeBattle(mission)) return false;
            return true;
        }
    }
}
