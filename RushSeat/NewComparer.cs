using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RushSeat
{
    class NewComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            DictionaryEntry X = (DictionaryEntry)x;
            DictionaryEntry Y = (DictionaryEntry)y;
            if (int.Parse(X.Value.ToString()) < int.Parse(Y.Value.ToString()))
                return -1;
            else if (int.Parse(X.Value.ToString()) > int.Parse(Y.Value.ToString()))
                return 1;
            else
                return 0;
        }
    }
}
