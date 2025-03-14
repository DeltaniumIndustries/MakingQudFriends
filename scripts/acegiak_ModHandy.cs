using System;
using XRL.World.Anatomy;

namespace XRL.World.Parts
{
    [Serializable]
    public class acegiak_ModHandy : acegiak_ModBase
    {
        public acegiak_ModHandy() { }

        public acegiak_ModHandy(int Tier) : base(Tier) { }

        public override void ApplyModification(GameObject Object)
        {
            Body partBody = Object.GetPart<Body>();
            if (partBody != null)
            {
                BodyPart body2 = partBody.GetBody();
                BodyPart firstAttachedPart = body2.GetFirstAttachedPart("Back", 0, partBody, true);
                if (firstAttachedPart != null)
                {
                    AddHandPartsAt(firstAttachedPart, body2);
                }
                else
                {
                    AddHandParts(body2);
                }
            }
        }

        private void AddHandPartsAt(BodyPart firstAttachedPart, BodyPart body2)
        {
            body2.AddPartAt(firstAttachedPart, "Arm", 2).AddPart("Hand", 2, "MetalFist", "Hands");
            body2.AddPartAt(firstAttachedPart, "Arm", 1).AddPart("Hand", 1, "MetalFist", "Hands");
            body2.AddPartAt(firstAttachedPart, "Missile Weapon", 2);
            body2.AddPartAt(firstAttachedPart, "Missile Weapon", 1);
            body2.AddPartAt(firstAttachedPart, "Thrown Weapon");
            body2.AddPartAt(firstAttachedPart, "Hands", 0, null, null, "Hands");
        }

        private void AddHandParts(BodyPart body2)
        {
            body2.AddPart("Arm", 2).AddPart("Hand", 2, "MetalFist", "Hands");
            body2.AddPart("Arm", 1).AddPart("Hand", 1, "MetalFist", "Hands");
            body2.AddPart("Missile Weapon", 2);
            body2.AddPart("Missile Weapon", 1);
            body2.AddPart("Thrown Weapon");
            body2.AddPart("Hands", 0, null, null, "Hands");
        }

        protected override string GetShortDescriptionText()
        {
            return "\n&CHandy: This robot has been equipped with arms and hands for holding and manipulating things.";
        }

        protected override string GetDisplayNamePrefix()
        {
            return "handy ";
        }
    }
}
