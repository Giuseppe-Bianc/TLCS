using System.Reflection;

namespace TLCS {
	public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>> where TEnum : Enumeration<TEnum> {
		private static readonly Dictionary<int, TEnum> Enumerations = CreateEnumeration();

		protected Enumeration(int value, string name) {
			Value = value;
			Name = name;
		}

		public int Value { get; protected init; }
		public string Name { get; protected set; } = string.Empty;
		public static IEnumerable<TEnum> GetAllEnumerations() => Enumerations.Values;
		public static TEnum? FromValue(int value) => Enumerations.TryGetValue(value, out TEnum? enumeration) ? enumeration : default;
		public static TEnum? FromName(string name) => Enumerations.Values.SingleOrDefault(e => e.Name == name);
		public bool Equals(Enumeration<TEnum>? other) => other?.GetType() == GetType() && Value == other.Value;
		public override bool Equals(object? obj) => obj is Enumeration<TEnum> other && Equals(other);
		public override int GetHashCode() => Value.GetHashCode();
		public override string? ToString() => Name;
		public static bool operator ==(Enumeration<TEnum>? left, Enumeration<TEnum>? right) => left?.Value == right?.Value;
		public static bool operator !=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) => !(left == right);
		private static Dictionary<int, TEnum> CreateEnumeration() {
			var EnumerationType = typeof(TEnum);

			return EnumerationType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
				.Where(fieldInfo => EnumerationType.IsAssignableFrom(fieldInfo.FieldType))
				.Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!)
				.ToDictionary(x => x.Value);
		}
	}
}
