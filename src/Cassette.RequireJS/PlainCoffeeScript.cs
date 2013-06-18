using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cassette.Utilities;
using Microsoft.Ajax.Utilities;

namespace Cassette.RequireJS
{
    class PlainCoffeeScript : Module, IAssetTransformer
    {
        readonly IModuleInitializer modules;
        readonly SimpleJsonSerializer jsonSerializer;

        public PlainCoffeeScript(IAsset asset, Bundle bundle, IModuleInitializer modules) 
            : base(asset, bundle)
        {
            this.modules = modules;
            jsonSerializer = new SimpleJsonSerializer();
            asset.AddAssetTransformer(this);
        }

        public string ModuleReturnExpression { get; set; }

        public Func<Stream> Transform(Func<Stream> openSourceStream, IAsset asset)
        {
            return () =>
            {
                var source = openSourceStream().ReadToEnd();
                var output = Transform(source, asset);
                return output.AsStream();
            };
        }

        string Transform(string source, IAsset asset)
        {
            if (ModuleReturnExpression != null)
            {
                return ModuleWithReturn(source, ModuleReturnExpression, asset);
            }
            
            if (TopLevelVariableFinder.JavaScriptContainsTopLevelVariable(source, Alias))
            {
                return ModuleWithReturn(source, Alias, asset);
            }

            return ModuleWithoutReturn(source, asset);
        }

        IEnumerable<string> DependencyPaths
        {
            get
            {
                return Asset.References
                    .Select(r => r.ToPath)
                    .Where(p => modules.RequireJsScriptPath != p)
                    .Select(p => modules[p].ModulePath);
            }
        }

        IEnumerable<string> DependencyAliases
        {
            get 
            {
                return Asset.References
                    .Select(r => r.ToPath)
                    .Where(p => modules.RequireJsScriptPath != p)
                    .Select(p => modules[p].Alias);
            }
        }

        class TopLevelVariableFinder : TreeVisitor
        {
            public static bool JavaScriptContainsTopLevelVariable(string javaScriptSource, string variableName)
            {
                var parser = new JSParser(javaScriptSource);
                var tree = parser.Parse(new CodeSettings());
                var finder = new TopLevelVariableFinder(variableName);
                tree.Accept(finder);
                return finder.found;
            }

            readonly string varName;
            bool found;

            TopLevelVariableFinder(string varName)
            {
                this.varName = varName;
            }

            public override void Visit(VariableDeclaration node)
            {
                if (node.EnclosingScope is GlobalScope && node.Identifier == varName)
                {
                    found = true;
                }

                base.Visit(node);
            }
        }

        string ModuleWithoutReturn(string source, IAsset asset)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("ModuleWithReturn Asset: {0}", asset.Path));
            System.Diagnostics.Debug.WriteLine(String.Format("Detected Coffeescript Asset: {0}", asset.Path));
            return source;
        }

        string ModuleWithReturn(string source, string export, IAsset asset)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("ModuleWithReturn Asset: {0}", asset.Path));
            System.Diagnostics.Debug.WriteLine(String.Format("Detected Coffeescript Asset: {0}", asset.Path));
            return source;
        }
    }
}