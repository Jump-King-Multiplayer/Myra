﻿using System.ComponentModel;
using System.Reflection;
using Myra.Graphics2D;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Utility
{
	public static class Serialization
	{
		public static bool HasDefaultValue(this PropertyInfo property, object value)
		{
			if (property.PropertyType == typeof(Thickness) &&
				value.Equals(Thickness.Zero))
			{
				// Skip empty Thickness
				return true;
			}

			var defaultAttribute = property.FindAttribute<DefaultValueAttribute>();
			if (defaultAttribute != null)
			{
				object defaultAttributeValue = defaultAttribute.Value;
				// If property is of Color type, than DefaultValueAttribute should contain its name or hex
				if (property.PropertyType == typeof(Color))
				{
					defaultAttributeValue = ColorStorage.FromName(defaultAttributeValue.ToString()).Value;
				}

				if (property.PropertyType == typeof(string) && 
					string.IsNullOrEmpty((string)defaultAttributeValue) && 
					string.IsNullOrEmpty((string)value))
				{
					// Skip empty/null string
					return true;
				}

				if (Equals(value, defaultAttributeValue))
				{
					// Skip default
					return true;
				}
			}

			return false;
		}

		public static Rectangle ParseRectangle(this string s)
		{
			var parts = s.Split(',');
			if (parts.Length != 4)
			{
				throw new Exception("Rectangle should consist of 4 numbers");
			}

			Rectangle result;
			result.X = int.Parse(parts[0].Trim());
			result.Y = int.Parse(parts[1].Trim());
			result.Width = int.Parse(parts[2].Trim());
			result.Height = int.Parse(parts[3].Trim());

			return result;
		}
	}
}