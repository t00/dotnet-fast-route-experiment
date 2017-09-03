using System;
using System.Collections.Generic;
using System.Linq;

namespace FastRouter
{
    public class ActionDictionaryRouter: BaseRouter
    {
        public ActionDictionary InitAuto()
        {
            var actionMinLength = ValidActions.Select(a => a.route.Length).Min();
            var actions = new ActionDictionary(actionMinLength);
            AddActions(actions, ValidActions, 0);
            return actions;
        }

        private void AddActions(ActionDictionary actions, ICollection<(string route, Action<ActionStatus> action)> add, int index)
        {
            var groups = add.Where(g => index < g.route.Length).GroupBy(g => g.route[index]).ToList();
            index++;
            if (groups.Count == 1 && groups.First().Count() == add.Count)
            {
                var group = groups.First();
                actions.Suffix = (actions.Suffix ?? string.Empty) + group.Key;
                foreach (var g in group)
                {
                    if (index == g.route.Length)
                    {
                        actions.Action = g.action;
                    }
                }
                AddActions(actions, add, index);
            }
            else
            {
                foreach (var group in groups)
                {
                    if (group.Count() == 1)
                    {
                        var r = group.First();
                        var s = r.route.Substring(index);
                        var action = new ActionDictionary(r.route.Length, string.IsNullOrEmpty(s) ? null : s, r.action);
                        actions.Add(group.Key, action);
                    }
                    else
                    {
                        var minLength = group.Select(g => g.route.Length).Min();
                        var action = new ActionDictionary(minLength);
                        actions.Add(group.Key, action);
                        AddActions(action, group.ToList(), index);
                    }
                }
            }
        }

        public override void Init()
        {
            actions = InitAuto();
        }

        private void InitTest()
        {
            var actions1 = new ActionDictionary(4)
            {
                { 'a', new ActionDictionary(10, "nimations", AnimationsAction) },
                { 'c', new ActionDictionary(10, "ategor")
                    {
                        { 'i', new ActionDictionary(10, "es", CategoriesAction) },
                        { 'y', new ActionDictionary(12, "tree", CategoryTreeAction) }
                    }
                },
                { 'g', new ActionDictionary(5, "roup", GroupAction)
                    {
                        { 't', new ActionDictionary(9, "ree", GroupTreeAction) }
                    }
                },
                { 'l', new ActionDictionary(4)
                    {
                        {
                            'i', new ActionDictionary(4)
                            {
                                { 'm', new ActionDictionary(5, "it", LimitAction) },
                                { 's', new ActionDictionary(4, "t", ListAction) }
                            }
                        },
                        { 'o', new ActionDictionary(5, "gin", LoginAction) }
                    }
                },
                { 'v', new ActionDictionary(7, "ersion", VersionAction)
                    {
                        { 'i', new ActionDictionary(11, "nfo", VersionInfoAction) }
                    }
                },
                { 'x', new ActionDictionary(12, "ml")
                    {
                        { 'p', new ActionDictionary(13, "roperties", XmlPropertiesAction) },
                        { 's', new ActionDictionary(12, "tructure", XmlStructureAction) }
                    }
                }
            };

            var actions2 = InitAuto();

            Compare(actions1, actions2);
        }

        private void Compare(ActionDictionary actions1, ActionDictionary actions2)
        {
            if (actions1.Action != actions2.Action || actions1.MinLength != actions2.MinLength || actions1.Suffix != actions2.Suffix || actions1.SuffixLength != actions2.SuffixLength)
            {
                System.Diagnostics.Trace.WriteLine("Differ prop");
            }
            if (actions1.Count != actions2.Count)
            {
                System.Diagnostics.Trace.WriteLine("Differ count");
            }
            foreach (var action in actions1.Zip(actions2, (a, b) => new { A = a, B = b }))
            {
                if (action.A.Key != action.B.Key)
                {
                    System.Diagnostics.Trace.WriteLine("Differ key");
                }
                Compare(action.A.Value, action.B.Value);
            }
        }

        public override void Route(string text)
        {
            var status = new ActionStatus(text);
            if (status.Length >= actions.MinLength && TestAction(status, actions))
            {
                return;
            }
            NotFoundAction(status);
        }

        private bool TestAction(ActionStatus status, ActionDictionary dict)
        {
            var c = status.Text[status.Index];
            if (dict.TryGetValue(c, out var result))
            {
                status.Index++;
                if (status.Length >= result.MinLength)
                {
                    if (result.SuffixLength == 0 || status.Text.IndexOf(result.Suffix, status.Index, result.SuffixLength, StringComparison.Ordinal) >= 0)
                    {
                        status.Index += result.SuffixLength;
                        if (status.Index < status.Length && result.Count > 0 && TestAction(status, result))
                        {
                            return true;
                        }
                        if (result.Action != null)
                        {
                            result.Action(status);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private ActionDictionary actions;
    }
}
