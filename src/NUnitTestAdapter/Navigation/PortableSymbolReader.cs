// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NUnit.VisualStudio.TestAdapter.Navigation
{
#if !NET45
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    /// <summary>
    ///     The portable symbol reader.
    /// </summary>
    internal class PortableSymbolReader : ISymbolReader
    {
        /// <summary>
        ///     Key in first dict is Type FullName
        ///     Key in second dict is method name
        /// </summary>
        private Dictionary<string, Dictionary<string, NavigationData>> methodsNavigationDataForType =
            new Dictionary<string, Dictionary<string, NavigationData>>();

        /// <summary>
        /// The cache symbols.
        /// </summary>
        /// <param name="binaryPath">
        /// The binary path.
        /// </param>
        /// <param name="searchPath">
        /// The search path.
        /// </param>
        public void CacheSymbols(string binaryPath, string searchPath)
        {
            this.PopulateCacheForTypeAndMethodSymbols(binaryPath);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            foreach (var methodsNavigationData in this.methodsNavigationDataForType.Values)
            {
                methodsNavigationData.Clear();
            }

            this.methodsNavigationDataForType.Clear();
            this.methodsNavigationDataForType = null;
        }

        /// <summary>
        /// The get navigation data.
        /// </summary>
        /// <param name="declaringTypeName">
        /// The declaring type name.
        /// </param>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <returns>
        /// The <see cref="INavigationData"/>.
        /// </returns>
        public NavigationData GetNavigationData(string declaringTypeName, string methodName)
        {
            NavigationData navigationData = null;
            if (this.methodsNavigationDataForType.ContainsKey(declaringTypeName))
            {
                var methodDict = this.methodsNavigationDataForType[declaringTypeName];
                if (methodDict.ContainsKey(methodName))
                {
                    navigationData = methodDict[methodName];
                }
            }

            return navigationData;
        }

        /// <summary>
        /// The populate cache for type and method symbols.
        /// </summary>
        /// <param name="binaryPath">
        /// The binary path.
        /// </param>
        private void PopulateCacheForTypeAndMethodSymbols(string binaryPath)
        {
            try
            {
                var pdbFilePath = Path.ChangeExtension(binaryPath, ".pdb");
                using (var pdbReader = new PortablePdbReader(new FileStream(pdbFilePath, FileMode.Open, FileAccess.Read)))
                {
                    // At this point, the assembly should be already loaded into the load context. We query for a reference to
                    // find the types and cache the symbol information. Let the loader follow default lookup order instead of
                    // forcing load from a specific path.
                    var asm = Assembly.Load(AssemblyLoadContext.GetAssemblyName(binaryPath));

                    foreach (var type in asm.GetTypes())
                    {
                        // Get declared method infos
                        var methodInfoList = ((TypeInfo)type.GetTypeInfo()).DeclaredMethods;
                        var methodsNavigationData = new Dictionary<string, NavigationData>();

                        foreach (var methodInfo in methodInfoList)
                        {
                            var diaNavigationData = pdbReader.GetDiaNavigationData(methodInfo);
                            if (diaNavigationData != null)
                            {
                                methodsNavigationData[methodInfo.Name] = diaNavigationData;
                            }
                            else
                            {
                                Debug.WriteLine($"Unable to find source information for method: {methodInfo.Name} type: {type.FullName}");
                            }
                        }

                        if (methodsNavigationData.Count != 0)
                        {
                            this.methodsNavigationDataForType[type.FullName] = methodsNavigationData;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PortableSymbolReader: Failed to load symbols for binary: {binaryPath}");
                Debug.WriteLine(ex.Message);
                Dispose();
                throw;
            }
        }
    }
#endif
}
