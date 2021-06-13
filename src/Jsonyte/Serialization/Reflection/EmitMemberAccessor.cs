#if NETCOREAPP || NETFRAMEWORK
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jsonyte.Serialization.Reflection
{
    internal class EmitMemberAccessor : MemberAccessor
    {
        public override Func<object?> CreateCreator(Type type)
        {
            var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);

            var method = new DynamicMethod(
                ConstructorInfo.ConstructorName,
                type,
                Type.EmptyTypes,
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            if (constructor == null)
            {
                var local = generator.DeclareLocal(type);

                generator.Emit(OpCodes.Ldloca_S, local);
                generator.Emit(OpCodes.Initobj, type);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, constructor);
            }

            generator.Emit(OpCodes.Ret);

            return (Func<object>) method.CreateDelegate(typeof(Func<object>));
        }

        public override Func<object[], object?> CreateParameterizedCreator(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            var method = new DynamicMethod(
                ConstructorInfo.ConstructorName,
                constructor.DeclaringType,
                new[] {typeof(object[])},
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            for (var i = 0; i < parameters.Length; i++)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4_S, i);
                generator.Emit(OpCodes.Ldelem_Ref);
                generator.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
            }

            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Ret);

            return (Func<object[], object?>) method.CreateDelegate(typeof(Func<object[], object?>));
        }

        public override Func<object, T> CreatePropertyGetter<T>(PropertyInfo property)
        {
            var declaringType = property.DeclaringType;

            var method = new DynamicMethod(
                property.Name + "Getter",
                property.PropertyType,
                new[] {JsonApiTypes.Object},
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (declaringType!.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, declaringType);
                generator.Emit(OpCodes.Call, property.GetMethod!);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, declaringType);
                generator.Emit(OpCodes.Callvirt, property.GetMethod!);
            }

            generator.Emit(OpCodes.Ret);

            return (Func<object, T>) method.CreateDelegate(typeof(Func<object, T>));
        }

        public override Action<object, T> CreatePropertySetter<T>(PropertyInfo property)
        {
            var declaringType = property.DeclaringType;

            var method = new DynamicMethod(
                property.Name + "Setter",
                typeof(void),
                new[] {JsonApiTypes.Object, property.PropertyType},
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (declaringType!.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, declaringType);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, declaringType);
            }

            generator.Emit(OpCodes.Ldarg_1);

            if (declaringType.IsValueType)
            {
                generator.Emit(OpCodes.Call, property.SetMethod!);
            }
            else
            {
                generator.Emit(OpCodes.Callvirt, property.SetMethod!);
            }

            generator.Emit(OpCodes.Ret);

            return (Action<object, T?>) method.CreateDelegate(typeof(Action<object, T?>));
        }

        public override Func<object, T> CreateFieldGetter<T>(FieldInfo field)
        {
            var declaringType = field.DeclaringType;

            var method = new DynamicMethod(
                field.Name + "Getter",
                field.FieldType,
                new[] {JsonApiTypes.Object},
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (declaringType!.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, declaringType);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, declaringType);
            }

            generator.Emit(OpCodes.Ldfld, field);

            if (field.FieldType.IsValueType && field.FieldType != typeof(T))
            {
                generator.Emit(OpCodes.Box, field.FieldType);
            }

            generator.Emit(OpCodes.Ret);

            return (Func<object, T>) method.CreateDelegate(typeof(Func<object, T>));
        }

        public override Action<object, T> CreateFieldSetter<T>(FieldInfo field)
        {
            var declaringType = field.DeclaringType;

            var method = new DynamicMethod(
                field.Name + "Setter",
                typeof(void),
                new[] {JsonApiTypes.Object, field.FieldType},
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (declaringType!.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, declaringType);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, declaringType);
            }

            generator.Emit(OpCodes.Ldarg_1);

            if (field.FieldType.IsValueType && field.FieldType != typeof(T))
            {
                generator.Emit(OpCodes.Unbox_Any, field.FieldType);
            }

            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);

            return (Action<object, T?>) method.CreateDelegate(typeof(Action<object, T?>));
        }
    }
}
#endif
