﻿using System.Collections.Immutable;

namespace TSM.Logic.Data_Parser.Models
{
    internal class LuaModel
    {
        public ImmutableList<LuaModel> Children { get; private set; } = ImmutableList<LuaModel>.Empty;

        public string Key { get; set; }

        public LuaModel? Parent { get; private set; }

        public string? Value { get; set; }

        public void AddChild(LuaModel child)
        {
            child.Parent = this;

            Children = Children.Add(child);
        }

        public void ClearEmptyChildren()
        {
            foreach (var child in Children)
            {
                child.ClearEmptyChildren();
            }

            Children = Children.Where(c => c.Key is not null || c.Children.Count > 0).ToImmutableList();
        }

        public override string ToString()
        {
            return $"{(Key ?? "(NULL)")}: {{{Children.Count}}}";
        }
    }
}