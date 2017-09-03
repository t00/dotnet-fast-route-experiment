using System;
using System.Collections.Generic;

namespace FastRouter
{
    public class HashDictionaryRouter: BaseRouter
    {
        public override void Init()
        {
            foreach (var testAction in ValidActions)
            {
                actions.Add(testAction.route, testAction.action);
            }
        }

        public override void Route(string text)
        {
            var status = new ActionStatus(text);
            if (actions.TryGetValue(text, out var result))
            {
                status.Index = text.Length;
                result(status);
            }
            NotFoundAction(status);
        }

        private readonly IDictionary<string, Action<ActionStatus>> actions = new Dictionary<string, Action<ActionStatus>>();
    }
}
