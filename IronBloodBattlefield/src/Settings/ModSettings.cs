namespace IronBloodBattlefield.Settings
{
    /// <summary>
    /// 设置
    /// </summary>
    public class SetSetValue<T>
    {
        public bool IsChanged { get; private set; }
        public T Value { get; private set; }

        public SetSetValue(bool isChanged, T value)
        {
            IsChanged = isChanged;
            Value = value;
        }
    }

    public static class ModSettings
    {
        /// <summary>
        /// 检查mod是否启用
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                return McmSettings.Instance?.GlobalEnable ?? false;
            }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public static SetSetValue<bool> IronBloodBattlefield
        {
            get
            {
                return new SetSetValue<bool>(true, IsEnabled);
            }
        }

        /// <summary>
        /// 敌人撤退阈值
        /// </summary>
        public static SetSetValue<int> EnemyRetreatThreshold
        {
            get
            {
                if (!IsEnabled) return new SetSetValue<int>(false, 0);
                return new SetSetValue<int>(true, McmSettings.Instance?.EnemyRetreatThreshold ?? 100);
            }
        }

        /// <summary>
        /// 玩家方撤退阈值
        /// </summary>
        public static SetSetValue<int> PlayerRetreatThreshold
        {
            get
            {
                if (!IsEnabled) return new SetSetValue<int>(false, 0);
                return new SetSetValue<int>(true, McmSettings.Instance?.PlayerRetreatThreshold ?? 50);
            }
        }

        /// <summary>
        /// 是否对玩家部队生效
        /// </summary>
        public static SetSetValue<bool> ApplyToPlayer
        {
            get
            {
                if (!IsEnabled) return new SetSetValue<bool>(false, false);
                return new SetSetValue<bool>(true, McmSettings.Instance?.PlayerEnable ?? false);
            }
        }

        /// <summary>
        /// 低于阈值后需要连续确认的次数
        /// </summary>
        public static SetSetValue<int> RetreatConfirmationCount
        {
            get
            {
                if (!IsEnabled) return new SetSetValue<int>(false, 0);
                return new SetSetValue<int>(true, McmSettings.Instance?.RetreatConfirmationCount ?? 20);
            }
        }
    }
} 