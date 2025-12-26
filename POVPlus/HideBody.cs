using System;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Hypostasis.Game.Structures;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using POVPlus.Windows;
using static FFXIVClientStructs.Havok.Animation.Rig.hkaPose;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

using StructsObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using System.Xml;

namespace POVPlus
{
    //Not sure if this is actualy required? Hypostatis SEEMS to be the thing that gives me access to the camera. - but I dont think it needs this injection. - what even IS an injection seems to work wtihout it
    //[HypostasisInjection]



    /// <summary>
    /// TODO
    /// 
    /// Camera->Mode
    /// can be used to detect if the user is in first person or not
    /// 
    /// PlayerBase->OnRenderModel
    /// Could be interesting?
    /// 
    /// 
    /// </summary>



    public static unsafe class HideBody
    {
        private static float CachedDefaultLookAtHeightOffset;
        private static GameObject* PrevCameraTarget;
        private static Vector3 PrevCameraTargetPosition;
        private static float InterpolatedHeight;

        private static float OffsetCalculatedX;
        private static float OffsetCalculatedZ;



        private static void GetCameraPositionDetour(GameCamera* camera, GameObject* target, Vector3* position, Bool swapPerson)
        {

            ///FIND OUT WHAT SWAP PERSON ACTUALY DOES?

            
           

            //Service.Log.Information($"=== PlayerBase === {camera->currentHRotation} / {target->Rotation} ===");



            //Log.Information($"CAMERA DETOUR");
            var newPos = Common.GetBoneWorldPosition(target, (uint)MagicNumbers.BoneToBind) + ((Vector3)target->Position - PrevCameraTargetPosition); ///26 Head 


            /// -------------------- Camera XYZ Offset ---------------------
            /// 



            float PlayerYaw = target->Rotation;

            float cosY = MathF.Cos(PlayerYaw);
            float sinY = MathF.Sin(PlayerYaw);

            Vector3 rotatedOffset = new Vector3(
                POVPlus.P.Configuration.OffsetX * sinY + POVPlus.P.Configuration.OffsetZ * cosY,
                POVPlus.P.Configuration.OffsetY,
                POVPlus.P.Configuration.OffsetX * cosY - POVPlus.P.Configuration.OffsetZ * sinY
            );

            Vector3 OffsetPos = new Vector3(POVPlus.P.Configuration.OffsetX + OffsetCalculatedX, POVPlus.P.Configuration.OffsetY, POVPlus.P.Configuration.OffsetZ + OffsetCalculatedZ);

            newPos = rotatedOffset + newPos;

            camera->minFoV = POVPlus.P.Configuration.FOV;

            // GETS THE PLAYER CHARACTER : Service.ClientState.LocalPlayer

            var player = Service.ClientState.LocalPlayer;
            if (player != null)
            {
                //if (player is ICharacter) { Service.Log.Information($"=== IS ICharacter === {player.Name} ==="); }

                var PlayerBase = GetCharacterBase(player);
                if (PlayerBase != null) 
                {
                    //Service.Log.Information($"=== PlayerBase === {PlayerBase->Skeleton->PartialSkeletonCount} ===");

                    if (PlayerBase->Skeleton != null)
                    {
                        var SkeletonTest = *PlayerBase->Skeleton;

                        var PartialSkeletonTest = SkeletonTest.PartialSkeletons;
                        if (PartialSkeletonTest != null)
                        {
                            // 0 Seems to be Character
                            var HavokPoseVal = 0;

                            var HavokBoneVal = 54;

                            var HavokBoneToBind = (int)MagicNumbers.BoneToBind;

                            var HavokBoneToBindX = (int)MagicNumbers.RotationBindBoneX;
                            var HavokBoneToBindZ = (int)MagicNumbers.RotationBindBoneZ;

                            if (PartialSkeletonTest != null && 1==1)
                            {
                                if (PartialSkeletonTest->GetHavokPose(HavokPoseVal) != null)
                                {
                                    /// ---------------------------------------------- The Skeleton has been found, Breaking down the details ----------------------------------------------

                                    //Gets the bone Name for the UI + Bone Counts
                                    if (1 == 1)
                                    {
                                        MagicNumbers.RotationBoneValueXName = PartialSkeletonTest->GetHavokPose(HavokPoseVal)->Skeleton->Bones[MagicNumbers.RotationBoneValueX].Name.String;
                                        MagicNumbers.RotationBoneValueZName = PartialSkeletonTest->GetHavokPose(HavokPoseVal)->Skeleton->Bones[MagicNumbers.RotationBoneValueZ].Name.String;
                                        MagicNumbers.PlayerBoneCount = PartialSkeletonTest->GetHavokPose(HavokPoseVal)->Skeleton->Bones.Length-1; //-1 needed due to length making 1=0 2=1
                                    }


                                    //X YAW ROTATION (Side to Side)
                                    if (1 == 1)
                                    {
                                        //Calculates the player CHARACTERS rotation - NO BONE LOGIC HERE
                                        //OUTPUT PlayerXRotationNormalize
                                        MagicNumbers.PlayerXRotationCurrent = target->Rotation;
                                        MagicNumbers.PlayerXRotationNormalize = MagicNumbers.PlayerXRotationCurrent - MagicNumbers.PlayerXRotationPrev;
                                        MagicNumbers.PlayerXRotationPrev = MagicNumbers.PlayerXRotationCurrent;












                                    }

                                    // YAW ROTATION (Up / Down)
                                    if (1 == 1)
                                    {



                                        //CALCULATE BONE YAW (SIDE SIDE)
                                        //OUTPUT BoneYawRotateOffset
                                        var BoneRotSource = PartialSkeletonTest->GetHavokPose(HavokPoseVal)->AccessBoneModelSpace(MagicNumbers.RotationBoneValueX, PropagateOrNot.DontPropagate);
                                        MagicNumbers.CameraQuartCurrent = new Quaternion(BoneRotSource->Rotation.X, BoneRotSource->Rotation.Y, BoneRotSource->Rotation.Z, BoneRotSource->Rotation.W);
                                        var NormalisedQuart = Quaternion.Normalize(MagicNumbers.CameraQuartCurrent);
                                        Vector3 forward = Vector3.Transform(Vector3.UnitZ, NormalisedQuart);
                                        MagicNumbers.BoneYawRotateCurrent = MathF.Atan2(forward.X, forward.Z);
                                        var BoneYawRotateOffset = MagicNumbers.BoneYawRotateCurrent - MagicNumbers.BoneYawRotateBefore;
                                        MagicNumbers.BoneYawRotateBefore = MathF.Atan2(forward.X, forward.Z);

                                        //CALCULATE BONE PITCH (UP DOWN)
                                        var sinPitch = 2 * (NormalisedQuart.W * NormalisedQuart.Z + NormalisedQuart.X * NormalisedQuart.Y);
                                        var cosPitch = 1 - 2 * (NormalisedQuart.Y * NormalisedQuart.Y + NormalisedQuart.Z * NormalisedQuart.Z);

                                        // Pitch in radians
                                        var pitchRad = MathF.Atan2(sinPitch, cosPitch);

                                        // Convert to degrees (this already gives [-180, +180])
                                        var pitchDeg = pitchRad * (180.0f / MathF.PI);

                                        var up = Vector3.Transform(Vector3.UnitY, NormalisedQuart);




                                        /// YAW STUFF
                                        /// 



                                        var forward3 = Vector3.Transform(Vector3.UnitZ, NormalisedQuart);

                                        float roll = MathF.Atan2(
                                        -forward.X,

                                        MathF.Sqrt(forward.X * forward.X + forward.Z * forward.Z));

                                        var FinalRotation = Matrix4x4.CreateFromQuaternion(NormalisedQuart);
                                        FinalRotation.M41 = BoneRotSource->Translation.X;
                                        FinalRotation.M42 = BoneRotSource->Translation.Y;
                                        FinalRotation.M43 = BoneRotSource->Translation.Z;

                                        var Finalforward = new Vector3(FinalRotation.M31, FinalRotation.M32, FinalRotation.M33); // Z-axis
                                        var Finalup = new Vector3(FinalRotation.M21, FinalRotation.M22, FinalRotation.M23); // Y-axis
                                        var Finalright = new Vector3(FinalRotation.M11, FinalRotation.M12, FinalRotation.M13);




                                        var V2Roll = MathF.Atan2(2.0f * (NormalisedQuart.X * NormalisedQuart.Y + NormalisedQuart.Z * NormalisedQuart.W), 1.0f - 2.0f * (NormalisedQuart.X * NormalisedQuart.X + NormalisedQuart.Z * NormalisedQuart.Z));


                                        //Service.Log.Information($"\n Bone Z rotation :  {V2Roll * (float)(180 / System.Math.PI)} \n Current Camera Value : {camera->currentVRotation} \n Raw Bone Z Value {V2Roll} \n {V2Roll+MathF.PI}");









                                        // Yaw (rotation around Y / left-right)
                                        var Finalyaw = MathF.Atan2(Finalforward.X, Finalforward.Z);

                                        // Pitch (rotation around X / up-down)
                                        // yaw-independent, so it doesn't change when turning left/right
                                        var Finalpitch = MathF.Atan2(
                                            -Finalforward.Y,
                                            MathF.Sqrt(Finalforward.X * Finalforward.X + Finalforward.Z * Finalforward.Z)
                                        );

                                        // Roll (rotation around Z / tilt)
                                        var Finalroll = MathF.Atan2(
                                            -Finalup.X * Finalforward.Y + Finalup.Y * Finalforward.X,
                                            Finalup.Z
                                        );


                                        //Service.Log.Information($"\n Finalyaw {Finalforward} \n Finalpitch {Finalpitch * (180f / MathF.PI)} \n Finalroll {(-Finalroll * (180f / MathF.PI))}");
                                        

                                        MagicNumbers.BonePitchRotateCurrent = -V2Roll + MathF.PI;

                                        //Service.Log.Information($"Current Raw {V2Roll} \n Current : {Configuration.BonePitchRotateCurrent} \n Previous : {(Configuration.BonePitchRotateBefore)} \n Offset Calculation {(Configuration.BonePitchRotateCurrent + 10f) - (Configuration.BonePitchRotateBefore + 10f)}");


                                        var BonePitchRotateOffset = (MagicNumbers.BonePitchRotateCurrent + 10f) - (MagicNumbers.BonePitchRotateBefore + 10f);
                                        MagicNumbers.BonePitchRotateBefore = MagicNumbers.BonePitchRotateCurrent;

                                        

                                        /// ----------------------------------- ROTATION / MOVEMENT RESTRICTIONS -----------------------------------


                                        /// ----------------------------------- YAW (LEFT RIGHT) -----------------------------------
                                        if (POVPlus.P.Configuration.RotationBindBoolX == true)
                                        {


                                            ///MY NIGHTMARE - YAW EDITION
                                            ///
                                            /// -- PART 1 --
                                            ///If the Camera Rotation yaw BEFORE any additional delta changes (How in the godless hell do i track that consistently) matches the Camera Rotation AND they are moving
                                            ///it is safe to assume that they are moving wtih /right mouse click held down
                                            ///
                                            /// -- PART 2 --
                                            /// Else they are probably just moving with WASD and we can progress as normal

                                            //Detects if the player is actively moving 

                                            var Ismoving = false;
                                            var RightClickMoving = false;

                                            MagicNumbers.CurrentPlayerPosition = target->Position;
                                            if (MagicNumbers.CurrentPlayerPosition != MagicNumbers.LastPlayerPosition)
                                            {
                                                Ismoving = true;
                                            }
                                            MagicNumbers.LastPlayerPosition = target->Position;

                                            if (Ismoving == true && camera->currentHRotation == target->Rotation)
                                            {
                                                MagicNumbers.RightClickMoving = true;
                                            }
                                            else
                                            {
                                                MagicNumbers.RightClickMoving = false;
                                            }

                                            //Checks for temporal issues (LAST FRAME NOT MATCHING WHEN TURNING - IDK WHY IT DOES IT BUT IT DO)


                                            // Current Camera Rotation
                                            var CurrentFrameHRotation = camera->currentHRotation;

                                            //Configuration.TotalRotationOffsetAddition = Configuration.TotalRotationOffsetAddition + BoneYawRotateOffset;
                                            //camera->currentHRotation = CurrentFrameHRotation;// + BoneYawRotateOffset;// + Configuration.PlayerXRotationNormalize;

                                            var ThisIsATest = CurrentFrameHRotation + BoneYawRotateOffset;

                                            var BeforeAnyChanges = ThisIsATest - BoneYawRotateOffset;




                                            MagicNumbers.TOTALYAW = MagicNumbers.TOTALYAW + BoneYawRotateOffset;

                                            MagicNumbers.TotalRotationOffsetAddition = MagicNumbers.TotalRotationOffsetAddition - BoneYawRotateOffset;

                                            //camera->hRotationDelta = BoneYawRotateOffset;

                                            //Service.Log.Information($" Total added offset : {Configuration.TotalRotationOffsetAddition} \n  Processed {camera->currentHRotation - Configuration.TotalRotationOffsetAddition} \n Camera Rotation (Current) {camera->currentHRotation}  \n Player Rotation (Current) {target->Rotation} ");
                                            //Service.Log.Information($"\nOffsetTotal {Configuration.TotalRotationOffsetAddition}  \n processed - offsettotal : {(camera->currentHRotation = camera->currentHRotation + BoneYawRotateOffset) - Configuration.TotalRotationOffsetAddition} \n Camera Rotation {camera->currentHRotation} \n Player Rotation {target->Rotation}");


                                            if (MagicNumbers.RightClickMoving == false)
                                            {

                                                if (MagicNumbers.TemporalYawPrevious == target->Rotation && Ismoving == true)
                                                {
                                                    //Service.Log.Information($" FIX PROCCED ");

                                                    MagicNumbers.RightClickMoving = true;

                                                }

                                                if (MagicNumbers.TemporalYawPrevious == target->Rotation && Ismoving == false)
                                                {
                                                    //Service.Log.Information($" Turning stationary ");

                                                    MagicNumbers.StationaryRotateRight = true;

                                                }
                                                else
                                                {
                                                    MagicNumbers.StationaryRotateRight = false;
                                                }


                                                var loggingoff = 1;
                                                if (loggingoff == 0)
                                                {
                                                    Service.Log.Information($"" +
                                                        $" First Call : {camera->currentHRotation}" +   // There is 'Temporal Distortion' - we need a check if the Current player Rotation = the last Camera Rotation
                                                                                                        // $"\n Adding Value ? {camera->currentHRotation = camera->currentHRotation + 0.412412f}" +
                                                                                                        // $"\n Second Call {camera->currentHRotation} " +
                                                                                                        // $"\n Minus Value {camera->currentHRotation = camera->currentHRotation - 0.412412f} " +
                                                                                                        // $"\n Final Call {camera->currentHRotation}" +
                                                        $"\n Player Rotation {target->Rotation}" +
                                                        $"\n Player Location {MagicNumbers.CurrentPlayerPosition}" +
                                                        $"\n Is moving? {Ismoving}" +
                                                        $"\n Right Click Moving? {MagicNumbers.RightClickMoving}" +
                                                        $"\n PREV VALUE {MagicNumbers.TemporalYawPrevious} ");
                                                }

                                                MagicNumbers.TemporalYawPrevious = camera->currentHRotation;


                                                //Configuration.TemporalYawPrevious = target->Rotation;

                                                //Service.Log.Information($"Current Rotation : {camera->currentHRotation} \n My Value {camera->currentHRotation + BoneYawRotateOffset + Configuration.PlayerXRotationNormalize} ");

                                                //rBase === {camera->currentHRotation} / {target->Rotation} ===");

                                            }







                                            //Rotates with Right Mouse
                                            if (MagicNumbers.RightClickMoving == true || MagicNumbers.StationaryRotateRight == true || 1==2)
                                            {

                                            }
                                            //Rotates with keyboard
                                            else
                                            {


                                            }


                                        }

                                        //JUST COMPRESSING EVERYTHING
                                        float finalXrotation=0f;

                                        if (POVPlus.P.Configuration.Setting_RotateWithplayer == true)
                                        {
                                            finalXrotation = finalXrotation + MagicNumbers.PlayerXRotationNormalize;
                                        }

                                        if (POVPlus.P.Configuration.RotationBindBoolX == true)
                                        {
                                            finalXrotation = finalXrotation + BoneYawRotateOffset;
                                        }

                                        camera->currentHRotation = camera->currentHRotation + finalXrotation;

                                         //   if (Configuration.Setting_RotateWithplayer == false)
                                       // {
                                        //    Configuration.PreviousRotationBeforeChanges = camera->currentHRotation;
                                            //camera->currentHRotation = CurrentFrameHRotation + BoneYawRotateOffset + Configuration.PlayerXRotationNormalize;
                                        //    camera->currentHRotation = camera->currentHRotation
                                                //+  Configuration.PlayerXRotationNormalize ///Player total Rotation
                                        //        + BoneYawRotateOffset;
                                        //}
                                        //else
                                        //{
                                        //    Configuration.PreviousRotationBeforeChanges = camera->currentHRotation;
                                            //camera->currentHRotation = CurrentFrameHRotation + BoneYawRotateOffset + Configuration.PlayerXRotationNormalize;
                                        //    camera->currentHRotation = camera->currentHRotation
                                        //        + Configuration.PlayerXRotationNormalize ///Player total Rotation
                                        //        + BoneYawRotateOffset;
                                        ///}

                                        // ----------------------------------- PITCH (UP DOWN) -----------------------------------

                                        if (POVPlus.P.Configuration.RotationBindBoolZ == false)
                                        {
                                            Common.CameraManager->worldCamera->maxVRotation = MagicNumbers.PreviousMaxVRotation;
                                            Common.CameraManager->worldCamera->minVRotation = MagicNumbers.PreviousMinVRotation;
                                            Common.CameraManager->worldCamera->tilt = MagicNumbers.PreviousTilt;
                                        }
                                        
                                        if (POVPlus.P.Configuration.RotationBindBoolZ == true)
                                        {




                                            var TestPitch = camera->currentVRotation + (BonePitchRotateOffset);
                                            //MiN Default = -1.4835298      Max Default = 0.7853982

                                            camera->minVRotation = -90;
                                            camera->maxVRotation = 90;

                                            //camera->currentVRotation = (camera->currentVRotation + (BonePitchRotateOffset));


                                            // CAMERA INVERSION TRYHARDERY FOR SPINNY PITCH :D
                                            var InversionCalculation = (TestPitch * (180f / Math.PI)) + 90;
                                            InversionCalculation = (Math.Floor(InversionCalculation / 180f) % 2 == 0) ? 1 : 0;
                                            if (InversionCalculation == 0)
                                            {
                                                camera->tilt = 3.14159f;
                                            }
                                            else
                                            {
                                                camera->tilt = 0;
                                            }
                                            //Bone Only Info



                                            var forward2 = Vector3.Transform(Vector3.UnitZ, NormalisedQuart);

                                            float yaw = MathF.Atan2(forward.X, forward.Z) * (180f / MathF.PI);

                                            float pitch = MathF.Asin(forward.Y) * (180f / MathF.PI);


                                            //BELOW WORKS WITH CAMERA BINDING
                                            //camera->currentVRotation =  -V2Roll + MathF.PI;

                                            camera->currentVRotation = camera->currentVRotation + BonePitchRotateOffset;

                                        }


                                        /// -------------------- FINAL CAMERA --------------------
                                        /// Fixes the weird camera issues with PrevCameraTargetPosition


                                        var CameraLocationTest = PartialSkeletonTest->GetHavokPose(HavokPoseVal)->AccessBoneModelSpace(MagicNumbers.RotationBoneValueX, PropagateOrNot.DontPropagate);

                                        var CameraLocationTest2 = CameraLocationTest->Translation;

                                        //Service.Log.Information($"X {CameraLocationTest2.X} " +
                                        //    $"\n Y {CameraLocationTest2.Y}" +
                                        //    $"\n Z {CameraLocationTest2.Z}" +
                                        //    $"\n W {CameraLocationTest2.W}" +
                                        //    $" ");










                                    }
                                }
                            }
                        }
                    }
                }
            }

            /// -------------------- FINAL CAMERA --------------------
            /// Fixes the weird camera issues with PrevCameraTargetPosition

            PrevCameraTargetPosition = target->Position;
            *position = newPos;

        }



