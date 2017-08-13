using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hatchet.Extensions;

namespace Hatchet
{    
    public static partial class HatchetConvert
    {
        private const string LineEnding = "\n";
        private const int IndentCount = 2;



        public static string Serialize(object input)
        {
            var stringBuilder = new StringBuilder();
            var prettyPrinter = new PrettyPrinter(stringBuilder);
            Serialize(input, prettyPrinter);
            return stringBuilder.ToString();
        }
        
        private static void Serialize(
            object input, 
            PrettyPrinter prettyPrinter,
            bool forceClassName = false)
        {
            foreach (var conversionFunction in SerializationRules)
            {
                if (conversionFunction.Item1(input))
                {
                    conversionFunction.Item2(input, prettyPrinter, forceClassName);
                    return;
                }
            }
            throw new HatchetException($"Could not serialize {input} of type {input.GetType()}");
        }

        private static bool IsSimpleValue(Type inputType)
        {
            return inputType.IsPrimitive 
                   || inputType == typeof(decimal) 
                   || inputType == typeof(DateTime)
                   || inputType == typeof(Guid);
        }

        private static void SerializeArray(object input, PrettyPrinter stringBuilder)
        {
            var inputArray = (Array) input;
            stringBuilder.AppendFormat("[{0}]", string.Join(" ", inputArray.Select(Serialize)));
        }

        private static void SerializeCollection(object input, PrettyPrinter prettyPrinter, bool forceClassName)
        {
            var inputList = (ICollection) input;

            foreach (var item in inputList)
            {
                prettyPrinter.Indent();
                Serialize(item, prettyPrinter, forceClassName);
                prettyPrinter.Deindent();
            }
        }

        private static void SerializeGenericEnumerable(object input, PrettyPrinter prettyPrinter, bool forceClassName)
        {
            var inputType = input.GetType();
            
            var elementType = inputType.GenericTypeArguments[0];

            if (elementType.IsAbstract)
                forceClassName = true;

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            prettyPrinter.Append("[");

            if (enumerableType.IsAssignableFrom(inputType))
            {
                var enr = ((IEnumerable) input).GetEnumerator();

                var addSpace = false;
                while (enr.MoveNext())
                {
                    if (addSpace)
                        prettyPrinter.AppendFormat(" ");
                    addSpace = true;

                    var o = enr.Current;
                    prettyPrinter.Indent();
                    Serialize(o, prettyPrinter, forceClassName);
                    prettyPrinter.Deindent();
                }
            }

            prettyPrinter.Append("]");
        }

        private static void SerializeDictionary(PrettyPrinter prettyPrinter, IDictionary inputDictionary)
        {
            if (inputDictionary.Count == 0)
            {
                prettyPrinter.Append("{}");
                return;
            }

            prettyPrinter.Append("{");
            prettyPrinter.Append(LineEnding);

            foreach (var key in inputDictionary.Keys)
            {
                var keyStr = key.ToString();
                if (keyStr.Contains(" "))
                {
                    throw new HatchetException(
                        $"`{keyStr}` is an invalid dictionary key. Key cannot contain spaces.");
                }

                var value = inputDictionary[keyStr];

                SerializeKeyValue(prettyPrinter, keyStr, value);
            }

            prettyPrinter.Append(' ', prettyPrinter.IndentLevel * IndentCount);
            prettyPrinter.Append("}");
        }

        private static void SerializeClassOrStruct(
            object input, 
            PrettyPrinter prettyPrinter, 
            bool forceClassName)
        {
            var inputType = input.GetType();
            
            prettyPrinter.Append("{");
            prettyPrinter.Append(LineEnding);

            if (forceClassName)
            {
                prettyPrinter.Append(' ', prettyPrinter.IndentLevel * IndentCount);
                prettyPrinter.Append(' ', IndentCount);
                prettyPrinter.AppendFormat("Class {0}", inputType.Name);
                prettyPrinter.Append(LineEnding);
            }

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var field in inputType.GetFields(bindingFlags))
            {
                SerializeField(input, prettyPrinter, field);
            }

            foreach (var property in inputType.GetProperties(bindingFlags))
            {
                SerializeProperty(input, prettyPrinter, property);
            }

            prettyPrinter.Append(' ', prettyPrinter.IndentLevel * IndentCount);
            prettyPrinter.Append("}");
        }

        private static void SerializeProperty(object input, PrettyPrinter prettyPrinter, PropertyInfo property)
        {
            if (property.HasAttribute<HatchetIgnoreAttribute>())
                return;

            if (property.SetMethod == null)
                return;

            var keyStr = property.Name;
            var value = property.GetValue(input);

            if (value == null)
                return;

            var forceClassName = property.PropertyType.IsAbstract;

            SerializeKeyValue(prettyPrinter, keyStr, value, forceClassName);
        }

        private static void SerializeField(object input, PrettyPrinter prettyPrinter, FieldInfo field)
        {
            if (field.HasAttribute<HatchetIgnoreAttribute>())
                return;

            var keyStr = field.Name;
            var value = field.GetValue(input);

            if (value == null)
                return;

            var forceClassName = field.FieldType.IsAbstract;

            SerializeKeyValue(prettyPrinter, keyStr, value, forceClassName);
        }
        
        private static void SerializeKeyValue(PrettyPrinter prettyPrinter, string key, object value, bool forceClassName = false)
        {
            prettyPrinter.Append(' ', prettyPrinter.IndentLevel * IndentCount);
            prettyPrinter.Append(' ', IndentCount);
            prettyPrinter.Append(key);
            prettyPrinter.Append(' ');
            prettyPrinter.Indent();
            Serialize(value, prettyPrinter, forceClassName);
            prettyPrinter.Deindent();
            prettyPrinter.Append(LineEnding);
        }
        
        private static Tuple<Func<object, bool>, 
            Action<object, PrettyPrinter, bool>> MakeSerialiser(Func<object, bool> test, 
            Action<object, PrettyPrinter, bool> action)
        {
            return new Tuple<Func<object, bool>, Action<object, PrettyPrinter, bool>>(test, action);
        }
    }
}