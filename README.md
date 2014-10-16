Hatchet
=======

**Because your data format is bad and you should feel bad.**

```
{
  /* 2014-10-15 Current definition of hatchet */
  name "Hatchet"
  isAwesome true
  cost 0
  durability 9001 // todo: nerf durability?
  description ![
     For when XML isnt cool enough, JSON isnt smart enough, you've given up
     trying to explain why YAML doesnt like tabs, and you find the lack of
     features disturbing. This is a text block by the way,
     and single quotes break the syntax highlighting.
     !]
  versions [0.1 0.2 0.3 0.4]
  tagcloud ['json' xml "data-storage"]
  dependencies [
    { name "nunit" version "2" }
    { name "fluentassertions" version "" } // todo: find actual version number
  ]
}

```

**Hatchet is a text-based data format similar to JSON and XML.**

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

  1. Obtain the source.
  2. Copy the Hatchet.csproj into your solution.
  3. *nuget package coming soon (tm).*

## Usage

  *(For examples see Hatchet.Tests.csproj)*
  
```
  public void Wow()
  {
     var input = "{ welcomeMessage 'Hello, welcome to Hatchet!' }";
     var parser = new Hatchet.Parser();
     result = (string)parser.Parse(ref input);
     Console.WriteLine(result);
  }
```

## License

MIT

## Feature Roadmap

  * Improved exception handling of malformed Hatchet (.axe) input.
  * Deserialization to types ``` var myObject = hatchetSerializer.Deserialize(input) ```
  * Serialization to Hatchet.
  * Stream support.
