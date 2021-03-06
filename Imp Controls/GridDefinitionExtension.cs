﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Imp.Controls
{
    public class GridDefinitionExtension : MarkupExtension
    {
        public string Name { get; set; }

        public GridDefinitionExtension(string name)
        {
            Name = name;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var refExt = new Reference(Name);
            var definition = refExt.ProvideValue(serviceProvider);
            if (definition is DefinitionBase)
            {
                var grid = (definition as FrameworkContentElement).Parent as Grid;
                if (definition is RowDefinition)
                {
                    return grid.RowDefinitions.IndexOf(definition as RowDefinition);
                }
                else
                {
                    return grid.ColumnDefinitions.IndexOf(definition as ColumnDefinition);
                }
            }
            else
            {
                throw new Exception("Found object is neither a RowDefinition nor a ColumnDefinition");
            }
        }
    }
}
