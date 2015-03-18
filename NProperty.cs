using System;

namespace NShapeCreator
{

    public class NProperty : ICloneable
    {

        public enum PropertyType
        {
            @int,
            @string,
            @float,
        }

        public String Name
        {
            get;
            set;
        }

        public PropertyType Type
        {
            get;
            set;
        }

        public virtual Object Clone()
        {
            NProperty me = new NProperty();
            me.Name = Name;
            me.Type = Type;
            return me;
        }

    }

}