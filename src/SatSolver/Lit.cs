// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.SatSolver;

/// <summary>
/// Pairing of a <see cref="Variable{T}"/> with a negation flag.
/// </summary>
internal readonly struct Lit<T> : IEquatable<Lit<T>>
    where T : IEquatable<T>
{
    public readonly Variable<T> Variable;
    public readonly bool Negated;

    public Lit(Variable<T> variable, bool negated)
    {
        Variable = variable;
        Negated = negated;
    }

    /// <summary>Returns the negation of this literal.</summary>
    public Lit<T> Negate() => new(Variable, !Negated);

    public static Lit<T> operator !(Lit<T> lit) => lit.Negate();

    /// <summary>
    /// The current truth value of this literal: <c>true</c>, <c>false</c>, or <c>null</c> for undecided.
    /// </summary>
    public bool? Value => Variable.Value is { } v ? v ^ Negated : null;

    /// <summary>
    /// Converts this internal literal back to the public <see cref="Literal{T}"/> representation.
    /// </summary>
    public Literal<T> ToPublic() => new(Variable.UserData, Negated);

    public bool Equals(Lit<T> other) => ReferenceEquals(Variable, other.Variable) && Negated == other.Negated;
    public override bool Equals(object? obj) => obj is Lit<T> other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Variable.Id, Negated);

    public static bool operator ==(Lit<T> a, Lit<T> b) => a.Equals(b);
    public static bool operator !=(Lit<T> a, Lit<T> b) => !a.Equals(b);

    public override string ToString() => Negated ? $"!{Variable.UserData}" : $"{Variable.UserData}";
}
