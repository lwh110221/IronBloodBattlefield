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
        /// 是否使用百分比模式
        /// </summary>
        public static SetSetValue<bool> UsePercentageMode
        {
            get
            {
                if (!IsEnabled) return new SetSetValue<bool>(false, false);
                return new SetSetValue<bool>(true, McmSettings.Instance?.UsePercentageMode ?? false);
            }
        }

        /// <summary>
        /// 敌方撤退损失百分比
        /// </summary>
        public static SetSetValue<float> EnemyRetreatPercentage
        {
            get
            {
                if (!IsEnabled || !UsePercentageMode.Value) return new SetSetValue<float>(false, 0f);
                return new SetSetValue<float>(true, McmSettings.Instance?.EnemyRetreatPercentage ?? 30.0f);
            }
        }

        /// <summary>
        /// 玩家方撤退损失百分比
        /// </summary>
        public static SetSetValue<float> PlayerRetreatPercentage
        {
            get
            {
                if (!IsEnabled || !UsePercentageMode.Value) return new SetSetValue<float>(false, 0f);
                return new SetSetValue<float>(true, McmSettings.Instance?.PlayerRetreatPercentage ?? 30.0f);
            }
        }

        /// <summary>
        /// 敌人撤退阈值
        /// </summary>
        public static SetSetValue<int> EnemyRetreatThreshold
        {
            get
            {
                if (!IsEnabled || UsePercentageMode.Value) return new SetSetValue<int>(false, 0);
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
                if (!IsEnabled || UsePercentageMode.Value) return new SetSetValue<int>(false, 0);
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