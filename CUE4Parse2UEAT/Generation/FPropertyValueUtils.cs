using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse2UEAT.Factory;
using UEATSerializer.UE;
using UEATSerializer.UEAT;

namespace CUE4Parse2UEAT.Generation
{
    public static class FPropertyValueUtils
    {
        public static FPropertyValue? CreateFPropertyValue(FPropertyTagType propertyTagType, IPackageObjectFactory packageObjectFactory)
        {
            switch (propertyTagType)
            {
                case ArrayProperty arrayProperty:
                    FPropertyArrayValue arrayProp = new FPropertyArrayValue();
                    foreach (var property in arrayProperty.Value.Properties)
                    {
                        arrayProp.Items.Add(CreateFPropertyValue(property, packageObjectFactory));
                    }
                    return arrayProp;
                case BoolProperty boolProperty:
                    FBoolPropertyValue boolProp = new FBoolPropertyValue();
                    boolProp.Value = boolProperty.Value;
                    return boolProp;
                case ByteProperty byteProperty:
                    FNumericPropertyValue byteProp = new FNumericPropertyValue();
                    byteProp.LongValue = byteProperty.Value;
                    return byteProp;
                case ClassProperty classProperty:
                    FObjectPropertyBaseValue objProp = new FObjectPropertyBaseValue();
                    objProp.Object = packageObjectFactory.CreatePackageObject(classProperty.Value);
                    return objProp;
                case DoubleProperty doubleProperty:
                    FNumericPropertyValue doubleProp = new FNumericPropertyValue();
                    doubleProp.DoubleValue = doubleProperty.Value;
                    return doubleProp;
                case EnumProperty enumProperty:
                    FEnumPropertyValue enumProp = new FEnumPropertyValue();
                    enumProp.Value = enumProperty.Value.Text;
                    return enumProp;
                case FieldPathProperty fieldPathProperty:
                    FFieldPathPropertyValue fieldPathProp = new FFieldPathPropertyValue();
                    fieldPathProp.Value = string.Join('/', fieldPathProperty.Value.Path.Select(n => n.Text)); // TODO: This is definitely wrong
                    return fieldPathProp;
                case FloatProperty floatProperty:
                    FNumericPropertyValue floatProp = new FNumericPropertyValue();
                    floatProp.DoubleValue = floatProperty.Value;
                    return floatProp;
                case Int16Property int16Property:
                    FNumericPropertyValue int16Prop = new FNumericPropertyValue();
                    int16Prop.LongValue = int16Property.Value;
                    return int16Prop;
                case Int64Property int64Property:
                    FNumericPropertyValue int64Prop = new FNumericPropertyValue();
                    int64Prop.LongValue = int64Property.Value;
                    return int64Prop;
                case Int8Property int8Property:
                    FNumericPropertyValue int8Prop = new FNumericPropertyValue();
                    int8Prop.LongValue = int8Property.Value;
                    return int8Prop;
                case IntProperty intProperty:
                    FNumericPropertyValue intProp = new FNumericPropertyValue();
                    intProp.LongValue = intProperty.Value;
                    return intProp;
                case InterfaceProperty interfaceProperty:
                    FInterfacePropertyValue interfaceProp = new FInterfacePropertyValue();
                    interfaceProp.Object = packageObjectFactory.CreatePackageObject(interfaceProperty.Value?.Object);
                    return interfaceProp;
                case MapProperty mapProperty:
                    FMapPropertyValue mapProp = new FMapPropertyValue();
                    foreach (var innerMapProperty in mapProperty.Value.Properties)
                    {
                        mapProp.KeyValuesPairs.Add(
                            CreateFPropertyValue(innerMapProperty.Key, packageObjectFactory),
                            CreateFPropertyValue(innerMapProperty.Value, packageObjectFactory));
                    }
                    return mapProp;
                case MulticastDelegateProperty multicastDelegateProperty:
                    FMulticastDelegatePropertyValue multicastDelegateProp = new FMulticastDelegatePropertyValue();
                    return multicastDelegateProp;
                case NameProperty nameProperty:
                    FNamePropertyValue nameProp = new FNamePropertyValue();
                    nameProp.Name = nameProperty.Value.Text;
                    return nameProp;
                case ObjectProperty objectProperty:
                    FObjectPropertyBaseValue objectPropertyBaseProp = new FObjectPropertyBaseValue();
                    objectPropertyBaseProp.Object = packageObjectFactory.CreatePackageObject(objectProperty.Value);
                    return objectPropertyBaseProp;
                case SetProperty setProperty:
                    FSetPropertyValue setProp = new FSetPropertyValue();
                    foreach (var innerSetProperty in setProperty.Value.Properties)
                    {
                        setProp.Values.Add(CreateFPropertyValue(innerSetProperty, packageObjectFactory));
                    }
                    return setProp;
                case SoftObjectProperty softObjectProperty:
                    FSoftObjectPropertyValue softObjectProp = new FSoftObjectPropertyValue();
                    softObjectProp.Path = softObjectProperty.Value.AssetPathName.Text;
                    return softObjectProp;
                case StrProperty strProperty:
                    FStrPropertyValue strProp = new FStrPropertyValue();
                    strProp.Value = strProperty.Value;
                    return strProp;
                case TextProperty textProperty:
                    FTextPropertyValue textProp = new FTextPropertyValue();
                    textProp.Value = textProperty.Value.Text;
                    return textProp;
                case UInt16Property uint16Property:
                    FNumericPropertyValue uint16Prop = new FNumericPropertyValue();
                    uint16Prop.ULongValue = uint16Property.Value;
                    return uint16Prop;
                case UInt32Property uint32Property:
                    FNumericPropertyValue uint32Prop = new FNumericPropertyValue();
                    uint32Prop.ULongValue = uint32Property.Value;
                    return uint32Prop;
                case UInt64Property uint64Property:
                    FNumericPropertyValue uint64Prop = new FNumericPropertyValue();
                    uint64Prop.ULongValue = uint64Property.Value;
                    return uint64Prop;
                case StructProperty structProperty:
                    switch (structProperty.Value.StructType)
                    {
                        case FDateTime dateTimeStructProperty:
                            FDateTimeStructPropertyValue dateTimeStructProp = new FDateTimeStructPropertyValue();
                            dateTimeStructProp.Ticks = (ulong)dateTimeStructProperty.Ticks;
                            return dateTimeStructProp;
                        case CUE4Parse.UE4.Objects.Core.Misc.FGuid guidProperty:
                            UEATSerializer.UEAT.FGuidPropertyValue guidProp = new UEATSerializer.UEAT.FGuidPropertyValue(
                                guidProperty.A, guidProperty.B, guidProperty.C, guidProperty.D);
                            return guidProp;
                        case FStructFallback fallbackStructProperty:
                            FFallbackStructPropertyValue fallbackStructProp = new FFallbackStructPropertyValue();
                            foreach (var fallbackStructInnerProperty in fallbackStructProperty.Properties)
                            {
                                fallbackStructProp.Properties.Add(KeyValuePair.Create(
                                    fallbackStructInnerProperty.Name.Text,
                                    CreateFPropertyValue(fallbackStructInnerProperty.Tag, packageObjectFactory)));
                            }
                            return fallbackStructProp;
                    }

                    // TODO: UEATSerializer should have definitions of many (all?) F* struct types
                    // handle automatically for now

                    break;
                case DelegateProperty delegateProperty:
                case AssetObjectProperty assetObjectProperty:
                case LazyObjectProperty lazyObjectProperty:
                default:
                    break;
            }

            var genericProp = new GenericPropertyValue();
            genericProp.Value = propertyTagType;
            return genericProp;
        }
    }
}
