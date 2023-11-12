﻿using System.Linq;
using System.Text.Json.Nodes;
using Json.JsonE.Operators;
using Json.More;

namespace Json.JsonE;

internal static class JsonNodeExtensions
{
	private static readonly JsonNode? _emptyString = string.Empty;
	private static readonly JsonNode? _zero = 0;
	private static readonly JsonNode? _false = false;

	public static bool IsTruthy(this JsonNode? node)
	{
		if (node is null) return false;
		if (node is JsonObject { Count: 0 }) return false;
		if (node is JsonArray { Count: 0 }) return false;
		if (node.IsEquivalentTo(_false)) return false;
		if (node.IsEquivalentTo(_zero)) return false;
		if (node.IsEquivalentTo(_emptyString)) return false;

		return true;
	}

	public static bool IsTemplateOr<T>(this JsonNode? node)
	{
		return node switch
		{
			T => true,
			JsonObject when OperatorRepository.Get(node) != null => true,
			JsonValue value when value.TryGetValue<T>(out _) => true,
			_ => false
		};
	}

	public static void VerifyNoUndefinedProperties(this JsonObject obj, string op, params string[] additionalKeys)
	{
		var undefinedKeys = obj.Select(x => x.Key).Where(x => x != op && !additionalKeys.Contains(x)).ToArray();
		if (undefinedKeys.Length != 0)
			throw new TemplateException(CommonErrors.UndefinedProperties(op, undefinedKeys));
	}
}