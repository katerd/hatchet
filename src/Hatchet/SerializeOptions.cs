﻿namespace Hatchet
{
    public class SerializeOptions
    {
        public bool IncludeDefaultValues { get; set; }

        public SerializeOptions()
        {
            IncludeDefaultValues = false;
        }
    }
}