using System;
using System.Collections.Generic;
using System.Linq;

namespace FastRouter
{
    public abstract class BaseRouter
    {
        public abstract void Init();

        public abstract void Route(string text);

        public IList<(string route, Action<ActionStatus> action)> ValidActions { get; }

        public IList<(string route, Action<ActionStatus> action)> TestActions { get; }

        protected BaseRouter()
        {
            // order matters, more frequent earlier for better performance, for testing alphabetical
            var actions = new List<(string route, Action<ActionStatus> action)>
            {
                ("animations", AnimationsAction),
                ("categories", CategoriesAction),
                ("categorytree", CategoryTreeAction),
                ("group", GroupAction),
                ("grouptree", GroupTreeAction),
                ("limit", LimitAction),
                ("list", ListAction),
                ("login", LoginAction),
                ("version", VersionAction),
                ("versioninfo", VersionInfoAction),
                ("xmlproperties", XmlPropertiesAction),
                ("xmlstructure", XmlStructureAction),
            };

            //ValidActions = actions;
            ValidActions = Enumerable.Range(1, 2).SelectMany(e => actions.Select(a => (a.route + e.ToString() + a.route, a.action))).ToList();

            TestActions = new List<(string route, Action<ActionStatus> action)>(ValidActions);
            TestActions.Add(("notfound", NotFoundAction));
        }

        public void NotFoundAction(ActionStatus t)
        {
            NotFoundCount++;
        }

        public void AnimationsAction(ActionStatus t)
        {
            AnimationsCount++;
        }

        public void CategoriesAction(ActionStatus t)
        {
            CategoriesCount++;
        }

        public void CategoryTreeAction(ActionStatus t)
        {
            CategoryTreeCount++;
        }

        public void GroupAction(ActionStatus t)
        {
            GroupCount++;
        }

        public void GroupTreeAction(ActionStatus t)
        {
            GroupTreeCount++;
        }

        public void LoginAction(ActionStatus t)
        {
            LoginCount++;
        }

        public void LimitAction(ActionStatus t)
        {
            LimitCount++;
        }

        public void ListAction(ActionStatus t)
        {
            ListCount++;
        }

        public void VersionAction(ActionStatus t)
        {
            VersionCount++;
        }

        public void VersionInfoAction(ActionStatus t)
        {
            VersionInfoCount++;
        }

        public void XmlPropertiesAction(ActionStatus t)
        {
            XmlPropertiesCount++;
        }

        public void XmlStructureAction(ActionStatus t)
        {
            XmlStructureCount++;
        }

        public int NotFoundCount;

        public int AnimationsCount;

        public int CategoriesCount;

        public int CategoryTreeCount;

        public int GroupCount;

        public int GroupTreeCount;

        public int LoginCount;

        public int LimitCount;

        public int ListCount;

        public int VersionCount;

        public int VersionInfoCount;

        public int XmlPropertiesCount;

        public int XmlStructureCount;
    }
}
