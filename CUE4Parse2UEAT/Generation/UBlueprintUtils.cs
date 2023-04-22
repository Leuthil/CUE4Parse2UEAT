namespace CUE4Parse2UEAT.Generation
{
    public static class UBlueprintUtils
    {
        public static void PopulateUBlueprintData(CUE4Parse.UE4.Objects.Engine.UBlueprintGeneratedClass cue4ParseUBlueprintGenClass,
            UEATSerializer.UEAT.UBlueprint ueatUBlueprintClass, GenerationContext context)
        {
            // GeneratedVariableNames (string[])
            // (Composed of variables from: SimpleConstructionScript.GetAllNodes.GetVariableName, Timelines.GetVariableName,
            //                              Timelines.GetDirectionPropertyName, Timelines.FlaotTracks, Timelines.VectorTracks,
            //                              Timelines.LinearColorTracks)
            // Timeline data doesn't seem available in CUE4Parse
            var scsObject = SimpleConstructionScriptUtils.FindSimpleConstructionScript(cue4ParseUBlueprintGenClass);
            var scsVariableNames = SimpleConstructionScriptUtils.GetSimpleConstructionScriptVariables(scsObject);
            foreach (var scsVariable in scsVariableNames)
            {
                ueatUBlueprintClass.GeneratedVariableNames.Add(scsVariable);
            }
        }
    }
}
