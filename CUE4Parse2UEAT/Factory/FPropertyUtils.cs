using CUE4Parse.UE4.Assets;

namespace CUE4Parse2UEAT.Factory
{
    public static class FPropertyUtils
    {
        public static UEATSerializer.UEAT.FProperty? CreateFProperty(CUE4Parse.UE4.Objects.UObject.FProperty prop, IoPackage package)
        {
            if (prop == null)
            {
                return null;
            }

            UEATSerializer.UEAT.FProperty? property;

            switch (prop)
            {
                case CUE4Parse.UE4.Objects.UObject.FEnumProperty enumProp:
                    var enumProperty = new UEATSerializer.UEAT.FEnumProperty();
                    enumProperty.Enum = PackageObjectUtils.CreatePackageObject(enumProp.Enum.ResolvedObject, package);
                    enumProperty.UnderlyingProp = (UEATSerializer.UEAT.FNumericProperty)CreateFProperty(enumProp.UnderlyingProp, package);
                    property = enumProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FStructProperty structProp:
                    var structProperty = new UEATSerializer.UEAT.FStructProperty();
                    structProperty.Struct = PackageObjectUtils.CreatePackageObject(structProp.Struct.ResolvedObject, package);
                    property = structProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FArrayProperty arrayProp:
                    var arrayProperty = new UEATSerializer.UEAT.FArrayProperty();
                    arrayProperty.Inner = CreateFProperty(arrayProp.Inner, package);
                    property = arrayProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FBoolProperty boolProp:
                    var boolProperty = new UEATSerializer.UEAT.FBoolProperty();
                    boolProperty.BoolSize = boolProp.ElementSize;
                    boolProperty.NativeBool = boolProp.bIsNativeBool;
                    property = boolProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FByteProperty byteProp:
                    var byteProperty = new UEATSerializer.UEAT.FByteProperty();
                    byteProperty.Enum = PackageObjectUtils.CreatePackageObject(byteProp.Enum.ResolvedObject, package);
                    property = byteProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FClassProperty classProp:
                    var classProperty = new UEATSerializer.UEAT.FClassProperty();
                    classProperty.MetaClass = PackageObjectUtils.CreatePackageObject(classProp.MetaClass.ResolvedObject, package);
                    property = classProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FInterfaceProperty interfaceProp:
                    var interfaceProperty = new UEATSerializer.UEAT.FInterfaceProperty();
                    interfaceProperty.InterfaceClass = PackageObjectUtils.CreatePackageObject(interfaceProp.InterfaceClass.ResolvedObject, package);
                    property = interfaceProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FMapProperty mapProp:
                    var mapProperty = new UEATSerializer.UEAT.FMapProperty();
                    mapProperty.KeyProp = CreateFProperty(mapProp.KeyProp, package);
                    mapProperty.ValueProp = CreateFProperty(mapProp.ValueProp, package);
                    property = mapProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FSetProperty setProp:
                    var setProperty = new UEATSerializer.UEAT.FSetProperty();
                    setProperty.ElementType = CreateFProperty(setProp.ElementProp, package);
                    property = setProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FSoftClassProperty softClassProp:
                    var softClassProperty = new UEATSerializer.UEAT.FSoftClassProperty();
                    softClassProperty.MetaClass = PackageObjectUtils.CreatePackageObject(softClassProp.MetaClass.ResolvedObject, package);
                    property = softClassProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FFieldPathProperty fieldPathProp:
                    var fieldPathProperty = new UEATSerializer.UEAT.FFieldPathProperty();
                    fieldPathProperty.PropertyClass = fieldPathProp.PropertyClass.Text;
                    property = fieldPathProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FObjectProperty objectProp:
                    var objectPropertyBaseProperty = new UEATSerializer.UEAT.FObjectPropertyBase();
                    objectPropertyBaseProperty.PropertyClass = PackageObjectUtils.CreatePackageObject(objectProp.PropertyClass.ResolvedObject, package);
                    property = objectPropertyBaseProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FDelegateProperty delegateProp:
                    var delegateProperty = new UEATSerializer.UEAT.FDelegateProperty();
                    delegateProperty.SignatureFunction = delegateProp.SignatureFunction.Name;
                    property = delegateProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FMulticastDelegateProperty multicastDelegateProp:
                    var multicastDelegateProperty = new UEATSerializer.UEAT.FMulticastDelegateProperty();
                    multicastDelegateProperty.SignatureFunction = multicastDelegateProp.SignatureFunction.Name;
                    property = multicastDelegateProperty;
                    break;
                case CUE4Parse.UE4.Objects.UObject.FNumericProperty:
                    // only declared so it can be referenced by other FProperty definitions
                    property = new UEATSerializer.UEAT.FNumericProperty();
                    break;
                default:
                    property = new UEATSerializer.UEAT.FProperty();
                    break;
            }

            // This should have values like "ObjectProperty", "BoolProperty", etc.
            // CUE4Parse FProperty types are named as they are UE, like "FObjectProperty", "FBoolProperty", so we just take off the "F"
            property.ObjectClass = prop.GetType().Name.Substring(1);
            property.ObjectName = prop.Name.Text;
            property.ArrayDim = prop.ArrayDim;
            property.PropertyFlags = prop.PropertyFlags;
            property.RepNotifyFunc = prop.RepNotifyFunc.Text;
            property.BlueprintReplicationCondition = (int)prop.BlueprintReplicationCondition;

            return property;
        }
    }
}
