using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
#pragma warning disable 649

namespace Hatchet.Tests.UsageTests.VortexShaderTests
{

    [TestFixture]
    public class DeserializationTests
    {
        private enum QualityLevel
        {
            Unknown = 0,
            Low = 1,
            Medium = 2,
            High = 3,
            Ultra = 4
        }

        private enum Blend
        {
            Unknown = 0,
            Zero = 1,
            One = 2,
            SrcColour = 3,
            OneMinusSrcColour = 4,
            DestColour = 5,
            OneMinusDestColour = 6,
            SrcAlpha = 7,
            OneMinusSrcAlpha = 8,
            DestAlpha = 9,
            OneMinusDestAlpha = 10
        }

        private enum LightPass
        {
            Base,
            Add
        }

        private enum BlendFunction
        {
            Unknown = 0,
            Add = 1,
            Max = 2,
            Min = 3,
            ReverseSubtract = 4,
            Subtract = 5
        }

        private class PassDefinition
        {
            public string Name;
            public LightPass LightPass;
            public bool ZWrite;
            public bool ZTest;
            public Blend SourceBlend;
            public Blend DestinationBlend;
            public BlendFunction BlendFunction;
            public string Vertex;
            public string Fragment;
        }

        private class TechniqueDefinition
        {
            public QualityLevel QualityLevel;
            public List<string> Passes;
        }

        private class DefineDefinitions
        {
            public string Name;
            public string Text;
        }

        private class SourceDefinition
        {
            public string Name;
            public int Queue;
            public List<PassDefinition> Passes;
            public List<TechniqueDefinition> Techniques;
            public List<DefineDefinitions> Defines;
        }

        [Test]
        public void Deserialize_ShaderFile_ObjectShouldBeDeserialized()
        {
            // Arrange
            var input = File.ReadAllText("vortex-engine-test.shader");

            // Act
            var result = HatchetConvert.Deserialize<SourceDefinition>(input);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Shader");
            result.Queue.Should().Be(1234);

            result.Techniques.Should().NotBeNull();
            result.Techniques.Should().HaveCount(1);

            result.Techniques[0].Should().NotBeNull();
            result.Techniques[0].QualityLevel.Should().Be(QualityLevel.High);

            result.Defines.Should().NotBeNull();
            result.Defines.Should().HaveCount(1);
            result.Defines[0].Should().NotBeNull();
            result.Defines[0].Name.Should().Be("example");
            result.Defines[0].Text.Should().Be("example define contents");

            result.Passes.Should().NotBeNull();
            result.Passes.Should().HaveCount(3);

            result.Passes[0].Should().NotBeNull();
            result.Passes[0].Name.Should().Be("pass01");
            result.Passes[0].ZWrite.Should().BeTrue();

            result.Passes[1].Should().NotBeNull();
            result.Passes[1].Name.Should().Be("pass02");
            result.Passes[1].ZWrite.Should().BeFalse();

            result.Passes[2].Should().NotBeNull();
            result.Passes[2].Name.Should().Be("pass03");
            result.Passes[2].ZWrite.Should().BeFalse();
            

        }
        
    }
}