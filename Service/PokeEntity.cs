using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBook;

/// <summary>
/// A representation of a poke for database storage.
/// </summary>
public class PokeEntity
{
    /// <summary>
    /// The ID of the poke.
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The ID of the contact that was poked.
    /// </summary>
    [Required]
    public string ContactId { get; set; } = default!;

    /// <summary>
    /// The contact that was poked.
    /// </summary>
    [ForeignKey(nameof(ContactId))]
    public ContactEntity Contact { get; set; } = default!;

    /// <summary>
    /// When the poke was performed.
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; }
}
