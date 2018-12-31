namespace Hatchet
{
    public static class Tokens
    {
        public const char ListOpen = '[';
        public const char ListClose = ']';

        public const char ObjectOpen = '{';
        public const char ObjectClose = '}';

        public const char SingleQuote = '\'';
        public const char DoubleQuote = '"';

        public const string BlockCommentOpen = "/*";
        public const string BlockCommentClose = "*/";

        public const string LineComment = "//";

        public const char Cr = '\r';
        public const char Lf = '\n';

        public const string Escape = "\\";

        public const string TextBlockOpen = "![";
        public const string TextBlockClose = "]!";
        
        public const string ShortFormClass = "@";
    }
}