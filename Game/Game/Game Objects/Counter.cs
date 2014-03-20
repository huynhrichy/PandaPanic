using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Counter
    {
        private int current, limit;

        public int Current
        {
            get { return current; }
        }

        public int Limit{
            get { return limit; }
            set{limit = value;}
        }

        public Counter(int limit)
        {
            this.limit = limit;
            current = 0;
        }

        public bool isReady()
        {
            current++;
            if (current % limit == 0)
            {
                current = 0;
                return true;
            }
            else return false;
        }

        public void reset()
        {
            current = 0;
        }
    }
}
