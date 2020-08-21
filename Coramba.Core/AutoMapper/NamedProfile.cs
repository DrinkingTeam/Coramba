using AutoMapper;

namespace Coramba.Core.AutoMapper
{
    public class NamedProfile: Profile
    {
        public NamedProfile(string name)
            : base(name)
        {
        }
    }
}
