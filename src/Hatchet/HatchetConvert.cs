namespace Hatchet
{
    public static class HatchetConvert
    {
        public static T Deserialize<T>(ref string input) where T : new()
        {
            var parser = new Parser();
            var result = parser.Parse(ref input);
            
            // read result into T.

            return new T();

        }
    }
}