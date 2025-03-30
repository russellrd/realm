using UnityEngine;
using UnityEngine.SceneManagement;

namespace Realm
{
    public class Back : MonoBehaviour
    {
        public void GoBack()
        {
            SceneManager.LoadScene("RealmInterface");
        }
    }
}
