// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyOrderPair.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the PropertyOrderPair type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The property order pair.
    /// </summary>
    public class PropertyOrderPair : IComparable
    {
        /// <summary>
        /// The _order.
        /// </summary>
        private int _order;

        /// <summary>
        /// The _name.
        /// </summary>
        private string _name;


        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrderPair"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        public PropertyOrderPair(string name, int order)
        {
            this._order = order;
            this._name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// The compare to.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CompareTo(object obj)
        {
            // Sort the pair objects by ordering by order value
            // Equal values get the same rank
            int otherOrder = ((PropertyOrderPair)obj)._order;
            if (otherOrder == this._order)
            {
                // If order not specified, sort by name
                string otherName = ((PropertyOrderPair)obj)._name;
                return string.CompareOrdinal(this._name, otherName);
            }
            else if (otherOrder > this._order)
            {
                return -1;
            }
            return 1;
        }
    }
}