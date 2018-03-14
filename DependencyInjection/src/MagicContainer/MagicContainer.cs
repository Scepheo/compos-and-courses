using System;
using System.Collections.Generic;

namespace MagicContainer
{
    public class MagicContainer
    {
        private readonly Dictionary<Type, Type> _typeMappings
            = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, object[]> _argumentConfigurations
            = new Dictionary<Type, object[]>();

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type type)
        {
            if (_typeMappings.TryGetValue(type, out var implementingType))
            {
                return Resolve(implementingType);
            }

            var constructors = type.GetConstructors();

            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Cannot construct type '{type.Name}'");
            }

            var constructor = constructors[0];

            var parameterInfos = constructor.GetParameters();
            object[] parameters;

            if (_argumentConfigurations.TryGetValue(type, out var configuredValues))
            {
                parameters = configuredValues;
            }
            else
            {
                parameters = new object[parameterInfos.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = Resolve(parameterInfos[i].ParameterType);
                }
            }

            return constructor.Invoke(parameters);
        }

        public void Implement<TRequested, TImplementing>()
            where TImplementing : TRequested
            => Implement(typeof(TRequested), typeof(TImplementing));

        public void Implement(Type requested, Type implementing)
        {
            _typeMappings.Add(requested, implementing);
        }

        public void Configure<T>(params object[] arguments)
            => Configure(typeof(T), arguments);

        public void Configure(Type type, params object[] arguments)
        {
            _argumentConfigurations.Add(type, arguments);
        }
    }
}
