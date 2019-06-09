using System.Collections.Generic;
using dnlib.DotNet;

namespace CawkEmulatorV4
{
    public class ValueStack
    {
        public Stack<dynamic> CallStack = new Stack<dynamic>();
        public Dictionary<FieldDef, dynamic> Fields = new Dictionary<FieldDef, dynamic>();
        public dynamic[] Locals;
        public Dictionary<Parameter, dynamic> Parameters = new Dictionary<Parameter, dynamic>();
    }
}