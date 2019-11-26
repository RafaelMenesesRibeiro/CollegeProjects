using System;
using System.Linq;
using System.Text;

namespace dida_clients.helpers
{
    public enum EFieldType { Str, ObjectType, ObjectConstructor, NullObject, UnknownType }

    public class FieldType
    {
        public static EFieldType ProcessArgumentType(string arg)
        {
            if (arg.Equals("null"))
            {
                return EFieldType.NullObject;
            }
            else if (arg.All(c => char.IsLower(c) || c.Equals('*')))
            {
                return EFieldType.Str;
            }
            else if (arg.Contains('(') && arg.Contains(')'))
            {
                return EFieldType.ObjectConstructor;
            }
            else if (arg.Any(c => char.IsUpper(c)) && !arg.Contains('(') || arg.Contains(')'))
            {
                return EFieldType.ObjectType;
            }
            return EFieldType.UnknownType;
        }
    }
}