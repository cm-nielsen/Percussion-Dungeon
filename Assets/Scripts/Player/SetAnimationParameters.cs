using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimationParameters : MonoBehaviour
{
    public ParameterFromControlKey[] paramaters;
    public bool playerKey = true;

    private Animator anim;
    private ControlKey controlKey;

    // Start is called before the first frame update
    void Start()
    {
        //ControlSteadyProxy
        if (playerKey)
            controlKey = GameObject.FindGameObjectWithTag
                ("pControl").GetComponent<ControlKey>();
        else
            controlKey = GetComponent<ControlKey>();
        anim = GetComponent<Animator>();
        foreach (ParameterFromControlKey p in paramaters)
            p.Setup(controlKey, anim);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ParameterFromControlKey p in paramaters)
            p.Update();
    }

    [System.Serializable]
    public class ParameterFromControlKey
    {
        public enum ParameterType { boolean, trigger, steadyBooleanProxy,
            toggledTriggerProxy}

        public ParameterType type;
        public string animationParameter, inputName;

        private ControlSteadyProxy steadyProxy;
        private ControlToggleProxy toggleProxy;
        private Animator anim;
        private ControlKey input;

        public void Setup(ControlKey controlKey, Animator animator)
        {
            input = controlKey;
            anim = animator;
            switch (type)
            {
                case ParameterType.steadyBooleanProxy:
                    steadyProxy = new ControlSteadyProxy();
                    steadyProxy.Setup(input, inputName);
                    break;
                case ParameterType.toggledTriggerProxy:
                    toggleProxy = new ControlToggleProxy();
                    toggleProxy.Setup(input, inputName);
                    break;
            }
        }

        public void Update()
        {
            switch (type)
            {
                case ParameterType.boolean:
                    anim.SetBool(animationParameter, input[inputName]);
                    break;
                case ParameterType.trigger:
                    if (input[inputName])
                        anim.SetTrigger(animationParameter);
                    break;
                case ParameterType.steadyBooleanProxy:
                    anim.SetBool(animationParameter, steadyProxy.val);
                    break;
                case ParameterType.toggledTriggerProxy:
                    toggleProxy.Update();
                    if (toggleProxy.val)
                        anim.SetTrigger(animationParameter);
                    break;
            }
        }
    }

    public void SetAnimatorBool(string name) { anim.SetBool(name, true); }
    public void ResetAnimatorBool(string name) { anim.SetBool(name, false); }
    public void SetAnimatorTrigger(string name) { anim.SetTrigger(name); }
    public void ResetAnimatorTrigger(string name) { anim.ResetTrigger(name); }
}
