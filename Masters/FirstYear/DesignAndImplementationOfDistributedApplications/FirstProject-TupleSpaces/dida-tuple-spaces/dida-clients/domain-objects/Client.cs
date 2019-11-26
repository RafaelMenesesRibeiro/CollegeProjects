using System;
using System.Collections.Generic;
using dida_clients.helpers;
using dida_contracts.domain_objects;
using dida_contracts.domain_objects.tuple_space_objects;

namespace dida_clients.domain_objects
{
    public class Client
    {
        #region Fields

        EClientType clientType;
        ForegroundClient foregroundClient;

        #endregion

        #region Constructors

        public Client()
        {
            clientType = EClientType.None;
            foregroundClient = null;
        }

        public Client(EClientType type, ForegroundClient foregroundClient)
        {
            clientType = type;
            this.foregroundClient = foregroundClient;
        }

        #endregion

        #region Tuple Space Methods
        public virtual void Write(string[] command)
        {
            foregroundClient.Write(NewTuple(command));
        }

        public virtual void Read(string[] command)
        {
            foregroundClient.Read(NewTuple(command));
        }

        public virtual void Take(string[] command)
        {
            foregroundClient.Take(NewTuple(command));
        }

        #endregion

        #region Other Methods
        private DIDATuple NewTuple(string[] command)
        {
            List<object> argumentsList = new List<object>();

            int numberOfArguments = command.Length;

            // Start argumentIndex at 1 instead of 0 so the DIDATuple doesn't include the type of command (write, read, take - or other if behaviour was erroneous).
            for (int argumentIndex = 1; argumentIndex < numberOfArguments; argumentIndex++)
            {
                EFieldType fieldType = FieldType.ProcessArgumentType(command[argumentIndex]);

                if (fieldType == EFieldType.NullObject)
                {
                    argumentsList.Add(null);
                }
                else if (fieldType == EFieldType.ObjectConstructor)
                {
                    argumentsList.Add(NewObject(command[argumentIndex]));
                }
                else if (fieldType == EFieldType.ObjectType)
                {
                    argumentsList.Add(NewType(command[argumentIndex]));
                }
                else if (fieldType == EFieldType.Str)
                {
                    if (command[0].Equals("add") && command[argumentIndex].Contains("*"))
                    {
                        throw new ArgumentException("Write primitives can't have wild carded strings.");
                    } else
                    {
                        argumentsList.Add(command[argumentIndex]);
                    }
                }
                else
                {
                    throw new ArgumentException($"Command argument: {command[argumentIndex]} could not be translated. Make sure the input is valid.");
                }
            }

            return new DIDATuple(argumentsList);
        }

        private Type NewType(string typeString)
        {
            if (typeString.Equals("DADTestA"))
            {
                return typeof(DADTestA);
            }
            else if (typeString.Equals("DADTestB"))
            {
                return typeof(DADTestB);
            }
            else if (typeString.Equals("DADTestC"))
            {
                return typeof(DADTestC);
            }
            else
            {
                throw new ArgumentException($" [x] Object type: {typeString} does not exist.");
            }
        }

        private object NewObject(string objectString)
        {
            string[] separatingChars = { "(", ")", "," };
            string[] objectAsArray = objectString.Split(separatingChars, StringSplitOptions.RemoveEmptyEntries);

            string objectType = objectAsArray[0];

            if (objectType.Equals("DADTestA"))
            {
                return new DADTestA(Int32.Parse(objectAsArray[1]), objectAsArray[2]);
            }
            else if (objectType.Equals("DADTestB"))
            {
                return new DADTestB(Int32.Parse(objectAsArray[1]), objectAsArray[2], Int32.Parse(objectAsArray[3]));
            }
            else if (objectType.Equals("DADTestC"))
            {
                return new DADTestC(Int32.Parse(objectAsArray[1]), objectAsArray[2], objectAsArray[3]);
            }
            else
            {
                throw new ArgumentException($" [x] Object type: {objectType} does not exist. Therefore it could not be instanciated.");
            }
        }

        public void SetVerbose(bool boolean)
        {
            foregroundClient.SetVerbose(boolean);
        }

        #endregion
    }
}