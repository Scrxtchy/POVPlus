using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;


namespace POVPlus.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly string goatImagePath;
    private readonly POVPlus plugin;

    //public static float arrowOffset = 0.75f;


    // We give this window a hidden ID using ##.
    // The user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(POVPlus plugin, string goatImagePath)
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

    private static bool ResetSliderFloat(string id, ref float val, float min, float max, float reset, string format)
    {
        var save = false;


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
        save = ImGui.SliderFloat(id, ref val, min, max, format);

        return save;
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
        if (ResetSliderFloat("FOV", ref plugin.Configuration.FOV, 0, 5, 0.75f, "%.2f"))
            plugin.Configuration.Save();

        ImGui.Spacing();
        if (ResetSliderFloat("X Offset (Forwards/Back)", ref plugin.Configuration.OffsetX, -1, 1, 0f, "%1f"))
            plugin.Configuration.Save();

        ImGui.Spacing();
        if(ResetSliderFloat("Y Offset (Up Down)", ref plugin.Configuration.OffsetY, -1, 1, 0f, "%1f"))
            plugin.Configuration.Save();

        ImGui.Spacing();
        if(ResetSliderFloat("Z Offset (Left Right)", ref plugin.Configuration.OffsetZ, -1, 1, 0f, "%1f"))
            plugin.Configuration.Save();

        ImGui.Spacing();
        ImGui.Dummy(new Vector2(0, 20));

        ImGui.TextUnformatted($"EXPERIMENTAL BELOW - Causes rotation issues when moving while holding right mouse button");

        ImGui.Spacing();

        if(ImGui.Checkbox("Bind Camera Rotation X (Left Right) to the Head Bone", ref plugin.Configuration.RotationBindBoolX))
            plugin.Configuration.Save();
        ImGui.Spacing();

        if(ImGui.Checkbox("Bind Camera Rotation Z (Up Down) to the Head Bone - (this also allows you to rotate the camera 360 degrees up and down , just to account for backflips flips etc)", ref plugin.Configuration.RotationBindBoolZ))
            plugin.Configuration.Save();


        ImGui.Spacing();
        ImGui.Dummy(new Vector2(0, 20));

        ImGui.TextUnformatted($"VERY BROKEN BELOW - Fine if you move with just the keyboard");


        if(ImGui.Checkbox("Camera Rotates when player rotates (emulates 1st person camera auto adjustment when moving - but different) VERY GLITCHY WHEN MOVING/ROTATING WITH RIGHT MOUSE CLICK", ref plugin.Configuration.Setting_RotateWithplayer))
            plugin.Configuration.Save();

    }
}
