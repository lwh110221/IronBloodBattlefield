using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace IronBloodBattlefield.Settings{
    public class McmSettings : AttributeGlobalSettings<McmSettings>
    {
        public override string Id => "IronBloodBattlefield_v1";
        public override string DisplayName => new TextObject("{=IronBloodBattlefield_DisplayName}IronBloodBattlefield - Ahao221").ToString();
        public override string FolderName => "IronBloodBattlefield";
        public override string FormatType => "json";

        
        public new static McmSettings Instance => AttributeGlobalSettings<McmSettings>.Instance;

        [SettingPropertyBool("{=IronBloodBattlefield_Enable}Enable IronBloodBattlefield",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_Enable_Hint}Enable IronBloodBattlefield. When enabled, soldiers will not retreat randomly. Tip: Does not affect Siege Battles",
            Order = 1)]
        [SettingPropertyGroup("{=IronBloodBattlefield_Enable_Group}Feature Settings", GroupOrder = 1)]
        public bool GlobalEnable { get; set; } = true;

        [SettingPropertyBool("{=IronBloodBattlefield_PlayerEnable}Affect Player Side",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_PlayerEnable_Hint}Whether it affects the player side",
            Order = 2)]
        [SettingPropertyGroup("{=IronBloodBattlefield_Enable_Group}Feature Settings", GroupOrder = 1)]
        public bool PlayerEnable { get; set; } = true;

        [SettingPropertyBool("{=IronBloodBattlefield_UsePercentageMode}Use Percentage Mode",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_UsePercentageMode_Hint}When enabled, retreat will be based on casualty percentage instead of fixed numbers",
            Order = 3)]
        [SettingPropertyGroup("{=IronBloodBattlefield_Enable_Group}Feature Settings", GroupOrder = 1)]
        public bool UsePercentageMode { get; set; } = false;

        [SettingPropertyFloatingInteger("{=IronBloodBattlefield_EnemyRetreatPercentage}Enemy Retreat Percentage", 5.0f, 100.0f, "0.0 '%'",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_EnemyRetreatPercentage_Hint}Enemy will retreat when casualties reach this percentage",
            Order = 1)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public float EnemyRetreatPercentage { get; set; } = 50.0f;

        [SettingPropertyFloatingInteger("{=IronBloodBattlefield_PlayerRetreatPercentage}Player Retreat Percentage", 5.0f, 100.0f, "0.0 '%'",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_PlayerRetreatPercentage_Hint}Player troops will retreat when casualties reach this percentage",
            Order = 2)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public float PlayerRetreatPercentage { get; set; } = 50.0f;

        [SettingPropertyInteger("{=IronBloodBattlefield_EnemyRetreatThreshold}Enemy Retreat Threshold",10, 500, "0",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_EnemyRetreatThreshold_Hint}Allows enemy soldiers to retreat when their number falls below this value",
            Order = 3)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public int EnemyRetreatThreshold { get; set; } = 100;

        [SettingPropertyInteger("{=IronBloodBattlefield_PlayerRetreatThreshold}Player Retreat Threshold",10, 500, "0",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_PlayerRetreatThreshold_Hint}Allows player soldiers to retreat when their number falls below this value",
            Order = 4)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public int PlayerRetreatThreshold { get; set; } = 50;

        [SettingPropertyInteger("{=IronBloodBattlefield_RetreatConfirmationCount}Buff Disappearance Time After Falling Below Threshold",1, 100, "0",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_RetreatConfirmationCount_Hint}Buff disappearance time after falling below the set number. If no reinforcements arrive within this time, the buff disappears （Unit: Second）",
            Order = 5)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public int RetreatConfirmationCount { get; set; } = 20;

        public McmSettings()
        {
            GlobalEnable = true;
            PlayerEnable = true;
            UsePercentageMode = false;
            EnemyRetreatPercentage = 50.0f;
            PlayerRetreatPercentage = 50.0f;
            EnemyRetreatThreshold = 100;
            PlayerRetreatThreshold = 50;
            RetreatConfirmationCount = 20;
        }
    }
}
