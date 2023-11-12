﻿using System.Linq;
using System.Text.Json.Nodes;
using Json.More;

namespace Json.JsonE.Operators;

internal class MergeDeepOperator : IOperator
{ 
	public const string Name = "$mergeDeep";

	public void Validate(JsonNode? template)
	{
		var obj = template!.AsObject();

		obj.VerifyNoUndefinedProperties(Name);

		var parameter = obj[Name];
		if (parameter is JsonArray arr && arr.All(x => x.IsTemplateOr<JsonObject>()) ||
		    OperatorRepository.Get(parameter) != null)
			return;

		throw new TemplateException(CommonErrors.IncorrectValueType(Name, "an array of objects"));
	}

	public JsonNode? Evaluate(JsonNode? template, EvaluationContext context)
	{
		var value = template!.AsObject()[Name]!;
		var array = JsonE.Evaluate(value, context) as JsonArray ??
		            throw new TemplateException(CommonErrors.IncorrectValueType(Name, "an array of objects"));

		return array.Aggregate(new JsonObject(), Merge);
	}

	private static JsonObject Merge(JsonObject result, JsonNode? current)
	{
		var obj = current!.AsObject();

		foreach (var kvp in obj)
		{
			var local = result[kvp.Key];
			if (local is JsonArray localArr && kvp.Value is JsonArray objArray) 
				result[kvp.Key] = localArr.Concat(objArray).ToJsonArray();
			else if (local is JsonObject localObj && kvp.Value is JsonObject objObj)
				result[kvp.Key] = Merge(localObj, objObj);
			else
				result[kvp.Key] = kvp.Value.Copy();
		}

		return result;
	}
}