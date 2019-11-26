using System;

namespace dida_contracts.domain_objects.tuple_space_objects
{
    [Serializable]
    public class DADTestB
    {
        public int i1;
        public string s1;
        public int i2;

        public DADTestB(int pi1, string ps1, int pi2)
        {
            i1 = pi1;
            s1 = ps1;
            i2 = pi2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj.GetType() != typeof(DADTestB)) return false;
                DADTestB o = (DADTestB)obj;
                return ((this.i1 == o.i1) && (this.s1.Equals(o.s1)) && (this.i2 == o.i2));
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
