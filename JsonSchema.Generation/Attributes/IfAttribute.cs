﻿using System;

namespace Json.Schema.Generation;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
public class IfAttribute : SchemaGenerationAttribute, IAttributeHandler<IfAttribute>
{
	public string PropertyName { get; set; }
	public object? Value { get; set; }

	public IfAttribute(string propertyName, object? value, object? group)
	{
		PropertyName = propertyName;
		Value = value;
	}

	public void AddConstraints(SchemaGenerationContextBase context, Attribute attribute)
	{
		throw new NotImplementedException();
	}
}