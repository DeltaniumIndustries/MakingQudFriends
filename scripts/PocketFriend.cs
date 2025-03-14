using System;
using XRL.Core;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class acegiak_PocketFriend : IPart
    {
        private bool boot = false;

        public acegiak_PocketFriend()
        {
            SetName("acegiak_PocketFriend");
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            RegisterEvents(Object);
            base.Register(Object, Registrar);
        }

        private void RegisterEvents(GameObject Object)
        {
            Object.RegisterPartEvent(this, "GetInventoryActions");
            Object.RegisterPartEvent(this, "PowerSwitchActivate");
            Object.RegisterPartEvent(this, "CommandTakeObject");
            Object.RegisterPartEvent(this, "Dropped");
            Object.RegisterPartEvent(this, "EnteredCell");
        }

        public override bool FireEvent(Event E)
        {
            switch (E.ID)
            {
                case "Dropped":
                    HandleDropped();
                    break;
                case "EnteredCell":
                    HandleEnteredCell();
                    break;
                case "GetInventoryActions":
                    HandleGetInventoryActions(E);
                    break;
            }

            return base.FireEvent(E);
        }

        private void HandleDropped()
        {
            boot = true;
        }

        private void HandleEnteredCell()
        {
            if (boot)
            {
                boot = false;
                if (ParentObject.GetPart<Brain>() == null)
                    return;

                var brain = ParentObject.GetPart<Brain>();
                brain.PerformReequip();
                brain.FactionFeelings.Clear();
                brain.BecomeCompanionOf(ParentObject.ThePlayer);
                brain.IsLedBy(ParentObject.ThePlayer);
                brain.SetPartyLeader(ParentObject.ThePlayer);
                brain.SetFactionMembership(ParentObject.ThePlayer.GetPrimaryFactionName(), 100);
                brain.Goals.Clear();
                brain.Allegiance.Calm = false;
                brain.Hibernating = false;

                ParentObject.AddPart(new Combat());
                XRLCore.Core.Game.ActionManager.AddActiveObject(ParentObject);
            }
        }

        private void HandleGetInventoryActions(Event E)
        {
            if (ParentObject.GetPart<Physics>() == null)
                return;

            var eventParameter = E.GetParameter("Actions") as EventParameterGetInventoryActions;
            var physics = ParentObject.GetPart<Physics>();

            if (physics.Equipped != null)
            {
                AddRemoveAction(eventParameter);
            }
            else
            {
                HandleInventoryActions(eventParameter, physics);
            }

            if (physics.IsAflame())
            {
                eventParameter.AddAction("Firefight", 'f', false, "&Wf&yight fire", "CommandFightFire");
            }
        }

        private void AddRemoveAction(EventParameterGetInventoryActions eventParameter)
        {
            if (!HasPropertyOrTag("NoRemoveOptionInInventory"))
            {
                eventParameter.AddAction("Remove", 'r', true, "&Wr&yemove", "InvCommandUnequipObject");
            }
        }

        private void HandleInventoryActions(EventParameterGetInventoryActions eventParameter, Physics physics)
        {
            bool flag2 = true;

            if (physics.InInventory == null || !physics.InInventory.IsPlayer())
            {
                if (ParentObject.IsTakeable())
                {
                    eventParameter.AddAction("Get", 'g', true, "&Wg&yet", "CommandTakeObject");
                }
                else
                {
                    flag2 = false;
                }
            }
            else if (!ParentObject.HasTagOrProperty("CannotDrop"))
            {
                eventParameter.AddAction("Drop", 'd', true, "&Wd&yrop", "CommandDropObject");
            }

            if (flag2 && !ParentObject.HasTagOrProperty("CannotEquip"))
            {
                eventParameter.AddAction("AutoEquip", 'e', true, "&We&yquip (auto)", "CommandAutoEquipObject");
                eventParameter.AddAction("DoEquip", 'E', true, "&WE&yquip (manual)", "CommandEquipObject");
            }
        }
    }
}
