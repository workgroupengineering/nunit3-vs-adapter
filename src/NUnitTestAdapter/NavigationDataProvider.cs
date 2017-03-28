// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Adapted from Microsoft.VisualStudio.TestPlatform.ObjectModel.DiaSession

using NUnit.VisualStudio.TestAdapter.Navigation;
using System;

namespace NUnit.VisualStudio.TestAdapter
{
    public class NavigationDataProvider : IDisposable
    {
        /// <summary>
        /// Characters that should be stripped off the end of test names.
        /// </summary>
        private static readonly char[] TestNameStripChars = { '(', ')', ' ' };

        /// <summary>
        /// The symbol reader.
        /// </summary>
        private readonly ISymbolReader _symbolReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiaSession"/> class.
        /// </summary>
        /// <param name="binaryPath">
        /// The binary path.
        /// </param>
        public NavigationDataProvider(string binaryPath)
            : this(binaryPath, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiaSession"/> class.
        /// </summary>
        /// <param name="binaryPath">
        /// The binary path is assembly path Ex: \path\to\bin\Debug\simpleproject.dll
        /// </param>
        /// <param name="searchPath">
        /// search path.
        /// </param>
        public NavigationDataProvider(string binaryPath, string searchPath)
            :
#if NET45
            this(binaryPath, searchPath, new FullSymbolReader())
#else
            this(binaryPath, searchPath, new PortableSymbolReader())
#endif
        {
        }

        internal NavigationDataProvider(string binaryPath, string searchPath, ISymbolReader symbolReader)
        {
            _symbolReader = symbolReader;
            if(string.IsNullOrWhiteSpace(binaryPath))
                throw new ArgumentException("binaryPath cannot be null or empty", nameof(binaryPath));
            _symbolReader.CacheSymbols(binaryPath, searchPath);
        }

        /// <summary>
        /// Dispose symbol reader
        /// </summary>
        public void Dispose()
        {
            _symbolReader?.Dispose();
        }

        /// <summary>
        /// Gets the navigation data for a method declared in a type.
        /// </summary>
        /// <param name="declaringTypeName"> The declaring type name. </param>
        /// <param name="methodName"> The method name. </param>
        /// <returns> The <see cref="INavigationData" /> for that method. </returns>
        /// <remarks> Leaving this method in place to preserve back compatibility. </remarks>
        public NavigationData GetNavigationData(string declaringTypeName, string methodName)
        {
            return GetNavigationDataForMethod(declaringTypeName, methodName);
        }

        /// <summary>
        /// Gets the navigation data for a method declared in a type.
        /// </summary>
        /// <param name="declaringTypeName"> The declaring type name. </param>
        /// <param name="methodName"> The method name. </param>
        /// <returns> The <see cref="INavigationData" /> for that method. </returns>
        public NavigationData GetNavigationDataForMethod(string declaringTypeName, string methodName)
        {
            if (string.IsNullOrWhiteSpace(declaringTypeName))
                throw new ArgumentException("declaringTypeName cannot be null or empty", nameof(declaringTypeName));
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("methodName cannot be null or empty", nameof(methodName));
            methodName = methodName.TrimEnd(TestNameStripChars);
            return _symbolReader.GetNavigationData(declaringTypeName, methodName);
        }
    }
}
