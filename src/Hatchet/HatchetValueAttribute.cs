using System;

namespace Hatchet;

/// <summary>
/// Use this attribute for specifying custom value serialization for
/// this type. It permits the outputting of a single value.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class HatchetValueAttribute : Attribute;
