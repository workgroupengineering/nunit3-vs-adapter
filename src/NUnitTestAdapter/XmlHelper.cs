﻿// ***********************************************************************
// Copyright (c) 2010-2021 Charlie Poole, Terje Sandstrom
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Globalization;
using System.Xml;

namespace NUnit.VisualStudio.TestAdapter;

/// <summary>
/// XmlHelper provides static methods for basic XML operations.
/// </summary>
public static class XmlHelper
{
    /// <summary>
    /// Creates a new top level element node.
    /// </summary>
    /// <param name="name">The element name.</param>
    /// <returns>A new XmlNode.</returns>
    public static XmlNode CreateTopLevelElement(string name)
    {
            var doc = new XmlDocument();
            doc.LoadXml("<" + name + "/>");
            return doc.FirstChild;
        }

    public static XmlNode CreateXmlNode(string xml)
    {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.FirstChild;
        }

    public static XmlNode ToXml(this string xml)
    {
            var doc = new XmlDocument();
            var fragment = doc.CreateDocumentFragment();
            fragment.InnerXml = xml;
            doc.AppendChild(fragment);
            return doc.FirstChild;
        }

    /// <summary>
    /// Adds an attribute with a specified name and value to an existing XmlNode.
    /// </summary>
    /// <param name="node">The node to which the attribute should be added.</param>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    public static void AddAttribute(this XmlNode node, string name, string value)
    {
            var attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.Append(attr);
        }

    /// <summary>
    /// Adds a new element as a child of an existing XmlNode and returns it.
    /// </summary>
    /// <param name="node">The node to which the element should be added.</param>
    /// <param name="name">The element name.</param>
    /// <returns>The newly created child element.</returns>
    public static XmlNode AddElement(this XmlNode node, string name)
    {
            XmlNode childNode = node.OwnerDocument.CreateElement(name);
            node.AppendChild(childNode);
            return childNode;
        }

    /// <summary>
    /// Adds the a new element as a child of an existing node and returns it.
    /// A CDataSection is added to the new element using the data provided.
    /// </summary>
    /// <param name="node">The node to which the element should be added.</param>
    /// <param name="name">The element name.</param>
    /// <param name="data">The data for the CDataSection.</param>
    public static XmlNode AddElementWithCDataSection(this XmlNode node, string name, string data)
    {
            var childNode = node.AddElement(name);
            childNode.AppendChild(node.OwnerDocument.CreateCDataSection(data));
            return childNode;
        }

    #region Safe Attribute Access

    /// <summary>
    /// Gets the value of the given attribute.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="name">The name.</param>
    public static string GetAttribute(this XmlNode result, string name)
    {
            var attr = result.Attributes?[name];

            return attr?.Value;
        }

    /// <summary>
    /// Gets the value of the given attribute as an int.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="name">The name.</param>
    /// <param name="defaultValue">The default value.</param>
    public static int GetAttribute(this XmlNode result, string name, int defaultValue)
    {
            var attr = result.Attributes[name];

            return attr == null
                ? defaultValue
                : int.Parse(attr.Value, CultureInfo.InvariantCulture);
        }

    /// <summary>
    /// Gets the value of the given attribute as a double.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="name">The name.</param>
    /// <param name="defaultValue">The default value.</param>
    public static double GetAttribute(this XmlNode result, string name, double defaultValue)
    {
            var attr = result.Attributes[name];

            return attr == null
                ? defaultValue
                : double.Parse(attr.Value, CultureInfo.InvariantCulture);
        }

    /// <summary>
    /// Gets the value of the given attribute as a DateTime.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="name">The name.</param>
    /// <param name="defaultValue">The default value.</param>
    public static DateTime GetAttribute(this XmlNode result, string name, DateTime defaultValue)
    {
            string dateStr = GetAttribute(result, name);
            if (dateStr == null)
                return defaultValue;

            return !DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out var date)
                ? defaultValue
                : date;
        }

    #endregion
}