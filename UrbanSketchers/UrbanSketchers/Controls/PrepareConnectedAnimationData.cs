using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSketchers.Controls
{
    public class PrepareConnectedAnimationData
    {
        public PrepareConnectedAnimationData(string key, object item, string elementName)
        {
            Key = key;
            Item = item;
            ElementName = elementName;
        }

        public string Key { get; private set; }
        public object Item { get; private set; }
        public string ElementName { get; private set; }
    }
}
