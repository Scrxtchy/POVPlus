using Dalamud.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Intrinsics.Arm;


namespace POVPlus;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public float FOV = 0.75f;

    public float OffsetX  = 0;
    public float OffsetY = 0;
    public float OffsetZ = 0;


    public bool RotationBindBoolX  = false;
    public bool RotationBindBoolZ  = false;
    public bool Setting_RotateWithplayer  = false;


   


    // The below exist just to make saving less cumbersome
    public void Save()
    {
        Service.PluginInterface.SavePluginConfig(this);
    }
}
