using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SinkiiLib.UI
{
    public class TabbedMenuController
    {
        private const string tabClassName = "tab";
        private const string currentlySelectedTabClassName = "currentlySelectedTab";
        private const string unselectedContentClassName = "unselectedContent";
        // Tab and tab content have the same prefix but different suffix
        // Define the suffix of the tab name
        private const string tabNameSuffix = "Tab";
        // Define the suffix of the tab content name
        private const string contentNameSuffix = "Content";


        public static event Action TabSelected;

        private readonly VisualElement root;

        public TabbedMenuController(VisualElement root)
        {
            this.root = root;
        }

        public void RegisterTabCallbacks()
        {
            UQueryBuilder<VisualElement> tabs = GetAllTabs();
            tabs.ForEach((t) => {
                t.RegisterCallback<ClickEvent>(TabOnClick);
            });
        }

        /* Method for the tab on-click event: 

           - If it is not selected, find other tabs that are selected, unselect them 
           - Then select the tab that was clicked on
        */
        private void TabOnClick(ClickEvent evt)
        {
            VisualElement clickedTab = evt.currentTarget as VisualElement;
            if (!TabIsCurrentlySelected(clickedTab))
            {
                GetAllTabs().Where(
                    (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
                ).ForEach(UnselectTab);
                SelectTab(clickedTab);
            }
        }
        //Method that returns a Boolean indicating whether a tab is currently selected
        private static bool TabIsCurrentlySelected(VisualElement tab)
        {
            return tab.ClassListContains(currentlySelectedTabClassName);
        }

        private UQueryBuilder<VisualElement> GetAllTabs()
        {
            return root.Query<VisualElement>(className: tabClassName);
        }

        /* Method for the selected tab: 
           -  Takes a tab as a parameter and adds the currentlySelectedTab class
           -  Then finds the tab content and removes the unselectedContent class */
        private void SelectTab(VisualElement tab)
        {
            tab.AddToClassList(currentlySelectedTabClassName);
            VisualElement content = FindContent(tab);
            content.RemoveFromClassList(unselectedContentClassName);
            TabSelected?.Invoke();
        }

        /* Method for the unselected tab: 
           -  Takes a tab as a parameter and removes the currentlySelectedTab class
           -  Then finds the tab content and adds the unselectedContent class */
        private void UnselectTab(VisualElement tab)
        {
            tab.RemoveFromClassList(currentlySelectedTabClassName);
            VisualElement content = FindContent(tab);
            content.AddToClassList(unselectedContentClassName);
            
        }

        // Method to generate the associated tab content name by for the given tab name
        private static string GenerateContentName(VisualElement tab) =>
            tab.name.Replace(tabNameSuffix, contentNameSuffix);

        // Method that takes a tab as a parameter and returns the associated content element
        private VisualElement FindContent(VisualElement tab)
        {
            return root.Q(GenerateContentName(tab));
        }
    }
}
