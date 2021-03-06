﻿using System;
using System.Collections.Generic;
using System.Reflection;
using NPlant.Core;
using NPlant.Generation.ClassDiagraming;
using NPlant.MetaModel.ClassDiagraming;

namespace NPlant
{
    public class ClassDiagram : IDiagram
    {
        private readonly TypeMetaModelSet _types = new TypeMetaModelSet();
        private string _name;
        private string _title;
        private readonly KeyedList<AssemblyDescriptor> _assemblyDescriptors = new KeyedList<AssemblyDescriptor>();
        private readonly KeyedList<ClassDescriptor> _classDescriptors = new KeyedList<ClassDescriptor>();
        private readonly ClassDiagramOptions _generationOptions;
        private ClassDiagramLegend _legend;
        private readonly List<ClassDiagramNote> _notes = new List<ClassDiagramNote>();
 
        public ClassDiagram(Type type, params Type[] types): this()
        {
            type.CheckForNullArg("type");

            this.AddClass(new ReflectedClassDescriptor(type));

            if (types != null)
            {
                foreach (var t in types)
                    this.AddClass(new ReflectedClassDescriptor(t));
            }
        }

        public ClassDiagram()
        {
            _generationOptions = new ClassDiagramOptions(this);
            _name = this.GetType().Name;
        }

        public TypeMetaModelSet Types { get { return _types; } }

        protected RootClassDescriptor<T> AddClass<T>()
        {
            var classDescriptor = new RootClassDescriptor<T>();

            this.AddClass(classDescriptor);
            _classDescriptors.Add(classDescriptor);

            return classDescriptor;
        }

        protected ClassDiagram AddAssemblyOf<T>()
        {
            return AddAssembly(typeof(T).Assembly);
        }

        protected ClassDiagram AddAssembly(Assembly assembly)
        {
            _assemblyDescriptors.Add(new AssemblyDescriptor(assembly));
            return this;
        }

        protected ClassDiagram AddAllSubClassesOff<T>()
        {
            foreach (var assembly in _assemblyDescriptors.InnerList)
            {
                var types = assembly.Assembly.GetTypesExtending<T>();

                foreach (var type in types)
                    this.AddClass(new ReflectedClassDescriptor(type), false);
            }

            return this;
        }

        internal void AddReflectedClass(int level, Type type)
        {
            var descriptor = new ReflectedClassDescriptor(type);

            descriptor.SetLevel(level);
            this.AddClass(descriptor);
        }

        public void AddClass(ClassDescriptor descriptor, bool addAssembly = true)
        {
            _classDescriptors.Add(descriptor.CheckForNullArg("descriptor"));

            if(addAssembly)
                AddAssembly(descriptor.ReflectedType.Assembly);
        }

        public KeyedList<ClassDescriptor> RootClasses { get { return _classDescriptors; } }

        IDiagramGenerator IDiagram.CreateGenerator()
        {
            return new ClassDiagramGenerator(this);
        }

        internal IDiagramFormatter CreateFormatter(ClassDiagramVisitorContext context)
        {
            return new ClassDiagramFormatter(this, context);
        }

        public string Name { get { return _name; } }

        public ClassDiagram Named(string name)
        {
            _name = name;

            return this;
        }

        public string Title { get { return _title; } }

        public ClassDiagram Titled(string title)
        {
            _title = title;

            return this;
        }

        public ClassDiagramOptions GenerationOptions
        {
            get { return _generationOptions; }
        }

        public int? DepthLimit { get; internal set; }

        public ClassDiagramLegend LegendOf(string legend)
        {
            _legend = new ClassDiagramLegend(this, legend);
            return _legend;
        }

        internal ClassDiagramLegend Legend { get { return _legend; } }

        internal IEnumerable<ClassDiagramNote> Notes
        {
            get { return _notes; }
        }

        public ClassDiagramNote AddNote(string line)
        {
            var note = new ClassDiagramNote(line, this);

            _notes.Add(note);

            return note;
        }

        public ClassDiagramVisitorContext CreateGenerationContext()
        {
            return new ClassDiagramVisitorContext(this, _types);
        }

        internal string GetClassColor(ClassDescriptor @class)
        {
            return null;
        }
    }
}
