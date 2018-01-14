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

        public string Key { get; }
        public object Item { get; }
        public string ElementName { get; }
    }
}