        public static void Initialize()
        {
            if (Common.CameraManager == null || !Common.IsValid(Common.CameraManager->worldCamera) || !Common.IsValid(Common.InputData))
                throw new ApplicationException("Failed to validate core structures!");

            var vtbl = Common.CameraManager->worldCamera->VTable;
            vtbl.getCameraPosition.CreateHook(GetCameraPositionDetour);

            Service.Log.Information($"===CALLED INIT===");

            MagicNumbers.PreviousMaxVRotation = Common.CameraManager->worldCamera->maxVRotation;
            MagicNumbers.PreviousMinVRotation = Common.CameraManager->worldCamera->minVRotation;
            MagicNumbers.PreviousCurrentFoV = Common.CameraManager->worldCamera->currentFoV;
            MagicNumbers.PreviousTilt = Common.CameraManager->worldCamera->tilt;
            MagicNumbers.PreviousMinFOV = Common.CameraManager->worldCamera->minFoV;


        }
        public static void Dispose()
        {
            //Cammy DOESNT do this but it stops the errors appearing? 
            Common.CameraManager->worldCamera->VTable.getCameraPosition.Hook.Disable();
            Common.CameraManager->worldCamera->VTable.getCameraPosition.Hook.Dispose();

            //Revert the chaos to what it was before the plugin was innitialised
            Common.CameraManager->worldCamera->currentVRotation = 0;
            Common.CameraManager->worldCamera->currentVRotation = 0;
            Common.CameraManager->worldCamera->maxVRotation = MagicNumbers.PreviousMaxVRotation;
            Common.CameraManager->worldCamera->minVRotation = MagicNumbers.PreviousMinVRotation;
            Common.CameraManager->worldCamera->currentFoV = MagicNumbers.PreviousCurrentFoV;
            Common.CameraManager->worldCamera->tilt = MagicNumbers.PreviousTilt;
            Common.CameraManager->worldCamera->minFoV = MagicNumbers.PreviousMinFOV;

            Service.Log.Information($"===CLOSING===");

        }



        public static unsafe T* GetDrawObject<T>(this IGameObject go) where T : unmanaged
        {
            //return (T*)go.Native()->DrawObject;

            return (T*)((StructsObject*)go.Address)->DrawObject;
        }


        public static unsafe CharacterBase* GetCharacterBase(this ICharacter go) => go.GetDrawObject<CharacterBase>();



    }


}


