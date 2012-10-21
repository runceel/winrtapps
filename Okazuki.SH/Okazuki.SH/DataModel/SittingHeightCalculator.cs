using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Okazuki.SH.DataModel
{
    static class SittingHeightCalculator
    {
        //椅子と机の適正な高さを出す式
        //椅子は身長の１／４
        //椅子と机の間は身長の１／６
        //机の高さは上記二つの合計

        public static SittingHeight Execute(int height)
        {
            return new SittingHeight
            {
                ChairHeight = height / 4,
                MarginHeight = height / 6
            };
        }

    }

    public class SittingHeight
    {
        public int ChairHeight { get; set; }
        public int MarginHeight { get; set; }
        public int TableHeight { get { return this.ChairHeight + this.MarginHeight; } }
    }
}
