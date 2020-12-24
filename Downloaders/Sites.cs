using System;
using System.Collections.Generic;
using System.Text;

namespace KobeiD.Downloaders
{
    public enum Sites
    {
        wuxiaWorldA,
        wuxiaWorldB,
        ScribbleHub,
        NovelFull,
        Error,
    }

    public static class SiteExt
    {
        public static Sites SiteFromString(this string str)
        {
            switch(new Uri(str).Host)
            {
                case "www.wuxiaworld.co": return Sites.wuxiaWorldA;
                case "www.wuxiaworld.com": return Sites.wuxiaWorldB;
                case "www.scribblehub.com": return Sites.ScribbleHub;
                case "novelfull.com": return Sites.NovelFull;
                default: return Sites.Error;
            }
        }
    }
}
