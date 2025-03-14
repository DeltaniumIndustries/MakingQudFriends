using System;
using XRL.World.Anatomy;

namespace XRL.World.Parts
{
    [Serializable]
    public class acegiak_ModTreaded : acegiak_ModBase
    {
        public acegiak_ModTreaded() { }

        public acegiak_ModTreaded(int Tier) : base(Tier) { }

        public override void ApplyModification(GameObject Object)
        {
            Body partBody = Object.GetPart<Body>();
            if (partBody != null)
            {
                BodyPart body2 = partBody.GetBody();
                if (body2 != null)
                {
                    body2.AddPart("Tread", 2);
                    body2.AddPart("Tread", 1);
                }
            }
        }

        protected override string GetShortDescriptionText()
        {
            return "\n&CTreaded: This robot has been equipped with treads allowing them to carry heavy loads.";
        }

        protected override string GetDisplayNamePrefix()
        {
            return "treaded ";
        }
    }
}
