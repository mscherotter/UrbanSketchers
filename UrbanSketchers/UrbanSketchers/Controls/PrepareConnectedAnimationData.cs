namespace UrbanSketchers.Controls
{
    /// <summary>
    ///     Prepare connected animation data
    /// </summary>
    public class PrepareConnectedAnimationData
    {
        /// <summary>
        ///     Initializes a new instance of the PrepareConnectedAnimationData class.
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="item">the item</param>
        /// <param name="elementName">the element name</param>
        public PrepareConnectedAnimationData(string key, object item, string elementName)
        {
            Key = key;
            Item = item;
            ElementName = elementName;
        }

        /// <summary>
        ///     Gets the key
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     Gets the item
        /// </summary>
        public object Item { get; }

        /// <summary>
        ///     Gets the element name
        /// </summary>
        public string ElementName { get; }
    }
}