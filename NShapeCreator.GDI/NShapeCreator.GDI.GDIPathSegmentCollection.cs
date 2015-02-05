using System;
using System.Collections;
using System.Collections.Generic;

namespace NShapeCreator.GDI
{
    public class GDIPathSegmentCollection : CollectionBase, ICloneable
    {

        public GDIPathSegment this[int index]
        {
            get
            {
                return ((GDIPathSegment)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(GDIPathSegment value)
        {
            return (List.Add(value));
        }

        public void Remove(GDIPathSegment value)
        {
            List.Remove(value);
        }

        public bool Contains(GDIPathSegment value)
        {
            return (List.Contains(value));
        }

        public Object Clone()
        {
            GDIPathSegmentCollection me = new GDIPathSegmentCollection();
            for (int i1 = 0; i1 < this.Count; i1++)
            {
                me.Add((GDIPathSegment)this[i1].Clone());
            }
            return (Object)me;
        }

        public void InsertAt(int index, GDIPathSegment value)
        {
            List.Insert(index, value);
        }

    }
}