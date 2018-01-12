using System.Text;

namespace Hatchet
{
    public static partial class HatchetConvert
    {
        private const string LineEnding = "\n";
        private const int IndentCount = 2;

        public static string Serialize(object input)
        {
            return Serialize(input, new SerializeOptions());
        }
        
        public static string Serialize(object input, SerializeOptions serializeOptions)
        {
            var stringBuilder = new StringBuilder();
            var prettyPrinter = new PrettyPrinter(stringBuilder);
            var serializer = new Serializer(prettyPrinter, stringBuilder, serializeOptions);

            Serializer.Serialize(input, serializer);
            return stringBuilder.ToString();
        }
        
    }
}