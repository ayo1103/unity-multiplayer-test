using UnityEngine;
using UnityEngine.UI;

public class UIVersion : MonoBehaviour
{
    [SerializeField] private Text _versionText;

    private void Awake()
    {
        _versionText.text = Application.version;
    }
}
