namespace TimePunch_WPF_StackedModern_Demo.Models
{
    /// <summary>
    /// Resource entry 
    /// </summary>
    public class Resource<T>
    {
        /// <summary>
        /// Initializes a new Resource class 
        /// </summary>
        public Resource()
        {
            IsNewItem = true;
        }

        /// <summary>
        /// Initializes a new Resource class and sets the Text and additional value to identify the resource
        /// </summary>
        /// <param name="text">Text Value</param>
        /// <param name="value">Resource</param>
        public Resource(string text, T value)
        {
            Text = text;
            Value = value;
            IsNewItem = false;
        }

        /// <summary>
        /// Gets the Text property
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the Value property
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets an indicator that the item is new
        /// </summary>
        public bool IsNewItem { get; set; }

        #region Overrides of Object

        public override string ToString()
        {
            return Text;
        }

        #endregion
    }
}
