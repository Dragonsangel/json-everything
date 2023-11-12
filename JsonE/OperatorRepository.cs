﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.JsonE.Operators;

namespace Json.JsonE;

internal static class OperatorRepository
{
	private static readonly Dictionary<string, IOperator> _operators = new()
	{
		[EvalOperator.Name] = new EvalOperator(),
		[FlattenOperator.Name] = new FlattenOperator(),
		[FlattenDeepOperator.Name] = new FlattenDeepOperator(),
		[FromNowOperator.Name] = new FromNowOperator(),
		[IfThenElseOperator.Name] = new IfThenElseOperator(),
		[MergeOperator.Name] = new MergeOperator(),
		[MergeDeepOperator.Name] = new MergeDeepOperator(),
		[ReverseOperator.Name] = new ReverseOperator(),
	};

	public static IOperator? Get(JsonNode? node)
	{
		if (node is not JsonObject obj) return null;

		var operatorKeys = obj.Select(x => x.Key).Intersect(_operators.Keys).ToArray();
		var op = operatorKeys.Length switch
		{
			> 1 => throw new TemplateException("only one operator allowed"),
			0 => HasReservedWords(obj)
				? throw new TemplateException("$<identifier> is reserved; use $$<identifier>")
				: null,
			_ => _operators[operatorKeys[0]]
		};

		if (op is null) return null;

		op.Validate(obj);
		
		return op;
	}

	private static bool HasReservedWords(JsonObject obj)
	{
		return obj.Any(x => Regex.IsMatch(x.Key, @"^\$[^$]"));
	}
}