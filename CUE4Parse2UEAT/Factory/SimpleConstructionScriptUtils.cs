using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;

namespace CUE4Parse2UEAT.Factory
{
    public static class SimpleConstructionScriptUtils
    {
        public static ResolvedObject? FindSimpleConstructionScript(UObject? uobject)
        {
            if (uobject == null)
            {
                return null;
            }

            return (uobject.Properties.Find(p => "SimpleConstructionScript".Equals(p?.Name.Text))?.Tag as ObjectProperty)?.Value?.ResolvedObject;
        }

        public static string[] GetSimpleConstructionScriptVariables(ResolvedObject? resolvedObject)
        {
            if (resolvedObject == null)
            {
                return Array.Empty<string>();
            }

            HashSet<string> variables = new HashSet<string>();

            if (resolvedObject.TryLoad(out var scsUObject))
            {
                var allNodesPropertyValue = scsUObject?.Properties?.Find(p => "AllNodes".Equals(p?.Name.Text))?.Tag as ArrayProperty;

                if (allNodesPropertyValue?.Value?.Properties != null)
                {
                    foreach (var nodeProperty in allNodesPropertyValue.Value.Properties.Cast<ObjectProperty>())
                    {
                        var scsNodeUObject = nodeProperty?.Value?.Load();
                        var internalVariableNamePropertyValue = scsNodeUObject?.Properties?
                            .Find(p => "InternalVariableName".Equals(p?.Name.Text))?.Tag as NameProperty;
                        var variableName = internalVariableNamePropertyValue?.Value.Text;

                        if (variableName != null)
                        {
                            variables.Add(variableName);
                        }
                    }
                }
            }

            return variables.ToArray();
        }
    }
}
