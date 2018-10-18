using System;
using UnityEngine;

namespace ModApi.Attachable
{
    public class Rigid : MonoBehaviour
    {
        public Part part
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs every frame.
        /// </summary>
        public virtual void Update()
        {
            // Written, 02.10.2018

            try
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 2, 1 << this.gameObject.layer) && this.part.isPartCollider(hitInfo.collider))
                    {
                        this.part.disassemble();
                    }
                }
            }
            catch (Exception ex)
            {
                MSCLoader.ModConsole.Error("[Rigid.Update] - " + ex.ToString());
            }
        }
    }
}
