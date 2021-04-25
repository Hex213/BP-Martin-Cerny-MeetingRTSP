using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LibHexUtils.Counters.Numeric
{
    public class Counter
    {
        private long min, max, actual;

        public long Actual => actual;

        public Counter(long min, long max, long start = 0)
        {
            if (min > max)
            {
                actual = max;
                this.max = min;
                this.min = actual;
            }
            else
            {
                this.min = min;
                this.max = max;
            }
            
            this.actual = start > min ? start : min;
        }

        public void Add(long add = 1)
        {
            try
            {
                checked
                {
                    if (actual + add > max)
                    {
                        this.actual = min + Math.Abs(max - actual - add);
                    }
                    else
                    {
                        actual += add;
                    }
                }
            }
            catch (OverflowException)
            {

            }
        }

        public void Sub(long sub = 1)
        {
            try
            {
                checked
                {
                    if (actual - sub < min)
                    {
                        this.actual = max - Math.Abs(min - actual - sub);
                    }
                    else
                    {
                        actual -= sub;
                    }
                }
            }
            catch (OverflowException)
            {

            }
        }
    }
}
