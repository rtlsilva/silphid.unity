﻿namespace Silphid.Showzup.Injection
{
    internal class NullBinding : IBinding
    {
        public IContainer Container => Injection.Container.Null;
        public IBinding InList() => Binding.Null;
        public IBinding AsSingle() => Binding.Null;
        public IBinding With(IResolver resolver) => Binding.Null;
    }
}