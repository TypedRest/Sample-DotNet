using System;
using System.ComponentModel.DataAnnotations;

namespace AddressBook
{
    /// <summary>
    /// A note about a specific <see cref="Contact"/>.
    /// </summary>
    public class Note : IEquatable<Note>
    {
        /// <summary>
        /// The content of the note.
        /// </summary>
        [Required]
        public string Content { get; set; }

        public bool Equals(Note other)
        {
            if (other == null) return false;
            return Content == other.Content;
        }

        public override bool Equals(object obj)
            => obj is Note other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Content);
    }
}
