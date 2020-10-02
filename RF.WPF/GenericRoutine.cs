using System;
using System.Threading.Tasks;
using RFReborn.Routines;

namespace RF.WPF
{
    public class GenericRoutine : RoutineBase
    {
        private readonly Func<Task> _func;

        public GenericRoutine(TimeSpan cooldownTime, Func<Task> func) : base(cooldownTime) => _func = func;

        protected override async Task OnTick() => await _func?.Invoke();

        protected override Task Shutdown() => throw new NotImplementedException();
    }
}
