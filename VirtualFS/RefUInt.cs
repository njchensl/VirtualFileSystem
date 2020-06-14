using System;

namespace VirtualFS
{
    [Serializable]
    public class RefUInt
    {
        public uint Value { get; set; }

        public RefUInt()
        {
            Value = 0;
        }
        public RefUInt(uint value)
        {
            Value = value;
        }

        // boxing and unboxing
        public static implicit operator uint(RefUInt value) => value.Value;
        public static implicit operator RefUInt(uint value) => new RefUInt(value);
    }

    [Serializable]
    public class StructRef<T> where T : struct
    {
        public T Value { get; set; }

        public StructRef(T value)
        {
            Value = value;
        }

        public static implicit operator T(StructRef<T> value) => value.Value;
        public static implicit operator StructRef<T>(T value) => new StructRef<T>(value);
    }
}