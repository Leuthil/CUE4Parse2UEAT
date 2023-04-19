# UEATSerializer Project
Defines the UEAssetToolkit format and the ability to serialize it. WIP. This should probably be its own repo at some point.

To serialize an asset in UEAssetToolkit format, fill in all the data of a UAsset object (good luck). Then call `uasset.Serialize()` to get the json.

# CUE4Parse2UEAT Project
Converts CUE4Parse objects to UEATSerializer objects (UEAssetToolkit format). WIP. At the moment only handles CUE4Parse's `IoPackage` (IO Store). Hard-coded for Hogwarts Legacy game files at the moment.

# CUE4Parse2UEAT-CLI Project
Provides a sample of how to use CUE4Parse2UEAT. Very hard-coded at the moment.
