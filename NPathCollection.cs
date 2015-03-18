using System;
using System.Collections;
using System.Collections.Generic;

namespace NShapeCreator
{
    public class NPathCollection : CollectionBase, ICloneable
    {

        public NPath this[int index]
        {
            get
            {
                return ((NPath)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(NPath value)
        {
            return (List.Add(value));
        }

        public void Remove(NPath value)
        {
            List.Remove(value);
        }

        public bool Contains(NPath value)
        {
            return (List.Contains(value));
        }

        public Object Clone()
        {
            NPathCollection me = new NPathCollection();
            for (int i1 = 0; i1 < this.Count; i1++)
            {
                me.Add((NPath)this[i1].Clone());
            }
            return me;
        }

        public void InsertAt(int index, NPath value)
        {
            List.Insert(index, value);
        }

	}
}
