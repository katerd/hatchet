using System.Collections.Generic;
using System.Text;

namespace Hatchet
{
    internal class Serializer
    {
        private PrettyPrinter PrettyPrinter { get; }
        private StringBuilder StringBuilder { get; }
        public SerializeOptions SerializeOptions { get; }

        public int IndentLevel => PrettyPrinter.IndentLevel;
        
        private readonly List<object> _metObjects;

        public Serializer(
            PrettyPrinter prettyPrinter, 
            StringBuilder stringBuilder, 
            SerializeOptions serializeOptions)
        {
            PrettyPrinter = prettyPrinter;
            StringBuilder = stringBuilder;
            SerializeOptions = serializeOptions;
            _metObjects = new List<object>();
        }

        internal static void Serialize(
            object input, 
            Serializer serializer,
            bool forceClassName = false)
        {
            serializer.PushObjectRef(input);
            
            var context = new SerializationContext(input, serializer, forceClassName);
            
            foreach (var conversionFunction in HatchetConvert.SerializationRules)
            {
                if (conversionFunction.Item1(input))
                {
                    conversionFunction.Item2(context);
                    serializer.PopObjectRef(input);
                    return;
                }
            }
            throw new HatchetException($"Could not serialize {input} of type {input.GetType()}");
        }

        private void PushObjectRef(object obj)
        {
            var type = obj.GetType();

            if (obj is string)
                return;
            
            if (type.IsValueType)
                return;
            
            if (_metObjects.Contains(obj))
                throw new CircularReferenceException(obj);
            _metObjects.Add(obj);
        }

        private void PopObjectRef(object obj)
        {
            _metObjects.Remove(obj);
        }

        public void AppendFormat(string str, params object[] args)
        {
            PrettyPrinter.AppendFormat(str, args);
        }

        public void Indent()
        {
            PrettyPrinter.Indent();
        }

        public void Deindent()
        {
            PrettyPrinter.Deindent();
        }

        public void Append(string str)
        {
            PrettyPrinter.Append(str);
        }

        public void Append(char chr, int count)
        {
            PrettyPrinter.Append(chr, count);
        }

        public void Append(char chr)
        {
            PrettyPrinter.Append(chr);
        }

        public void Append(object obj)
        {
            PrettyPrinter.Append(obj);
        }

        public void AppendOpenBlock()
        {
            PrettyPrinter.AppendOpenBlock();
        }

        public void AppendCloseBlock()
        {
            PrettyPrinter.AppendCloseBlock();
        }

        public void AppendEnum(object input)
        {
            PrettyPrinter.AppendEnum(input);
        }

        public void AppendDateTime(object input)
        {
            PrettyPrinter.AppendDateTime(input);
        }

        public void AppendString(string inputAsString)
        {
            PrettyPrinter.AppendString(inputAsString);
        }
    }
}