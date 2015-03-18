using System;
using System.Collections;
using System.Collections.Generic;

namespace NShapeCreator
{
    public class NSegmentCollection : CollectionBase, ICloneable
    {

        public NSegment this[int index]
        {
            get
            {
                return ((NSegment)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(NSegment value)
        {
            return (List.Add(value));
        }

        public void Remove(NSegment value)
        {
            List.Remove(value);
        }

        public bool Contains(NSegment value)
        {
            return (List.Contains(value));
        }

        public Object Clone()
        {
            NSegmentCollection me = new NSegmentCollection();
            for (int i1 = 0; i1 < this.Count; i1++)
            {
                me.Add((NSegment)this[i1].Clone());
            }
            return me;
        }

        public void InsertAt(int index, NSegment value)
        {
            List.Insert(index, value);
        }

    }
}