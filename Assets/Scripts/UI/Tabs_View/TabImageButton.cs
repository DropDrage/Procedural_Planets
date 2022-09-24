using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Tabs_View
{
    public class TabImageButton : Button
    {
        public new class UxmlFactory : UxmlFactory<TabImageButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _targetId = new() {name = "target-id"};


            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(visualElement, bag, cc);
                var item = (TabImageButton) visualElement;

                item.TargetId = _targetId.GetValueFromBag(bag, cc);
            }
        }


        private const string StyleName = "tabs-view";
        private const string UxmlName = "TabImageButton";
        private const string UssClassName = "tab-image-button";
        private const string UssActiveClassName = UssClassName + "--active";

        private VisualElement _image;

        public string TargetId { get; private set; }
        public VisualElement Target { get; set; }

        public event Action<TabImageButton> Selected;


        public TabImageButton()
        {
            Init();
        }

        public TabImageButton(string text, VisualElement target)
        {
            Init();
            Target = target;
        }

        private void Init()
        {
            styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{StyleName}"));
            AddToClassList(UssClassName);

            var visualTree = Resources.Load<VisualTreeAsset>($"UXML/{UxmlName}");
            visualTree.CloneTree(this);

            clicked += OnClick;

            RegisterCallback<AttachToPanelEvent>(Callback);
        }

        private void Callback(AttachToPanelEvent evt)
        {
            ApplyImage();
        }

        private void ApplyImage() //ToDo icon isn't applied
        {
            _image = this.Q("Image");
            _image.style.backgroundImage = style.backgroundImage;
            style.backgroundImage = null;
        }


        private void OnClick()
        {
            Selected?.Invoke(this);
        }


        public void Select()
        {
            AddToClassList(UssActiveClassName);

            if (Target != null)
            {
                Target.style.display = DisplayStyle.Flex;
                Target.style.flexGrow = 1;
            }
        }

        public void Deselect()
        {
            RemoveFromClassList(UssActiveClassName);
            MarkDirtyRepaint();

            if (Target != null)
            {
                Target.style.display = DisplayStyle.None;
                Target.style.flexGrow = 0;
            }
        }
    }
}
