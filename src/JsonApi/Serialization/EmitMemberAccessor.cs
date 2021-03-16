using System;
using System.Reflection;
using System.Reflection.Emit;

namespace JsonApi.Serialization
{
#if NETCOREAPP || NETFRAMEWORK
    internal class EmitMemberAccessor : IMemberAccessor
    {
        private static readonly Type ObjectType = typeof(object);

        public Func<object?> CreateCreator(Type type)
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

            return method.CreateDelegate<Func<object>>();
        }

        public Func<object, T?> CreatePropertyGetter<T>(PropertyInfo property)
        {
            var declaringType = property.DeclaringType;

            var method = new DynamicMethod(
                property.Name + "Getter",
                property.PropertyType,
                new[] {ObjectType},
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (declaringType.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, declaringType);
                generator.Emit(OpCodes.Call, property.GetMethod);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, declaringType);
                generator.Emit(OpCodes.Callvirt, property.GetMethod);
            }

            generator.Emit(OpCodes.Ret);

            return method.CreateDelegate<Func<object, T>>();
        }

        public Action<object, T?> CreatePropertySetter<T>(PropertyInfo property)
        {
            var declaringType = property.DeclaringType;

            var method = new DynamicMethod(
                property.Name + "Setter",
                typeof(void),
                new[] {ObjectType, property.PropertyType},
                typeof(EmitMemberAccessor).Module,
                true);

            var generator = method.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            if (declaringType.IsValueType)
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
                generator.Emit(OpCodes.Call, property.SetMethod);
            }
            else
            {
                generator.Emit(OpCodes.Callvirt, property.SetMethod);
            }

            generator.Emit(OpCodes.Ret);

            return method.CreateDelegate<Action<object, T?>>();
        }
    }
#endif
}
