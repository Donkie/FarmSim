using System;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Components
{
    public class ComponentBase
    {
        List<ComponentBase> components = new List<ComponentBase>();

        public T1 GetComponent<T1>() where T1 : ComponentBase
        {
            return components.OfType<T1>().FirstOrDefault();
        }

        public T1 AddComponent<T1>() where T1 : ComponentBase, new()
        {
            ComponentBase t = new T1();
            components.Add(t);
            return (T1) t;
        }
    }
}
