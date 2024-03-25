// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedProperties.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   The advanced properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;
    using System.Collections;
    using System.Linq;


    [Serializable]
    /// <summary>
    /// The advanced properties.
    /// </summary>
    public class AdvancedProperties : ICollection, IEnumerator
    {
        /// <summary>
        /// The array of properties.
        /// </summary>
        private readonly ArrayList arrprop;

        /// <summary>
        /// The index.
        /// </summary>
        private int index = -1;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedProperties"/> class.
        /// </summary>
        public AdvancedProperties()
        {
            this.arrprop = new ArrayList(10);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedProperties"/> class.
        /// </summary>
        /// <param name="theArraylist">
        /// The the array list.
        /// </param>
        protected AdvancedProperties(ArrayList theArraylist)
        {
            this.arrprop = theArraylist;
        }

        /// <summary>
        /// The sort.
        /// </summary>
        public void Sort()
        {
            this.arrprop.Sort(new CompareOrder());
        }
        
        /// <summary>
        /// The sort.
        /// </summary>
        public AdvancedProperty Get(string Name)
        {
            foreach (AdvancedProperty property in this.arrprop) {
                if (property.PropertyName == Name)
                    return property;
            }
            return null;
        }

        /// <summary>
        /// The sort.
        /// </summary>
        public bool Exists(string Name)
        {
            foreach (AdvancedProperty property in this.arrprop)
            {
                if (property.PropertyName == Name)
                    return true;
            }
            return false;
        }

        #region ICollection

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get { return this.arrprop.Count; }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        public void Add(AdvancedProperty obj)
        {
            this.arrprop.Add(obj);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="pindex">
        /// The index.
        /// </param>
        /// <param name="obj">
        /// The object.
        /// </param>
        protected void Insert(int pindex, AdvancedProperty obj)
        {
            this.arrprop.Insert(pindex, obj);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        protected void Remove(object obj)
        {
            this.arrprop.Remove(obj);
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="indexer">
        /// The indexer.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// </exception>
        /// <returns>
        /// The <see cref="AdvancedProperty"/>.
        /// </returns>
        public AdvancedProperty this[int indexer]
        {
            get
            {
                //Check if the indexer falls in allowable range.
                if ((indexer >= 0) && (indexer < this.arrprop.Count)) return (AdvancedProperty)this.arrprop[indexer];
                else
                    throw new System.IndexOutOfRangeException(
                        "Index must be between 0 and " + this.arrprop.Count.ToString() + ". Supplied: "
                        + indexer.ToString());
            }
            set
            {
                if ((indexer >= 0) && (indexer < this.arrprop.Count)) this.arrprop[indexer] = value;
                else
                    throw new System.IndexOutOfRangeException(
                        "Index must be between 0 and " + this.arrprop.Count.ToString() + ". Supplied: "
                        + indexer.ToString());
            }
        }
        #endregion

        #region IEnumerator Members

        //both IEnumerator and IDictionaryEnumerator
        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)new AdvancedProperties(this.arrprop);
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public void Reset()
        {
            this.index = -1;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        public object Current
        {
            get { return this.arrprop[this.index]; }
        }

        /// <summary>
        /// The move next.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool MoveNext()
        {
            this.index++;
            return this.index < this.arrprop.Count;
        }

        /// <summary>
        /// Gets the sync root.
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether is synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// The copy to.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public void CopyTo(Array a, int index)
        {
            this.arrprop.CopyTo(a, index);
        }

        #endregion

    }
}