using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base.Helper;
using Base.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    public class DebugTool : BaseMono
    {
        [SerializeField] private FloatingButton floatingButton;
        [SerializeField] private GameObject debugContent;
        [SerializeField] private MonoBehaviour functionObj;

        [SerializeField] private TMP_Dropdown functionDropdown;

        private bool _init = false;
        private int _functionIndex = 0;

        private List<GameDebugProperty> _debugProperties = new List<GameDebugProperty>();
        private List<GameDebugSceneMethod> _debugSceneMths = new List<GameDebugSceneMethod>();
        private Dictionary<SceneName, List<GameDebugSceneMethod>> _sceneMethods = new Dictionary<SceneName, List<GameDebugSceneMethod>>();

        protected override void Start()
        {
            base.Start();

            floatingButton.OnClick += OpenDebugUI;
        }

        private void OnDestroy()
        {
            floatingButton.OnClick -= OpenDebugUI;
        }

        private void OpenDebugUI()
        {
            if (!_init) Init(functionObj);
            
            FilterByScene();
            
            debugContent.SetActive(true);
            floatingButton.Active = false;
        }

        private void Init(MonoBehaviour mono)
        {
            _sceneMethods.Clear();
            _debugProperties.Clear();

            {
                // Method
                List<MethodInfo> mths = mono.GetType().GetMethods()
                    .Where(e => e.GetCustomAttributes(typeof(DebugActionAttribute), false).Length > 0).ToList();

                int count = mths.Count;
                for (int i = 0; i < count; ++i)
                {
                    MethodInfo methodInfo = mths[i];
                    object[] attributes = methodInfo.GetCustomAttributes(false);
                    DebugActionAttribute debugActionAttribute = null;

                    foreach (var attr in attributes)
                    {
                        debugActionAttribute = attr as DebugActionAttribute;
                        if (debugActionAttribute != null) break;
                    }
                    
                    if (debugActionAttribute == null || !debugActionAttribute.IsEnable) { continue; }

                    if (!_sceneMethods.TryGetValue(debugActionAttribute.ActionScene, out List<GameDebugSceneMethod> methods))
                    {
                        methods = new List<GameDebugSceneMethod>();
                        _sceneMethods[debugActionAttribute.ActionScene] = methods;
                    }
                    
                    methods.Add(new GameDebugSceneMethod
                    {
                        SceneName = debugActionAttribute.ActionScene,
                        Attribute = debugActionAttribute,
                        Method = methodInfo
                    });
                }
            }

            {
                // Properties
                List<PropertyInfo> infos = mono.GetType().GetProperties()
                    .Where(e => e.GetCustomAttributes(typeof(DebugInfoAttribute), false).Length > 0).ToList();
                for (int i = 0; i < infos.Count; ++i)
                {
                    PropertyInfo propertyInfo = infos[i];
                    object[] attrs = propertyInfo.GetCustomAttributes(false);
                    DebugInfoAttribute debugInfoAttribute = null;
                    foreach (var attr in attrs)
                    {
                        debugInfoAttribute = attr as DebugInfoAttribute;
                        if (debugInfoAttribute != null) break;
                    }

                    if (debugInfoAttribute == null) { continue; }

                    GameDebugProperty debugProperty = new GameDebugProperty
                    {
                        Attribute = debugInfoAttribute,
                        Property = propertyInfo
                    };
                    
                    _debugProperties.Add(debugProperty);
                }
                
                _debugProperties.Sort((a,b) => a.Attribute.Order - b.Attribute.Order);
            }

            _init = true;
        }

        private void FilterByScene()
        {
            _debugSceneMths.Clear();
            string sceneName = GetOpenScene();

            Enum.TryParse(sceneName, false, out SceneName scene);

            foreach (var pair in _sceneMethods)
            {
                if (pair.Key == scene || pair.Key == SceneName.AnyScene)
                {
                    _debugSceneMths.AddRange(pair.Value);
                }
            }
            
            _debugSceneMths.Sort((a, b) =>
            {
                int compareTo = string.Compare(a.Attribute.ActionGroup, b.Attribute.ActionGroup, StringComparison.Ordinal);
                if (compareTo != 0) { return compareTo; }

                compareTo = a.Attribute.Priority - b.Attribute.Priority;
                if (compareTo != 0) { return compareTo; }

                compareTo = string.Compare(a.Attribute.ActionName, b.Attribute.ActionName, StringComparison.Ordinal);
                return compareTo;
            });
            
            functionDropdown.ClearOptions();
            List<string> cheatNames = new List<string>();
            foreach (var sceneMethod in _debugSceneMths)
            {
                cheatNames.Add($"<b>[{sceneMethod.Attribute.ActionGroup}]</b> {sceneMethod.Attribute.ActionName}");
            }
            functionDropdown.AddOptions(cheatNames);
            functionDropdown.value = _functionIndex;
        }

        private string GetOpenScene()
        {
            string sceneName = String.Empty;
            for (int i = 1; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene.isLoaded) sceneName = scene.name;
            }

            return sceneName;
        }
    }

    public enum SceneName
    {
        AnyScene = 0,
        ManagerScene = 1,
    }
    
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DebugActionAttribute : Attribute
    {
        public readonly SceneName ActionScene;
        public readonly string    ActionGroup;
        public readonly string    ActionName;
        public readonly int       Priority;
        public readonly bool      IsEnable = true;
        
        public DebugActionAttribute(string actionName, SceneName scene, int priority = 0, bool isEnable = true)
        {
            ActionName  = actionName;
            ActionGroup = "Misc";
            Priority    = priority;
            IsEnable    = isEnable;
            ActionScene = scene;
        }

        public DebugActionAttribute(string actionName, string actionGroup, SceneName scene)
        {
            ActionName  = actionName;
            ActionGroup = actionGroup;
            ActionScene = scene;
        }

        public DebugActionAttribute(string actionName, string actionGroup, SceneName scene, int priority = 0, bool isEnable = true)
        {
            ActionName  = actionName;
            ActionGroup = actionGroup;
            Priority    = priority;
            IsEnable    = isEnable;
            ActionScene = scene;
        }
    }
    
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SelectAttribute : Attribute
    {
        private int _selectIndex;

        public int SelectIndex
        {
            get => _selectIndex;
            set => _selectIndex = value;
        }
        
        public readonly string MethodName;

        public SelectAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class DebugInfoAttribute : Attribute
    {
        public string Name  = null;
        public string Color = null;
        public int    Order = 0;

        public DebugInfoAttribute(string name, string color, int order = 0)
        {
            Name  = name;
            Color = color;
            Order = order;
        }
        public DebugInfoAttribute(string name, int order = 0)
        {
            Name  = name;
            Color = null;
            Order = order;
        }
    }
    
    public class GameDebugSceneMethod
    {
        public SceneName            SceneName;
        public MethodInfo           Method;
        public DebugActionAttribute Attribute;
        public int[]                ParamIndex;
    }
    public class GameDebugProperty
    {
        public PropertyInfo       Property;
        public DebugInfoAttribute Attribute;
    }
}

