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

        [SettingPropertyInteger("{=IronBloodBattlefield_EnemyRetreatThreshold}Enemy Retreat Threshold",10, 500, "0",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_EnemyRetreatThreshold_Hint}Allows enemy soldiers to retreat when their number falls below this value",
            Order = 1)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public int EnemyRetreatThreshold { get; set; } = 100;

        [SettingPropertyInteger("{=IronBloodBattlefield_PlayerRetreatThreshold}Player Retreat Threshold",10, 500, "0",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_PlayerRetreatThreshold_Hint}Allows player soldiers to retreat when their number falls below this value",
            Order = 2)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public int PlayerRetreatThreshold { get; set; } = 50;

        [SettingPropertyInteger("{=IronBloodBattlefield_RetreatConfirmationCount}Buff Disappearance Time After Falling Below Threshold",1, 100, "0",
            RequireRestart = false,
            HintText = "{=IronBloodBattlefield_RetreatConfirmationCount_Hint}Buff disappearance time after falling below the set number. If no reinforcements arrive within this time, the buff disappears （Unit: Second）",
            Order = 3)]
        [SettingPropertyGroup("{=IronBloodBattlefield_RetreatThreshold_Group}Retreat Threshold Settings", GroupOrder = 2)]
        public int RetreatConfirmationCount { get; set; } = 20;

        public McmSettings()
        {
            GlobalEnable = true;
            PlayerEnable = true;
            EnemyRetreatThreshold = 100;
            PlayerRetreatThreshold = 50;
            RetreatConfirmationCount = 20;
        }
    }
}
