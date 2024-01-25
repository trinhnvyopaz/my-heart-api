using System;
using System.Collections.Generic;
using System.Text;

namespace MyHeart.Core.Helpers
{
    public class ConvertByteToMegaBytes
    {
        public static double Convert(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}