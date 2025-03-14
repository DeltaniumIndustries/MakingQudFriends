using System;
using System.Text;
using XRL.Core;

namespace XRL.World.Parts
{
    [Serializable]
    public class acegiak_ModAwoken : IModification
    {
        private bool boot = false;

        public acegiak_ModAwoken() { }

        public acegiak_ModAwoken(int Tier) : base(Tier) { }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            var zombablePart = Object.GetPart<acegiak_Zombable>();
            return zombablePart?.Body?._Body != null;
        }

        public override void ApplyModification(GameObject Object)
        {
            InitializeStatistics(Object);
            ReanimateBody(Object);
            AddNewParts(Object);
            ConfigureRender(Object);
        }

        private void InitializeStatistics(GameObject Object)
        {
            var statValues = new (string stat, int min, int max, int value)[]
            {
                ("Energy", -100000, 100000, 0),
                ("Speed", 1, 100000, 100),
                ("MoveSpeed", -200, 200, 100),
                ("Hitpoints", 0, 64000, 16),
                ("AV", 0, 100, 0),
                ("DV", -100, 100, 0),
                ("Agility", 1, 9000, 16),
                ("Strength", 1, 9000, 16),
                ("Toughness", 1, 9000, 16),
                ("Wisdom", 1, 9000, 16),
                ("Ego", 1, 9000, 16),
                ("Intelligence", 1, 9000, 16),
                ("SP", 0, 2147483647, 0),
                ("MP", 0, 2147483647, 0),
                ("AP", 0, 2147483647, 0),
                ("MA", -100, 2147483647, 0),
                ("Level", 1, 10000, 1),
                ("XP", 0, 2147483647, 0),
                ("XPValue", 0, 2147483647, 0),
                ("HeatResistance", -100, 100, 0),
                ("ColdResistance", -100, 100, 0),
                ("ElectricalResistance", -100, 100, 0),
                ("AcidResistance", -100, 100, 0)
            };

            foreach (var (stat, min, max, value) in statValues)
            {
                Object.Statistics[stat] = new Statistic(stat, min, max, value, Object);
            }
        }

        private void ReanimateBody(GameObject Object)
        {
            var zomb = Object.GetPart<acegiak_Zombable>();
            Body newBody = new Body();
            newBody._Body = acegiak_Zombable.BodyPartCopy(zomb.Body._Body, Object, newBody);
            Object.AddPart(newBody);
        }

        private void AddNewParts(GameObject Object)
        {
            Object.AddPart(new Brain());
            Object.AddPart(new ConversationScript("acegiak_Zomber"));
            Object.AddPart(new Inventory());
            Object.AddPart(new Leveler());
            Object.AddPart(new Mutations());
            Object.AddPart(new Skills());
            Object.AddPart(new ActivatedAbilities());
            Object.AddPart(new RandomLoot());
        }

        private void ConfigureRender(GameObject Object)
        {
            var zomb = Object.GetPart<acegiak_Zombable>();
            var render = Object.GetPart<Render>();
            render.Tile = zomb.storedTile;
            render.TileColor = "y";
            render.DetailColor = "r";
        }

        public override bool SameAs(IPart p) => base.SameAs(p);

        public override bool AllowStaticRegistration() => true;

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Object.RegisterPartEvent(this, "GetDisplayName");
            Object.RegisterPartEvent(this, "GetShortDescription");
            Object.RegisterPartEvent(this, "GetShortDisplayName");
            Object.RegisterPartEvent(this, "Dropped");
            Object.RegisterPartEvent(this, "EnteredCell");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            switch (E.ID)
            {
                case "Dropped":
                    boot = true;
                    break;

                case "EnteredCell" when boot:
                    boot = false;
                    ReequipAndSetCompanion();
                    break;

                case "GetShortDescription":
                    E.SetParameter("Postfix", E.GetStringParameter("Postfix") + "\n&CAwoken: This corpse has been reanimated with cybernetic technologies.");
                    break;

                case "GetDisplayName":
                case "GetShortDisplayName":
                    if (!ParentObject.Understood() || !ParentObject.HasProperName)
                    {
                        E.GetParameter<StringBuilder>("Prefix").Append("awoken ");
                    }
                    break;
            }

            return base.FireEvent(E);
        }

        private void ReequipAndSetCompanion()
        {
            var brain = ParentObject.GetPart<Brain>();
            if (brain == null) return;

            brain.PerformReequip();
            brain.FactionFeelings.Clear();
            brain.BecomeCompanionOf(ParentObject.ThePlayer);
            brain.IsLedBy(ParentObject.ThePlayer);
            brain.SetPartyLeader(ParentObject.ThePlayer);
            brain.Goals.Clear();
            brain.Allegiance.Calm = false;
            brain.Hibernating = false;

            ParentObject.AddPart(new Combat());
            XRLCore.Core.Game.ActionManager.AddActiveObject(ParentObject);
        }
    }
}
