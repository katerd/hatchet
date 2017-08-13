using System;
using System.Collections.Generic;
using System.Linq;
using Hatchet.Extensions;

namespace Hatchet
{
    internal static class ObjectFactory
    {
        internal static object CreateComplexType(Type type, Dictionary<string, object> inputValues)
        {
            object output;

            var ctors = type.GetConstructors();

            var withAttrs = ctors.Where(x => MemberInfoExtensions.HasAttribute<HatchetConstructorAttribute>(x))
                .ToList();

            if (withAttrs.Count > 0)
            {
                if (withAttrs.Count > 1)
                    throw new HatchetException("Only one constructor can be tagged with [HatchetConstructor]");

                var ctor = withAttrs.First();
                var ctorParams = ctor.GetParameters();

                var args = new List<object>();

                foreach (var parameterInfo in ctorParams)
                {
                    var argValue = inputValues[parameterInfo.Name];
                    args.Add(argValue);
                }

                output = ctor.Invoke(args.ToArray());
            }
            else
            {
                // find default constructor
                var singleCtor = ctors.SingleOrDefault(x => x.GetParameters().Length == 0);

                if (!type.IsClass && singleCtor == null)
                {
                    // structs have no default constructor
                    output = Activator.CreateInstance(type);
                }
                else if (singleCtor != null)
                {
                    output = singleCtor.Invoke(null);
                }
                else
                {
                    throw new HatchetException($"Failed to create {type} - no constructor available");
                }
            }
            return output;
        }
    }
}