using System.Collections.Generic;

namespace HotKeySystem_example.HotKeySystem
{
    public class HotKeyList<T> : List<T> where T : HotKeyBase
    {
        public void AddIfNotNull(T item)
        {
            if(item != null)
            {
                Add(item);
            }
        }
    }
}
