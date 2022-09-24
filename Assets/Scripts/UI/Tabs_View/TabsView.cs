using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace UI.Tabs_View
{
    public class TabsView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TabsView, UxmlTraits>
        {
        }


        private const string StyleName = "tabs-view";
        private const string UssClassName = "tabs-view";
        private const string ContentContainerClassName = "tabs-view__content";
        private const string NotTabClassName = "not-tab";

        private readonly VisualElement _tabButtonsContainer;
        private readonly VisualElement _tabsContainer;

        private TabImageButton _activeTabImageButton;

        public override VisualElement contentContainer => _tabsContainer;
        public VisualElement Content => _activeTabImageButton.Target;

        public VisualElement[] Tabs => contentContainer.Children()
            .Where(element => !element.ClassListContains(NotTabClassName))
            .ToArray();

        public bool IsTabContentVisible
        {
            get => _tabsContainer.style.display == DisplayStyle.Flex;
            set
            {
                if (value)
                {
                    style.flexGrow = _tabsContainer.style.flexGrow = 1;
                    _tabsContainer.style.display = DisplayStyle.Flex;
                }
                else
                {
                    style.flexGrow = _tabsContainer.style.flexGrow = 0;
                    _tabsContainer.style.display = DisplayStyle.None;
                }
            }
        }

        public event Action<bool> TabContentVisibilityChanged;


        public TabsView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{StyleName}"));

            AddToClassList(UssClassName);

            // style.flexDirection = FlexDirection.Row;

            _tabButtonsContainer = new VisualElement
            {
                name = "TabButtons",
                style =
                {
                    flexDirection = FlexDirection.Column,
                },
            };
            // _tabContent.AddToClassList(TabsContainerClassName);
            hierarchy.Add(_tabButtonsContainer);

            _tabsContainer = new VisualElement
            {
                name = "ContentContainer",
                style =
                {
                    flexGrow = 1f,
                },
            };
            _tabsContainer.AddToClassList(ContentContainerClassName);
            hierarchy.Add(_tabsContainer);

            RegisterCallback<AttachToPanelEvent>(ProcessEvent);
        }

        private void ProcessEvent(AttachToPanelEvent e)
        {
            // This code takes any existing tab buttons and hooks them into the system...
            MoveTabButtons();

            // Finally, if we need to, activate this tab...
            if (_activeTabImageButton != null)
            {
                SelectTab(_activeTabImageButton);
            }
            else if (_tabButtonsContainer.childCount > 0)
            {
                _activeTabImageButton = (TabImageButton) _tabButtonsContainer[0];

                SelectTab(_activeTabImageButton);
            }
        }

        private void MoveTabButtons()
        {
            for (var i = 0; i < _tabsContainer.childCount; ++i)
            {
                var element = _tabsContainer[i];

                if (element is TabImageButton button)
                {
                    _tabsContainer.Remove(element);
                    AddTabButton(button);
                    --i;
                }
                else if (!element.ClassListContains(NotTabClassName))
                {
                    element.style.display = DisplayStyle.None;
                }
            }
        }

        private void AddTabButton(TabImageButton tabButton)
        {
            _tabButtonsContainer.Add(tabButton);

            if (tabButton.Target == null && tabButton.TargetId.IsNotNullOrEmpty())
            {
                var targetId = tabButton.TargetId;
                tabButton.Target = this.Q(targetId);
            }

            tabButton.Selected += Activate;
        }

        private void SelectTab(TabImageButton tabImageButton)
        {
            var target = tabImageButton.Target;

            tabImageButton.Select();
            if (target != null)
            {
                target.style.display = DisplayStyle.Flex;

                if (!IsTabContentVisible)
                {
                    IsTabContentVisible = true;
                    TabContentVisibilityChanged?.Invoke(true);
                }
            }
        }

        private void Activate(TabImageButton tabImageButton)
        {
            if (_activeTabImageButton != null)
            {
                DeselectTab(_activeTabImageButton);
            }

            _activeTabImageButton = tabImageButton;
            SelectTab(_activeTabImageButton);
        }

        private static void DeselectTab(TabImageButton tabImageButton)
        {
            var target = tabImageButton.Target;

            if (target != null)
            {
                target.style.display = DisplayStyle.None;
            }
            tabImageButton.Deselect();
        }
    }
}
