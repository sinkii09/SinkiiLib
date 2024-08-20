using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace SinkiiLib.Systems
{
    public class BindableProperty<T>
    {
        readonly Func<T> getter;

        BindableProperty(Func<T> getter)
        {
            this.getter = getter;
        }

        [CreateProperty]
        public T Value => getter();

        public static BindableProperty<T> Bind(Func<T> getter) => new BindableProperty<T>(getter);
    }
}
