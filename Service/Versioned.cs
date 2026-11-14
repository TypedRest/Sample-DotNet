namespace AddressBook;

/// <summary>
/// A value along with the version of the resource it was read from.
/// </summary>
/// <param name="Value">The value.</param>
/// <param name="Version">The version of the resource.</param>
public record Versioned<T>(T Value, Guid Version);
