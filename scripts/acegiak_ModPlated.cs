using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class acegiak_ModPlated : acegiak_ModBase
    {
        public acegiak_ModPlated() { }

        public acegiak_ModPlated(int Tier) : base(Tier) { }

        public override void ApplyModification(GameObject Object)
        {
            if (Object.Statistics.ContainsKey("Toughness"))
            {
                Object.Statistics["Toughness"].BaseValue += 4;
            }
            if (Object.Statistics.ContainsKey("AV"))
            {
                Object.Statistics["AV"].BaseValue += 3;
            }
        }

        protected override string GetShortDescriptionText()
        {
            return "\n&Kp&ylate&Kd&C: This robot has been covered in heavy plating, increasing its toughness and armor value.";
        }

        protected override string GetDisplayNamePrefix()
        {
            return "&Kp&ylate&Kd&C ";
        }
    }
}
