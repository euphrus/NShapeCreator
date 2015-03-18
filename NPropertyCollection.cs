using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NShapeCreator
{
    public class NPropertyCollection : CollectionBase, ICloneable
    {

        public NProperty this[int index]
        {
            get
            {
                return ((NProperty)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public int Add(NProperty value)
        {
            return (List.Add(value));
        }

        public void Remove(NProperty value)
        {
            List.Remove(value);
        }

        public bool Contains(NProperty value)
        {
            return (List.Contains(value));
        }

        public Object Clone()
        {
            NPropertyCollection me = new NPropertyCollection();
            for (int i1 = 0; i1 < this.Count; i1++)
            {
                me.Add((NProperty)this[i1].Clone());
            }
            return me;
        }

        public void InsertAt(int index, NProperty value)
        {
            List.Insert(index, value);
        }

    }
}
