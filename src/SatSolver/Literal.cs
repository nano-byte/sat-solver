// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoByte.SatSolver;

/// <summary>
/// A Boolean Literal.
/// </summary>
/// <typeparam name="T">The underlying type used to identify/compare Literals.</typeparam>
public readonly struct Literal<T> : IEquatable<Literal<T>>
    where T : IEquatable<T>
{
    /// <summary>
    /// The underlying value used to identify/compare Literals.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Indicates whether this Literal has been negated.
    /// </summary>
    public bool Negated { get; }

    /// <summary>
    /// Creates a Literal.
    /// </summary>
    /// <param name="value">The underlying value used to identify/compare Literals.</param>
    /// <param name="negated">Indicates whether this Literal has been negated.</param>
    public Literal(T value, bool negated = false)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        Value = value;
        Negated = negated;
    }

    /// <summary>
    /// Creates a <see cref="Literal{T}"/>.
    /// </summary>
    /// <param name="value">The underlying value used to identify/compare Literals.</param>
    public static implicit operator Literal<T>(T value)
        => Literal.Of(value);

    /// <summary>
    /// Returns a negated copy of this Literal.
    /// </summary>
    public Literal<T> Negate()
        => new(Value, !Negated);

    /// <summary>
    /// Returns a negated copy of this Literal.
    /// </summary>
    public static Literal<T> operator !(Literal<T> literal)
        => literal.Negate();

    /// <summary>
    /// Creates a <see cref="Clause{T}"/> consisting of a single <see cref="Literal{T}"/>.
    /// </summary>
    /// <param name="literal">The single Literal in the Clause.</param>
    public static implicit operator Clause<T>(Literal<T> literal)
        => [literal];

    /// <summary>
    /// Creates a <see cref="Clause{T}"/> consisting of two <see cref="Literal{T}"/>s.
    /// </summary>
    /// <param name="literal1">The first Literal in the Clause.</param>
    /// <param name="literal2">The second Literal in the Clause.</param>
    public static Clause<T> operator |(Literal<T> literal1, Literal<T> literal2)
        => [literal1, literal2];

    /// <summary>
    /// Creates a <see cref="Formula{T}"/> consisting of two <see cref="Clause{T}"/>s, each containing a single <see cref="Literal{T}"/>.
    /// </summary>
    /// <param name="literal1">The Literal in the first Clause.</param>
    /// <param name="literal2">The Literal in the second Clause.</param>
    public static Formula<T> operator &(Literal<T> literal1, Literal<T> literal2)
        => [literal1, literal2];

    /// <summary>
    /// Checks whether this Literal conflicts with the given <paramref name="literal"/>.
    /// </summary>
    public bool ConflictsWith(Literal<T> literal)
        => Value.Equals(literal.Value) && Negated != literal.Negated;

    /// <summary>
    /// Checks whether this Literal conflicts with any of the given <paramref name="literals"/>.
    /// </summary>
    public bool IsPure(IEnumerable<Literal<T>> literals)
        => !literals.Any(ConflictsWith);

    public override string ToString()
        => Negated ? $"!{Value}" : $"{Value}";

    public bool Equals(Literal<T> other)
        => Value.Equals(other.Value) && Negated == other.Negated;

    public override bool Equals(object? obj)
        => obj is Literal<T> literal && Equals(literal);

    public override int GetHashCode()
    {
        unchecked
        {
            return Value.GetHashCode() * Negated.GetHashCode();
        }
    }

    public static bool operator ==(Literal<T> left, Literal<T> right) => left.Equals(right);

    public static bool operator !=(Literal<T> left, Literal<T> right) => !left.Equals(right);
}

/// <summary>
/// Factory methods for <see cref="Literal{T}"/>s.
/// </summary>
public static class Literal
{
    /// <summary>
    /// Creates a <see cref="Literal{T}"/>.
    /// </summary>
    /// <param name="value">The underlying value used to identify/compare Literals.</param>
    public static Literal<T> Of<T>(T value)
        where T : IEquatable<T>
        => new(value);
}
