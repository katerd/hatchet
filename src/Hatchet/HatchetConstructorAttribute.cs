using System;

namespace Hatchet;

/// <summary>
/// For types that do not have a default constructor, the preferred constructor
/// should be identified by adding this attribute to it.
/// 
/// For types that are to be initialized by a single string property this attribute
/// can added to a static constructor method that take a single string argument
/// and returns the required instance.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
public class HatchetConstructorAttribute : Attribute
{

}