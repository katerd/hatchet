Hatchet
=======

**A data interchange format.**

```
{
  /* 2014-10-15 Current definition of hatchet */
  name "Hatchet"
  isAwesome true
  cost 0
  durability 9001 // todo: nerf durability?
  description ![
     For when XML isn't cool enough, JSON isnt smart enough, you've given up
     trying to explain why YAML doesn't like tabs, and you find the lack of
     features disturbing. This is a text block by the way.
     ]!
  versions [0.1 0.2 0.3 0.4]
  tagcloud ['json' xml "data-storage"]
  dependencies [
    { name "nunit" version "2" }
    { name "fluentassertions" version "" } // todo: find actual version number
  ]
}

```

Hatchet is a text-based data format with features borrowed or ignored from JSON, YAML and XML. The format is intended more as a method of storing configuration and textual assets as opposed to data interchange.

##Features

  - Block and in-line comments.
  - Text blocks **![Like this]!**
  - Object definitions with key-value pairs.
  - Lists **[1.0 2.0 3.0 -4.0]**
  - Prevention of tooth decay by removing syntactic sugar.
  - Implicit string parsing **[this will be parsed as a list of strings]**

## Requirements

  - .net framework 4.5

## Installation

via the sauce:

  1. Obtain the source.
  2. Copy the Hatchet.csproj into your solution.
  
or via nuget:

```
PM> Install-Package Hatchet
```

## Usage

  *(For examples see Hatchet.Tests.csproj)*
  
```
  public void UsingTheParser()
  {
     var input = "{ welcomeMessage 'Hello, welcome to Hatchet!' }";
     var parser = new Hatchet.Parser();
     result = (Dictionary<string, object>)parser.Parse(ref input);
     Console.WriteLine(result["welcomeMessage"]);
  }
```

```
  public void UsingTheConverter()
  {
    var input = "{ exampleString 'This is an example' exampleInt 1234 }";
    var result = HatchetConvert.Deserialize<Test>(ref input);
    Console.WriteLine(string.Format("string = {0}", result.ExampleString));
    Console.WriteLine(string.Format("int    = {0}", result.ExampleInt));
  }
  
  public class Test
  {
    public string ExampleString { get; set; }
    public int ExampleInt { get; set; }
  }
```

## License

MIT

## Feature Roadmap

  * ~~Deserialization to types ``` var myObject = hatchetSerializer.Deserialize(input) ```~~ **Done!**  
  * Improved exception handling of malformed Hatchet (.axe) input.
  * ~~Serialization to Hatchet.~~
  * Stream support.
