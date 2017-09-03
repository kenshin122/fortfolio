using UnityEditor;

namespace Zenject
{
    [CustomEditor(typeof(GameObjectContext))]
    public class GameObjectContextEditor : ContextEditor
    {
        SerializedProperty _kernel;
        public override void OnEnable()
        {
            base.OnEnable();
            _kernel = serializedObject.FindProperty("_kernel");
//			_dontInject = serializedObject.FindProperty("_dontInject");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_kernel);
        }
    }
}