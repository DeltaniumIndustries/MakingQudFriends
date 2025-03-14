using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.World.Anatomy;

namespace XRL.World.Parts
{
    [Serializable]
    public class acegiak_Zombable : IPart
    {
        [NonSerialized]
        public Body Body = new Body();

        public string storedTile;

        public acegiak_Zombable()
        {
            SetName("acegiak_Zombable");
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Object.RegisterPartEvent(this, "BeforeDie");
            Object.RegisterPartEvent(this, "Dismember");

            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeDie")
            {
                Zombablify();
            }
            return base.FireEvent(E);
        }

        public void Zombablify()
        {
            var CorpsePart = ParentObject.GetPart<Corpse>();
            var part = ParentObject.GetPart<Body>();

            if (CorpsePart == null || part == null)
            {
                return; // No Corpse or Body to zombify
            }

            GameObject gameObject = null;
            bool shouldCreateCorpse = false;

            if (ParentObject.Physics.LastDamagedByType == "Fire")
            {
                shouldCreateCorpse = TryCreateCorpse(CorpsePart.BurntCorpseChance, CorpsePart.BurntCorpseRequiresBodyPart, CorpsePart.BurntCorpseBlueprint, ref gameObject);
            }
            else if (ParentObject.Physics.LastDamagedByType == "Vaporized")
            {
                shouldCreateCorpse = TryCreateCorpse(CorpsePart.VaporizedCorpseChance, CorpsePart.VaporizedCorpseRequiresBodyPart, CorpsePart.VaporizedCorpseBlueprint, ref gameObject);
            }
            else
            {
                shouldCreateCorpse = TryCreateCorpse(CorpsePart.CorpseChance, CorpsePart.CorpseRequiresBodyPart, CorpsePart.CorpseBlueprint, ref gameObject);
            }

            if (gameObject != null)
            {
                // Proceed with zombifying the corpse
                acegiak_Zombable zombieparts = CreateZombifiedBody(part);
                gameObject.AddPart(zombieparts);
            }
            else
            {
                ResetCorpseChances(CorpsePart);
            }
        }

        private bool TryCreateCorpse(int chance, string requiredPart, string blueprint, ref GameObject gameObject)
        {
            if (chance > 0 && (string.IsNullOrEmpty(requiredPart) || ParentObject.GetPart<Body>()?.GetFirstPart(requiredPart) != null) &&
                (chance >= 100 || Stat.Random(1, 100) <= chance))
            {
                gameObject = GameObject.CreateUnmodified(blueprint);
                return true;
            }
            return false;
        }

        private void ResetCorpseChances(Corpse CorpsePart)
        {
            CorpsePart.CorpseChance = 0;
            CorpsePart.VaporizedCorpseChance = 0;
            CorpsePart.BurntCorpseChance = 0;
        }

        private acegiak_Zombable CreateZombifiedBody(Body part)
        {
            acegiak_Zombable zombieparts = new acegiak_Zombable
            {
                Body = new Body { _Body = BodyPartCopy(part._Body, ParentObject, new Body()) }
            };

            if (zombieparts.Body._Body == null)
                return null;

            zombieparts.storedTile = ParentObject.Render.Tile;
            return zombieparts;
        }

        public static BodyPart BodyPartCopy(BodyPart origin, GameObject Parent, Body NewBody)
        {
            if (origin == null || NewBody == null || Parent == null)
            {
                return null;
            }

            BodyPart bodyPart = new BodyPart(NewBody)
            {
                Type = origin.Type,
                VariantType = origin.VariantType,
                Description = origin.Description,
                Name = origin.Name,
                SupportsDependent = origin.SupportsDependent,
                DependsOn = origin.DependsOn,
                RequiresType = origin.RequiresType,
                Category = origin.Category,
                Laterality = origin.Laterality,
                RequiresLaterality = origin.RequiresLaterality,
                Mobility = origin.Mobility,
                Primary = origin.Primary,
                Native = origin.Native,
                Integral = origin.Integral,
                Mortal = origin.Mortal,
                Abstract = origin.Abstract,
                Extrinsic = origin.Extrinsic,
                Plural = origin.Plural,
                Position = origin.Position,
                _ID = origin._ID,
                ParentBody = NewBody
            };

            // Copy all child parts if present
            if (origin.Parts != null)
            {
                bodyPart.Parts = new List<BodyPart>(origin.Parts.Count);
                foreach (var part in origin.Parts)
                {
                    if (!part.Extrinsic)
                    {
                        var copy = BodyPartCopy(part, Parent, NewBody);
                        if (copy != null)
                        {
                            bodyPart.AddPart(copy);
                        }
                    }
                }
            }

            return bodyPart;
        }

        public override void Write(GameObject Object, SerializationWriter Writer)
        {
            Writer.Write(Body._Body == null ? 0 : 1);
            if (Body._Body != null)
            {
                Body.Write(Object, Writer);
            }
            base.Write(Object, Writer);
        }

        public override void Read(GameObject Object, SerializationReader Reader)
        {
            int num = Reader.ReadInt32();
            if (num > 0)
            {
                Body.Read(Object, Reader);
            }
            base.Read(Object, Reader);
        }
    }
}
