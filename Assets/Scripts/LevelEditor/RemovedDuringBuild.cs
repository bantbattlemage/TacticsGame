using UnityEngine;

// entire gameObject will be destroyed while the game is being compiled.
// This means any references to it will be null.
public class RemovedDuringBuild : MonoBehaviour
{
#if UNITY_EDITOR
	private void Reset() { Ensure_EditorTag(); }
	private void OnEnable() { Ensure_EditorTag(); }
	private void Awake() { Ensure_EditorTag(); }


	void Ensure_EditorTag()
	{
		if (gameObject.tag != "EditorOnly")
		{
			UnityEditor.EditorUtility.SetDirty(gameObject);//applies changes, preventing unity-deserializing old data when we exit gameplay mode, etc
		}
		//a special tag, that will ensure that an object won't be included in build
		gameObject.tag = "EditorOnly";
	}
#endif
}