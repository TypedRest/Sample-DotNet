using System;
using System.ComponentModel.DataAnnotations;

namespace AddressBook
{
    /// <summary>
    /// A contact in an address book.
    /// </summary>
    public class Contact : IEquatable<Contact>
    {
        /// <summary>
        /// The ID of the contact.
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// The first name of the contact.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the contact.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        public bool Equals(Contact other)
        {
            if (other == null) return false;
            return Id == other.Id
                && FirstName == other.FirstName
                && LastName == other.LastName;
        }

        public override bool Equals(object obj)
            => obj is Contact other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (FirstName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (LastName?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
