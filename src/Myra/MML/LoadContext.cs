﻿using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Myra.Attributes;
using FontStashSharp;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.MML
{
	internal class LoadContext: BaseContext
	{
		public Dictionary<string, string> LegacyClassNames = null;
		public Dictionary<string, string> LegacyPropertyNames = null;
		public Dictionary<string, Color> Colors;
		public HashSet<string> NodesToIgnore = null;
		public Func<Type, XElement, object> ObjectCreator = (type, el) => Activator.CreateInstance(type);
		public string[] Namespaces;
		public Assembly Assembly = typeof(Widget).Assembly;
		public Func<Type, string, object> ResourceGetter = null;
		public ITypeResolver TypeResolver;

		private const string UserDataAttributePrefix = "_";

		public void Load<T>(object obj, XElement el, T handler) where T : class
		{
			var type = obj.GetType();
			var handlerType = typeof(T);

			var baseObject = obj as BaseObject;

			List<PropertyInfo> complexProperties, simpleProperties;
			ParseProperties(type, out complexProperties, out simpleProperties);

			string newName;
			foreach (var attr in el.Attributes())
			{
				var propertyName = attr.Name.ToString();
				if (LegacyPropertyNames != null && LegacyPropertyNames.TryGetValue(propertyName, out newName))
				{
					propertyName = newName;
				}

				var property = (from p in simpleProperties where p.Name == propertyName select p).FirstOrDefault();

				if (property != null)
				{
					object value = null;

					var propertyType = property.PropertyType;
					if (propertyType.IsEnum)
					{
						value = Enum.Parse(propertyType, attr.Value);
					}
					else if (propertyType == typeof(Color) || propertyType == typeof(Color?))
					{
						Color color;
						if (Colors != null && Colors.TryGetValue(attr.Value, out color))
						{
							value = color;
						}
						else
						{
							value = ColorStorage.FromName(attr.Value);
							if (value == null)
							{
								throw new Exception(string.Format("Could not find parse color '{0}'", attr.Value));
							}
						}
					}
					else if ((typeof(IBrush).IsAssignableFrom(propertyType) ||
							 propertyType == typeof(SpriteFontBase)) &&
							 !string.IsNullOrEmpty(attr.Value) &&
							 ResourceGetter != null)
					{
						try
						{
							var texture = ResourceGetter(propertyType, attr.Value);
							if (texture == null)
							{
								throw new Exception(string.Format("Could not find resource '{0}'", attr.Value));
							}
							value = texture;

							if (baseObject != null)
							{
								baseObject.Resources[property.Name] = attr.Value;
							}
						}
						catch (Exception)
						{
						}
					}
					else if (propertyType == typeof(Thickness))
					{
						try
						{
							value = Thickness.FromString(attr.Value);
						}
						catch (Exception)
						{
						}
					}
					else
					{
						if (propertyType.IsNullablePrimitive())
						{
							propertyType = propertyType.GetNullableType();
						}

						value = Convert.ChangeType(attr.Value, propertyType, CultureInfo.InvariantCulture);
					}

					property.SetValue(obj, value);
				}
				else if (handler != null && type.GetEvent(attr.Name.LocalName) != null)
				{
					var method = handlerType.GetMethod(attr.Value, BindingFlags.Public | BindingFlags.Instance);
					var eventHandler = type.GetEvent(attr.Name.LocalName);
					if (method == null)
					{
						throw new InvalidOperationException($"Handler of type '{handlerType}' does not contain method '{attr.Value}'. If it does, ensure the method is both public and non-static.");
					}

					var delegateMethod = method.CreateDelegate(eventHandler.EventHandlerType, handler);
					eventHandler.AddEventHandler(obj, delegateMethod);
				}
				else
				{
					// Stow away custom user attributes
					if (propertyName.StartsWith(UserDataAttributePrefix) && baseObject != null)
					{
						baseObject.UserData.Add(propertyName, attr.Value);
					}
				}
			}
			

			var contentProperty = (from p in complexProperties
								   where p.FindAttribute<ContentAttribute>() 
								   != null select p).FirstOrDefault();

			foreach (var child in el.Elements())
			{
				var childName = child.Name.ToString();
				if (NodesToIgnore != null && NodesToIgnore.Contains(childName))
				{
					continue;
				}

				var isProperty = false;
				if (childName.Contains("."))
				{
					// Property name
					var parts = childName.Split('.');
					childName = parts[1];
					isProperty = true;
				}

				if (LegacyPropertyNames != null && LegacyPropertyNames.TryGetValue(childName, out newName))
				{
					childName = newName;
				}

				// Find property
				var property = (from p in complexProperties where p.Name == childName select p).FirstOrDefault();
				if (property != null)
				{
					do
					{
						var value = property.GetValue(obj);
						var asList = value as IList;
						if (asList != null)
						{
							// List
							foreach (var child2 in child.Elements())
							{
								var item = ObjectCreator(property.PropertyType.GenericTypeArguments[0], child2);
								Load(item, child2, handler);
								asList.Add(item);
							}

							break;
						}

						var asDict = value as IDictionary;
						if (asDict != null)
						{
							// Dict
							foreach (var child2 in child.Elements())
							{
								var item = ObjectCreator(property.PropertyType.GenericTypeArguments[1], child2);
								Load(item, child2, handler);

								var id = string.Empty;
								if (child2.Attribute(IdName) != null)
								{
									id = child2.Attribute(IdName).Value;
								}

								asDict[id] = item;
							}

							break;
						}

						if (property.SetMethod == null)
						{
							// Readonly
							Load(value, child, handler);
						}
						else
						{
							var newValue = ObjectCreator(property.PropertyType, child);
							Load(newValue, child, handler);
							property.SetValue(obj, newValue);
						}
						break;
					} while (true);
				}
				else
				{
					// Property not found
					if (isProperty)
					{
						throw new Exception(string.Format("Class {0} doesnt have property {1}", type.Name, childName));
					}

					// Should be widget class name then
					var widgetName = childName;
					if (LegacyClassNames != null && LegacyClassNames.TryGetValue(widgetName, out newName))
					{
						widgetName = newName;
					}

					Type itemType = null;
					foreach(var ns in Namespaces)
					{
						itemType = Assembly.GetType(ns + "." + widgetName);
						if (itemType != null)
						{
							break;
						}
					}

					itemType ??= TypeResolver?.ResolveType(widgetName);
					
					if (itemType != null)
					{
						var item = ObjectCreator(itemType, child);
						Load(item, child, handler);

						if (contentProperty == null)
						{
							throw new Exception(string.Format("Class {0} lacks property marked with ContentAttribute", type.Name));
						}

						var containerValue = contentProperty.GetValue(obj);
						var asList = containerValue as IList;
						if (asList != null)
						{
							// List
							asList.Add(item);
						} else
						{
							// Simple
							contentProperty.SetValue(obj, item);
						}
					}
					else
					{
						throw new Exception(string.Format("Could not resolve tag '{0}'", widgetName));
					}
				}
			}
		}
	}
}
