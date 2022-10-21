using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AddressBook;

/// <summary>
/// A representation of a contact for database storage.
/// </summary>
public class ContactEntity
{
    /// <summary>
    /// The ID of the contact.
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The first name of the contact.
    /// </summary>
    [Required]
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// The last name of the contact.
    /// </summary>
    [Required]
    public string LastName { get; set; } = default!;

    /// <summary>
    /// A note about a contact.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// A list of pokes performed on this contact.
    /// </summary>
    public ICollection<PokeEntity> Pokes { get; set; } = new List<PokeEntity>();
}
