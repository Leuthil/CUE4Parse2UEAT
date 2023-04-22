using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;

namespace CUE4Parse2UEAT.Generation
{
    public static class SimpleConstructionScriptUtils
    {
        public static ResolvedObject? FindSimpleConstructionScript(UObject? uobject)
        {
            if (uobject == null)
            {
                return null;
            }

            return UObjectUtils.GetPropertyValue<ObjectProperty>(uobject, "SimpleConstructionScript")?.Value?.ResolvedObject;
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
                var allNodesPropertyValue = UObjectUtils.GetPropertyValue<ArrayProperty>(scsUObject, "AllNodes");

                if (allNodesPropertyValue?.Value?.Properties != null)
                {
                    foreach (var nodeProperty in allNodesPropertyValue.Value.Properties.Cast<ObjectProperty>())
                    {
                        var scsNodeUObject = nodeProperty?.Value?.Load();
                        var internalVariableNamePropertyValue = UObjectUtils.GetPropertyValue<NameProperty>(scsNodeUObject, "InternalVariableName");
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
