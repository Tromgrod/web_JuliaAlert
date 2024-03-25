namespace LIB.BusinessObjects
{
    /// <summary>
    /// The Permission Enumerator.
    /// </summary>
    public enum OperationTypes : int
    {
        /// <summary>
        /// The none.
        /// </summary>
        Update = 1,
        
        /// <summary>
        /// The super admin.
        /// </summary>
        Delete = 2,
        
        /// <summary>
        /// The control panel area access.
        /// </summary>
        Cancel = 3,

        /// <summary>
        /// The control panel area access.
        /// </summary>
        Insert = 4
    }
}