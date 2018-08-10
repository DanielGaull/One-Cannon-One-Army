using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OneCannonOneArmy
{
    [Serializable]
    public struct StatusEffect
    {
        public int Damage;
        public string Name;

        public StatusEffect(int dmgPer2Sec, string name)
        {
            Damage = dmgPer2Sec;
            Name = name;
        }

        public static StatusEffect Fire
        {
            get
            {
                return new StatusEffect(2, "Fire");
            }
        }
        public static StatusEffect Poison
        {
            get
            {
                return new StatusEffect(1, "Poison");
            }
        }
        public static StatusEffect Plasma
        {
            get
            {
                return new StatusEffect(6, "Plamsa");
            }
        }
        public static StatusEffect Curse
        {
            get
            {
                return new StatusEffect(4, "Cursed");
            }
        }
        public static StatusEffect None
        {
            get
            {
                return new StatusEffect(0, "None");
            }
        }

        public static bool operator ==(StatusEffect s1, StatusEffect s2)
        {
            return s1.Damage == s2.Damage;
        }
        public static bool operator !=(StatusEffect s1, StatusEffect s2)
        {
            return !(s1.Damage == s2.Damage);
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
        public override int GetHashCode()
        {
            return (int)(Math.Pow(Damage, 9) * 3);
        }

        public override string ToString()
        {
            return Name;
        }

        public static int TotalDamage(List<StatusEffect> effects)
        {
            int damage = 0;
            for (int i = 0; i < effects.Count; i++)
            {
                damage += effects[i].Damage;
            }
            return damage;
        }
    }
}