using dida_contracts.exceptions;
using dida_contracts.web_services;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace dida_contracts.domain_objects
{
    [Serializable]
    public class DIDATuple : ISerializable
    {
        public List<object> Arguments { get; }

        public DIDATuple(List<object> argumentsList)
        {
            Arguments = argumentsList;
        }

        public bool Matches(DIDATuple otherTuple)
        {
            if (this.Arguments.Count != otherTuple.Arguments.Count) return false;

            for (int i = 0; i < Arguments.Count; i++)
            {
                object thisArgument = this.Arguments[i];
                Type thisType = thisArgument.GetType();
     
                object otherArgument = otherTuple.Arguments[i];
                Type otherType;

                if (otherArgument == null)
                {
                    Console.WriteLine($" >> User argument{i} is any object...");
                    // User is looking for an object of any type, other than string and this tuple argument i is a string
                    if (thisType == typeof(string)) return false;
                    // If this tuple argument is not a string, than anything goes, because otherArgument as null, matches any object.
                    else continue;
                }
                else
                {   
                    // Since the other argument is not a null, we need to know if we are dealing, with strings, types or specific objects.
                    otherType = otherArgument.GetType();
                    if (otherType == typeof(string))
                    {
                        Console.WriteLine($" >> User argument{i} is a string...");
                        // If user is searching for a tuple whose argument[i] is a string, then this tuple must also be a string and must match.
                        // If this tuple doesn't have string in argument i. It's not a match.
                        if (thisType != typeof(string)) return false;
                        // If both are of type string and they match, advance to the next argument of this tuple.
                        else if (TryStringMatch(thisArgument, otherArgument)) continue;
                        // Both are Strings but they dont match. Search for another feasible tuple.
                        else return false;
                    }
                    else
                    {
                        // User is not searching for a string. He is searching for an object.
                        // If otherArgument is null, we would done everything we wanted on the first condition of this function.
                        // So we we need to check if the user wants an object of a Type, or if he wants a specific object.
                        if (otherArgument.GetType().Name.Equals("RuntimeType"))
                        {
                            Console.WriteLine($" >> User argument{i} is a Type...");
                            // User is looking for an object of a given type, but it's parameters don't matter. Just check if type is the same.
                            // Types are the same, compare next argument
                            if (otherArgument.Equals(thisType)) continue;
                            // Types are not equal, search another tuple.
                            else return false;
                        }
                        // User is searching for a specific object, with the same type and same parameters
                        // If they are do a object equality comparison. If they are the same check next tuple argument
                        Console.WriteLine($" >> User argument{i} is a specific object...");
                        if (otherArgument.Equals(thisArgument)) continue;
                        // Else, search another tuple.
                        else return false;
                    }
                }
            }
            // If all arguments matched correctly return the trueeeeeeeeeee
            return true;
        }

        private bool TryStringMatch(object thisArgument, object otherArgument)
        {
            if (thisArgument.GetType() == typeof(string) && otherArgument.GetType() == typeof(string)) {
                string queryString = (string)otherArgument;
                string currentStringArgument = (string)thisArgument;
                Console.WriteLine($" >> User searched for string: {queryString}");

                // If both are strings, compare for equality
                if (queryString.Equals("*") || currentStringArgument.Equals(queryString))
                {
                    return true;
                }
                else if (queryString.StartsWith("*") && currentStringArgument.EndsWith(queryString.Substring(1)))
                {
                    return true;
                }
                else if (queryString.EndsWith("*") && currentStringArgument.StartsWith(queryString.Substring(0, queryString.Length - 1)))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DIDATuple)) return false;

            DIDATuple tupleData = (DIDATuple)obj;

            if (Arguments.Count != tupleData.Arguments.Count) return false;

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (Arguments[i].GetType() == typeof(string))
                {
                    if (tupleData.Arguments[i].GetType() != typeof(string)) return false;

                    Regex rx = new Regex((string)tupleData.Arguments[i]);
                    if (!rx.IsMatch((string)Arguments[i])) return false;
                }

                if (Arguments[i] == null && tupleData.Arguments[i] != null) return false;
                if (tupleData.Arguments[i].GetType() == typeof(Type) && tupleData.Arguments[i].Equals(Arguments[i].GetType())) return false;
                if (Arguments[i].GetType() != tupleData.Arguments[i].GetType()) return false;
                if (!Arguments[i].Equals(tupleData.Arguments[i])) return false;
            }

            return true;
        }

        public override string ToString()
        {
            string didaString = "";

            foreach (object obj in Arguments)
            {
                didaString = $"{didaString}, {obj.ToString()}";
            }

            return didaString;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Serialization Methods
        public DIDATuple(SerializationInfo info, StreamingContext context)
        {
            Arguments = (List<object>)info.GetValue("tuple", typeof(List<object>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("tuple", Arguments);
        }
        #endregion
    }
}
