using UnityEngine;

namespace JakePerry.Unity
{
    [CreateAssetMenu(fileName = "UniqueId", menuName = "JakePerry/UniqueId")]
    public sealed class UniqueId : ScriptableObject
    {
        [SerializeField]
        private string m_id;

        public string Id => m_id ?? string.Empty;

        public static implicit operator string(UniqueId obj)
        {
#pragma warning disable UNT0008 // Null propagation on Unity objects
            return obj?.Id ?? string.Empty;
#pragma warning restore UNT0008
        }
    }
}
