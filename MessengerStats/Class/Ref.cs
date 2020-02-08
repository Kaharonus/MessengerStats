using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerStats {
    /// <summary>
    /// C# did not support async ref parameters so here we are. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Ref<T> {
        public Ref() { }
        public Ref(T value) { Value = value; }
        public T Value { get; set; }
        public override string ToString() {
            T value = Value;
            return value == null ? "" : value.ToString();
        }
        public static implicit operator T(Ref<T> r) { return r.Value; }
        public static implicit operator Ref<T>(T value) { return new Ref<T>(value); }
    }
}
