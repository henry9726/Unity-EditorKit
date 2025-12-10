using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit
{
    using Henry.EditorKit.Component;
    using Henry.EditorKit.BuiltInComponent;

    class Main : EditorWindow, IHasCustomMenu
    {
        struct PanelData
        {
            public string Name { get; private set; }
            public ISubPanel Panel { get; private set; }

            public PanelData(string name, ISubPanel panel)
            {
                Name = name;
                Panel = panel;
            }
        }

        const string MenuOpenEditorKit = MenuPath.EditorKitMenuPath + "/Open EditorKit";
        const string MenuHidePanelBar = MenuPath.EditorKitMenuPath + "/Hide panel bar";

        [SerializeField] PinnedPanel pinnedPanel;
        [SerializeField] SearchPanel searchPanel;

        [SerializeField] RecordStore compRecordStore;
        [SerializeField] InstanceStore compStore;

        bool isInitialized = false;
        PanelData[] panelDataArr;
        string[] panelNameCacheArr;
        int activePanelIdx = 0;

        static bool IsHidePanelBar
        {
            get => EditorPrefs.GetBool("Henry_EditorKit_NavigationBar", false);
            set => EditorPrefs.SetBool("Henry_EditorKit_NavigationBar", value);
        }

        [MenuItem(MenuOpenEditorKit + " %&k", false, Priority.MainPanel)]
        static void OpenWindow()
        {
            var window = GetWindow<Main>(false, MenuPath.EditorKitDisplayName);
        }

        [MenuItem(MenuHidePanelBar)]
        static void HidePanelBar()
        {
            IsHidePanelBar = !IsHidePanelBar;
        }
        [MenuItem(MenuHidePanelBar, true)]
        static bool HidePanelBarValidate()
        {
            Menu.SetChecked(MenuHidePanelBar, IsHidePanelBar);
            return true;
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            panelDataArr[activePanelIdx].Panel.AddItemsToMenu(menu);
            menu.AddItem(new GUIContent("Hide panel bar"), IsHidePanelBar, HidePanelBar);
        }

        void Initialize()
        {
            pinnedPanel = new();
            searchPanel = new();

            compRecordStore = RecordStore.LoadRecord();
            compStore = new();

            var infoDict = ComponentRegistry.InfoDict;
            var records = compRecordStore.Records;
            compStore.SetData(InstanceStore.InstanceFromRecord(records, infoDict));
            pinnedPanel.SetPinnedComps(compStore.Components);
        }

        void OnEnable()
        {
            EditorApplication.update += RepaintOnUpdate;

            if (isInitialized is false)
            {
                Initialize();
                isInitialized = true;
            }

            {
                // 初始化不能序列化的欄位
                panelDataArr = new PanelData[] {
                    new("Pinned", pinnedPanel),
                    new("Browse", searchPanel)
                };
                panelNameCacheArr = panelDataArr.Select(el => el.Name).ToArray();

                // 更新個[元件資料]的Info資訊
                foreach (var item in compStore.Components)
                {
                    item.SetInfo(ComponentRegistry.InfoDict[item.Record.compTypeFullName]);
                }

                pinnedPanel.Setup(PinPresetComps);

                // 擷取[崁入式]的元件給[PinnedPanel]
                FilterPinnedComponentForPinnedPanel();

                // 
                RegisterPinnedPanelEvents();
                RegisterSearchPanelEvents();
            }

            foreach (var item in panelDataArr)
            {
                item.Panel.OnEnable();
            }
        }

        void OnDisable()
        {
            EditorApplication.update -= RepaintOnUpdate;

            foreach (var item in panelDataArr)
            {
                item.Panel.OnDisable();
            }

            if (compRecordStore != null)
            {
                compRecordStore.SetRecords(compStore.Components.Select(el => el.Record));
                compRecordStore.SaveRecord();
                DestroyImmediate(compRecordStore);
            }
        }

        void RepaintOnUpdate()
        {
            this.Repaint();
        }

        void OnGUI()
        {
            if (StyleSheet.Instance.IsSetup == false)
            {
                StyleSheet.Instance.Setup();
            }

            using (new EditorGUILayout.VerticalScope())
            {
                if (IsHidePanelBar is false)
                {
                    GUILayout.Space(2);
                    activePanelIdx = GUILayout.Toolbar(activePanelIdx, panelNameCacheArr);
                }
                panelDataArr[activePanelIdx].Panel.OnGUI(position);
            }
        }

        void PinPresetComps()
        {
            var infoDict = ComponentRegistry.InfoDict;

            var infos = new Info[]
            {
                    infoDict[typeof(CodeEditorTool).FullName],
                    infoDict[typeof(SpritePackerSwitchTool).FullName],
                    infoDict[typeof(TimeScaleSwitchTool).FullName],
            };

            var comps = InstanceStore.InstanceFromInfo(infos);

            foreach (var el in comps)
            {
                el.Component.OnEnable();
            }

            compStore.SetData(comps);
            pinnedPanel.SetPinnedComps(comps);
        }

        void RegisterPinnedPanelEvents()
        {
            pinnedPanel.OnRequestUnpinComp += OnRequestUnpinComp;
            pinnedPanel.OnRequestPopupComp += OnRequestPopupComp;
            pinnedPanel.OnRequestUnpinAllComp += OnRequestUnpinAllComp;

            void OnRequestUnpinComp(Data data)
            {
                compStore.RemoveData(data);
                FilterPinnedComponentForPinnedPanel();
            }

            void OnRequestPopupComp(Data data)
            {
                compStore.RemoveData(data);

                compStore.AddData(data);

                ComponentWindow.Create(data);
                FilterPinnedComponentForPinnedPanel();
            }

            void OnRequestUnpinAllComp()
            {
                compStore.RemoveAllData();
                FilterPinnedComponentForPinnedPanel();
            }
        }

        void RegisterSearchPanelEvents()
        {
            searchPanel.OnRequestOpenComp += OnRequestOpenComp;
            searchPanel.OnRequestPinComp += OnRequestPinComp;

            void OnRequestOpenComp(Info info)
            {
                var instance = InstanceStore.CreateComponent(info.TypeFullName);
                var data = new Data(instance, info, new(info.TypeFullName));

                data.Component.OnInstance();
                data.Component.OnEnable();
                ComponentWindow.Create(data);
            }

            void OnRequestPinComp(Info info)
            {
                var instance = InstanceStore.CreateComponent(info.TypeFullName);
                var data = new Data(instance, info, new(info.TypeFullName));

                data.Component.OnInstance();
                data.Component.OnEnable();
                compStore.AddData(data);
                FilterPinnedComponentForPinnedPanel();

                ShowNotification(new GUIContent("Component pinned"), 0.5d);
            }
        }

        void FilterPinnedComponentForPinnedPanel()
        {
            var pinnedComps = new List<Data>();
            foreach (var item in compStore.Components)
            {
                pinnedComps.Add(item);
            }
            pinnedPanel.SetPinnedComps(pinnedComps);
        }
    }
}