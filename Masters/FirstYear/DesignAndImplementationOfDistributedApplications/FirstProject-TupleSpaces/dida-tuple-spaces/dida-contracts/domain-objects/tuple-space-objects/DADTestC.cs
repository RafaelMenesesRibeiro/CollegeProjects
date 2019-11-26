using System;

namespace dida_contracts.domain_objects.tuple_space_objects
{
    [Serializable]
    public class DADTestC
    {
        public int i1;
        public string s1;
        public string s2;

        public DADTestC(int pi1, string ps1, string ps2)
        {
            i1 = pi1;
            s1 = ps1;
            s2 = ps2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj.GetType() != typeof(DADTestC)) return false;
                DADTestC o = (DADTestC)obj;
                return ((this.i1 == o.i1) && (this.s1.Equals(o.s1)) && (this.s2.Equals(o.s2)));
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}