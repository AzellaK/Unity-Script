using UnityEngine;

public class ActivateTrigger : MonoBehaviour {
	public enum Mode {
        /// <summary>
        /// ֻ�轫�����㲥��Ŀ��
        /// </summary>
		Trigger = 0, // Just broadcast the action on to the target
        /// <summary>
        /// ��Դ�滻Ŀ��
        /// </summary>
		Replace = 1, // replace target with source
        /// <summary>
        /// ����Ŀ��GameObject
        /// </summary>
		Activate = 2, // Activate the target GameObject
        /// <summary>
        /// �������
        /// </summary>
		Enable = 3, // Enable a component
        /// <summary>
        /// ��Ŀ���Ͽ�ʼ����
        /// </summary>
		Animate = 4, // Start animation on target
        /// <summary>
        /// ͣ��Ŀ��GameObject
        /// </summary>
		Deactivate = 5 // Decativate target GameObject
	}

    ///<summary>
    /// The action to accomplish
    ///Ҫ��ɵ��ж�
    ///</summary>
    public Mode action = Mode.Activate;

    /// <summary>
    /// The game object to affect. If none, the trigger work on this game object
    /// Ӱ����Ϸ���� ���û�У��򴥷����Դ���Ϸ����������
    /// </summary>
    public Object target;
	public GameObject source;
	public int triggerCount = 1;///
	public bool repeatTrigger = false;
	
	void DoActivateTrigger () {
		triggerCount--;

		if (triggerCount == 0 || repeatTrigger) {
			Object currentTarget = target != null ? target : gameObject;
			Behaviour targetBehaviour = currentTarget as Behaviour;
			GameObject targetGameObject = currentTarget as GameObject;
			if (targetBehaviour != null)
				targetGameObject = targetBehaviour.gameObject;
		
			switch (action) {
				case Mode.Trigger:
					targetGameObject.BroadcastMessage ("DoActivateTrigger");
					break;
				case Mode.Replace:
					if (source != null) {
						Object.Instantiate (source, targetGameObject.transform.position, targetGameObject.transform.rotation);
						DestroyObject (targetGameObject);
					}
					break;
				case Mode.Activate:
					targetGameObject.SetActive(true);
					break;
				case Mode.Enable:
					if (targetBehaviour != null)
						targetBehaviour.enabled = true;
					break;	
				case Mode.Animate:
					targetGameObject.GetComponent<Animation>().Play ();
					break;	
				case Mode.Deactivate:
					targetGameObject.SetActive(false);
					break;
			}
		}
	}

	void OnTriggerEnter (Collider other) {
		DoActivateTrigger ();
	}
}