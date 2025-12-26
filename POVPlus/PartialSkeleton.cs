using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using static FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton;


namespace POVPlus
{
    public unsafe class PartialSkeletonn(Skeleton skeleton, int id)
    {
        public int Id { get; } = id;

        public Skeleton Skeleton { get; } = skeleton;

        public List<nint> Poses { get; set; } = [];

        private readonly Dictionary<int, Bone> parambones = [];

        public List<Bone> RootBones { get; set; } = [];



        public Bone? GetBone(int index)
        {
            if (parambones.TryGetValue(index, out var bone))
                return bone;

            return null;
        }

    }
}
