using UnityEngine;

namespace JakePerry.Unity
{
    [CreateAssetMenu(fileName = "UniqueId", menuName = "JakePerry/UniqueId")]
    public sealed class UniqueId : ScriptableObject
    {
        [SerializeField]
        private string m_id;

        public string Id => m_id;
    }
}
