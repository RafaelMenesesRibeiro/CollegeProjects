using dida_contracts.exceptions;
using dida_contracts.web_services;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace dida_contracts.domain_objects.dida_tuple_types
{
    public class NullTuple : DIDATuple
    {
        public NullTuple(List<object> argumentsList) : base(argumentsList) { }
    }
}
