using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastRouter
{
    public class EmitRouter : BaseRouter
    {
        public override void Init()
        {
            var ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
            var mod = ab.DefineDynamicModule("HashDictionaryRouter");
            var mb = mod.DefineGlobalMethod("Route", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Any, typeof(Action<ActionStatus>), new[] { typeof(string) });
            var gen = mb.GetILGenerator();
            throw new NotImplementedException();
        }

        public override void Route(string text)
        {
            
        }
    }
}
