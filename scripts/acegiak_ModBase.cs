using System;
using System.Text;
using XRL.World.Anatomy;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class acegiak_ModBase : IModification
    {
        public acegiak_ModBase() { }

        public acegiak_ModBase(int Tier) : base(Tier) { }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Object.RegisterPartEvent(this, "GetDisplayName");
            Object.RegisterPartEvent(this, "GetShortDescription");
            Object.RegisterPartEvent(this, "GetShortDisplayName");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            switch (E.ID)
            {
                case "GetShortDescription":
                    AddShortDescription(E);
                    break;
                case "GetDisplayName":
                case "GetShortDisplayName":
                    AddDisplayNamePrefix(E);
                    break;
                case "GetMaxWeight":
                    AdjustMaxWeight(E);
                    break;
            }

            return base.FireEvent(E);
        }

        private void AddShortDescription(Event E)
        {
            string description = GetShortDescriptionText();
            E.SetParameter("Postfix", E.GetStringParameter("Postfix") + description);
        }

        protected abstract string GetShortDescriptionText();

        private void AddDisplayNamePrefix(Event E)
        {
            if (!ParentObject.Understood() || !ParentObject.HasProperName)
            {
                E.GetParameter<StringBuilder>("Prefix").Append(GetDisplayNamePrefix());
            }
        }

        protected abstract string GetDisplayNamePrefix();

        private void AdjustMaxWeight(Event E)
        {
            E.AddParameter("Weight", (int)Math.Floor((double)(int)E.GetParameter("Weight") * 100));
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            return Object.GetPart(this.GetType()) == null;
        }

        public abstract void ApplyModification(GameObject Object);
    }
}
