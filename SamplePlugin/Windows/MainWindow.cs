using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;
using Serilog;
using static Dalamud.Interface.Utility.Raii.ImRaii;
using static FFXIVClientStructs.FFXIV.Client.UI.RaptureAtkHistory.Delegates;
using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Lumina.Excel.Sheets.Experimental;
using SamplePlugin;
using System.Runtime.InteropServices;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly string goatImagePath;
    private readonly Plugin plugin;

    public static float arrowOffset = 0.75f;


    // We give this window a hidden ID using ##.
    // The user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin, string goatImagePath)
        : base("POV+ Alpha 0.1.0", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.goatImagePath = goatImagePath;
        this.plugin = plugin;
    }

    private static void ResetSliderFloat(string id, ref float val, float min, float max, float reset, string format)
    {
        var save = true;


        ///This is the Reset button
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.UndoAlt.ToIconString()}##{id}"))
        {
            val = reset;
            save = true;
            Service.Log.Information($"===UI===");
        }
        ImGui.PopFont();

        ///This is the Slider
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 160 * ImGuiHelpers.GlobalScale);
        save |= ImGui.SliderFloat(id, ref val, min, max, format);

        if (!save) return;
        //Service.Log.Information($"==={val}===");
        //    if (CurrentPreset == PresetManager.CurrentPreset)
        //        CurrentPreset.Apply();
    }

    private static void ResetSliderInt(string id, ref float val, float min, float max, float reset, string format)
    {
        var save = true;


        ///This is the Reset button
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.CalendarMinus.ToIconString()}##{id}"))
        {
            val --;
            save = true;
            Service.Log.Information($"===UI===");
        }
        ImGui.PopFont();

        ///This is the Slider
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 160 * ImGuiHelpers.GlobalScale);
        val = (int)val;
        save |= ImGui.SliderFloat(id, ref val, min, max, format);

        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.CalendarPlus.ToIconString()}##{id}"))
        {
            val ++;
            save = true;
            Service.Log.Information($"===UI===");
        }
        ImGui.PopFont();

        if (!save) return;
        //Service.Log.Information($"==={val}===");
        //    if (CurrentPreset == PresetManager.CurrentPreset)
        //        CurrentPreset.Apply();

    }

    private static void ControlledSliderInt(string id, ref int val, int min, int max, int reset)
    {
        var save = true;


        if (ImGui.Button("-"))
        {
            if (val != min)
            {
                val -= 1;
            }
        }

        ImGui.SameLine();

        if (ImGui.Button("+"))
        {
            if (val != max)
            {
                val += 1;
            }
        }

        ImGui.SameLine();

        ImGui.SliderInt(id, ref val, min,max );

        

    }



    public void Dispose() { }

    //TEST VARIABLES
    public bool MyTest { get; set; } = false;

    public override void Draw()
    {




        ImGui.TextUnformatted($" While using this mod I reccomend doing the following \n Character Configuration>General>1st Person Camera Auto-adjustment - Set this to Never" +
            $"\n This is not essential but it prevents the overwriting of the camera X and Z rotation that the First Person Auto Adjustment overwrites");
        ImGui.Dummy(new Vector2(0, 20));

        ImGui.TextUnformatted($"Camera Offset:");

        ImGui.Spacing();
        ResetSliderFloat("FOV", ref arrowOffset, 0, 5, 0.75f, "%.2f");

        ImGui.Spacing();
        ResetSliderFloat("X Offset (Forwards/Back)", ref Configuration.OffsetX, -1, 1, 0f, "%1f");

        ImGui.Spacing();
        ResetSliderFloat("Y Offset (Up Down)", ref Configuration.OffsetY, -1, 1, 0f, "%1f");

        ImGui.Spacing();
        ResetSliderFloat("Z Offset (Left Right)", ref Configuration.OffsetZ, -1, 1, 0f, "%1f");

        ImGui.Spacing();
        ImGui.Dummy(new Vector2(0, 20));

        ImGui.TextUnformatted($"EXPERIMENTAL BELOW - Causes rotation issues when moving while holding right mouse button");

        ImGui.Spacing();

        ImGui.Checkbox("Bind Camera Rotation X (Left Right) to the Head Bone", ref Configuration.RotationBindBoolX);
        ImGui.Spacing();

        ImGui.Checkbox("Bind Camera Rotation Z (Up Down) to the Head Bone - (this also allows you to rotate the camera 360 degrees up and down , just to account for backflips flips etc)", ref Configuration.RotationBindBoolZ);


        ImGui.Spacing();
        ImGui.Dummy(new Vector2(0, 20));

        ImGui.TextUnformatted($"VERY BROKEN BELOW - Fine if you move with just the keyboard");


        ImGui.Checkbox("Camera Rotates when player rotates (emulates 1st person camera auto adjustment when moving - but different) VERY GLITCHY WHEN MOVING/ROTATING WITH RIGHT MOUSE CLICK", ref Configuration.Setting_RotateWithplayer);


        //Disable all my godless testing BS
        if (1 == 2)
        {

            ImGui.TextUnformatted($"\nCamera Bone:\n1-POV / 26 - Head");

            ImGui.Spacing();


            ImGui.SliderInt("", ref Configuration.BoneToBind, 0, 300);


            ImGui.Dummy(new Vector2(0, 20));
            ImGui.Separator();
            ImGui.TextUnformatted($"Camera Rotation Bindings\nJ_te_r - hand right - 65 \nJ_te_l - hand left - 64 \nJ_kao - head - 46 \n");

            //Bind Rotation X
            ImGui.TextUnformatted($"\n Camera X Rotation Bone Binding : ");
            ImGui.Spacing();
            ImGui.Checkbox("Bind Camera Rotation X (Left Right) to Bone", ref Configuration.RotationBindBoolX);
            ImGui.Spacing();
            var XBoneName = Configuration.RotationBoneValueXName;
            ImGui.TextUnformatted($"Bone: {XBoneName}");
            ImGui.Spacing();
            ControlledSliderInt("Bone Ref X", ref Configuration.RotationBoneValueX, 0, Configuration.PlayerBoneCount, 1);
            ImGui.Spacing();
            ImGui.DragFloat("Bone X Rotation Offset", ref Configuration.RotationBindOffsetX, 0.01f, -3.6f, 3.6f, "%.3f");
            ImGui.Spacing();
            ImGui.Spacing();

            //Bind Rotation Z
            ImGui.TextUnformatted($"\n Camera Z Rotation Bone Binding : ");
            ImGui.Spacing();
            ImGui.Checkbox("Bind Camera Rotation Z (Up Down) to Bone", ref Configuration.RotationBindBoolZ);
            ImGui.Spacing();
            var ZBoneName = Configuration.RotationBoneValueZName;
            ImGui.TextUnformatted($"Bone: {ZBoneName}");
            ImGui.SliderInt("Bone Ref Z", ref Configuration.RotationBoneValueZ, 0, 300);
            ImGui.Spacing();
            ImGui.DragFloat("Bone Z Rotation Offset", ref Configuration.RotationBindOffsetZ, 0.01f, -3.6f, 3.6f, "%.3f");
            ImGui.Spacing();
            ImGui.Spacing();
            //camera->minFoV = 1;
            //camera->maxFoV = 2;


            // Button Test

            ImGui.Spacing();

            // Normally a BeginChild() would have to be followed by an unconditional EndChild(),
            // ImRaii takes care of this after the scope ends.
            // This works for all ImGui functions that require specific handling, examples are BeginTable() or Indent().
        }
    }
}
