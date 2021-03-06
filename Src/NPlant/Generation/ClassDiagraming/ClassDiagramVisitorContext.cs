﻿using System;
using System.Collections.Generic;
using NPlant.Core;
using NPlant.MetaModel.ClassDiagraming;

namespace NPlant.Generation.ClassDiagraming
{
    public class ClassDiagramVisitorContext
    {
        private readonly Queue<ClassDescriptor> _unvisitedRelatedClasses = new Queue<ClassDescriptor>();
        private readonly List<ClassDiagramRelationship> _relationships = new List<ClassDiagramRelationship>();
        private readonly List<ClassDescriptor> _visitedRelatedClasses = new List<ClassDescriptor>();

        public ClassDiagramVisitorContext(ClassDiagram diagram, TypeMetaModelSet metaModelSet)
        {
            this.TypeMetaModelSet = metaModelSet;
            this.Diagram = diagram;
        }

        protected TypeMetaModelSet TypeMetaModelSet { get; private set; }

        protected ClassDiagram Diagram { get; set; }

        public void AddRelatedClass(ClassDescriptor parent, ClassDescriptor child, ClassDiagramRelationshipTypes relationshipType, int level, string name = null)
        {
            //todo:  wrap w/ a depth check?

            if (! AlreadyRegistered(child))
                _unvisitedRelatedClasses.Enqueue(child);

            var relationship = new ClassDiagramRelationship(name, parent, child, relationshipType);

            if (!_relationships.Contains(relationship))
                _relationships.Add(relationship);
        }

        private bool AlreadyRegistered(ClassDescriptor child)
        {
            if(_visitedRelatedClasses.Contains(child))
                return true;

            if (_unvisitedRelatedClasses.Contains(child))
                return true;
            
            if (this.Diagram.RootClasses.InnerList.Contains(child))
                return true;

            return false;
        }

        public IEnumerable<ClassDescriptor> VisitedRelatedClasses { get { return _visitedRelatedClasses; } }

        public IEnumerable<ClassDiagramRelationship> Relationships { get { return _relationships; } } 

        public TypeMetaModel GetTypeMetaModel(Type type)
        {
            return this.TypeMetaModelSet[type];
        }

        public void VisitAllRelatedClasses()
        {
            while (_unvisitedRelatedClasses.Count > 0)
            {
                var descriptor = _unvisitedRelatedClasses.Dequeue();

                descriptor.Visit(this);

                _visitedRelatedClasses.Add(descriptor);
            }
        }
    }
}