using System;

namespace dida_contracts.domain_objects.tuple_space_objects
{
    [Serializable]
    public class DADTestA
    {
        public int i1;
        public string s1;

        public DADTestA(int pi1, string ps1)
        {
            i1 = pi1;
            s1 = ps1;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj.GetType() != typeof(DADTestA)) return false;
                DADTestA o = (DADTestA)obj;
                return ((this.i1 == o.i1) && (this.s1.Equals(o.s1)));
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
