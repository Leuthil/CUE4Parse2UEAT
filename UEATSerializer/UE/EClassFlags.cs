namespace UEATSerializer.UE
{
    [Flags]
    public enum EClassFlags : uint
    {
        CLASS_None                      = 0x00000000u,
        CLASS_Abstract                  = 0x00000001u,
        CLASS_DefaultConfig             = 0x00000002u,
        CLASS_Config                    = 0x00000004u,
        CLASS_Transient                 = 0x00000008u,
        CLASS_Parsed                    = 0x00000010u,
        CLASS_MatchedSerializers        = 0x00000020u,
        CLASS_ProjectUserConfig         = 0x00000040u,
        CLASS_Native                    = 0x00000080u,
        CLASS_NoExport                  = 0x00000100u,
        CLASS_NotPlaceable              = 0x00000200u,
        CLASS_PerObjectConfig           = 0x00000400u,
        CLASS_ReplicationDataIsSetUp    = 0x00000800u,
        CLASS_EditInlineNew             = 0x00001000u,
        CLASS_CollapseCategories        = 0x00002000u,
        CLASS_Interface                 = 0x00004000u,
        CLASS_CustomConstructor         = 0x00008000u,
        CLASS_Const                     = 0x00010000u,
        CLASS_LayoutChanging            = 0x00020000u,
        CLASS_CompiledFromBlueprint     = 0x00040000u,
        CLASS_MinimalAPI                = 0x00080000u,
        CLASS_RequiredAPI               = 0x00100000u,
        CLASS_DefaultToInstanced        = 0x00200000u,
        CLASS_TokenStreamAssembled      = 0x00400000u,
        CLASS_HasInstancedReference     = 0x00800000u,
        CLASS_Hidden                    = 0x01000000u,
        CLASS_Deprecated                = 0x02000000u,
        CLASS_HideDropDown              = 0x04000000u,
        CLASS_GlobalUserConfig          = 0x08000000u,
        CLASS_Intrinsic                 = 0x10000000u,
        CLASS_Constructed               = 0x20000000u,
        CLASS_ConfigDoNotCheckDefaults  = 0x40000000u,
        CLASS_NewerVersionExists        = 0x80000000u,

        CLASS_ShouldNeverBeLoaded       = (CLASS_Native | CLASS_Intrinsic | CLASS_TokenStreamAssembled)
    }
}
