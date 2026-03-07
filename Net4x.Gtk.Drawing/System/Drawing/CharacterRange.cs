namespace System.Drawing
{
    /// <summary>
    /// Specifies a range of character positions within a string.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    /// <see cref="System.Drawing.CharacterRange" /> structure, specifying a range of character positions within a string.
    /// </summary>
    /// <param name="First">The position of the first character in the range. For example, if
    /// <paramref name="First" /> is set to 0, the first position of the range is position 0 in the string.
    /// </param>
    /// <param name="Length">The number of positions in the range.
    /// </param>
    public struct CharacterRange
    {
        private int _dummyPrimitive;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Drawing.CharacterRange" /> structure, specifying a range of character positions within a string.
        /// </summary>
        /// <param name="First">The position of the first character in the range. For example, if
        /// <paramref name="First" /> is set to 0, the first position of the range is position 0 in the string.
        /// </param>
        /// <param name="Length">The number of positions in the range.
        /// </param>
        public CharacterRange(int First, int Length)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the position in the string of the first character of this
        /// <see cref="System.Drawing.CharacterRange" />.
        /// </summary>
        /// <returns>The first position of this
        /// <see cref="System.Drawing.CharacterRange" />.
        /// </returns>
        public int First
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the number of positions in this
        /// <see cref="System.Drawing.CharacterRange" />.
        /// </summary>
        /// <returns>The number of positions in this
        /// <see cref="System.Drawing.CharacterRange" />.
        /// </returns>
        public int Length
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is equivalent to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare to for equality.
        /// </param>
        /// <returns>
        /// <see langword="true" /> to indicate the specified object is an instance with the same
        /// <see cref="System.Drawing.CharacterRange.First" /> and
        /// <see cref="System.Drawing.CharacterRange.Length" /> value as this instance; otherwise,
        /// <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

    }
}
