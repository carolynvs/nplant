﻿using System;
using NPlant.MetaModel.ClassDiagraming;
using NPlant.Samples.CIrcularReferfences;
using NUnit.Framework;

namespace NPlant.Tests.Diagrams.ClassDiagrams
{
    [TestFixture]
    public class EdgeScenarios
    {
        [Test]
        public void Circular_References_Dont_Create_Multiple_Classes()
        {
            var simulation = new ClassDiagramSimulation(new ClassDiagram(typeof(Foo)));
            simulation.Simulate();

            Assert.That(simulation.Classes.Count, Is.EqualTo(3));
            Assert.That(simulation.Classes["Foo"].Members["SomeString"].MemberType, Is.EqualTo(typeof(string)));
            Assert.That(simulation.Classes["Foo"].Members["TheBar"].MemberType, Is.EqualTo(typeof(Bar)));

            Assert.That(simulation.Classes["Bar"].Members["SomeDate"].MemberType, Is.EqualTo(typeof(DateTime?)));

            Assert.That(simulation.Classes["Baz"].Members["TheFoo"].MemberType, Is.EqualTo(typeof(Foo)));
        }
    }
}
