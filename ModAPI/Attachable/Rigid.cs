using System;
using UnityEngine;

namespace ModApi.Attachable
{
    /// <summary>
    /// Represents the rigid instance of the <see cref="Part"/>. Represents logic that is only relevant to the rigid (installed) instance. 
    /// Handles the part's dissassemble logic in <see cref="Rigid.Update"/>. 
    /// </summary>
    public class Rigid : MonoBehaviour
    {
        /// <summary>
        /// Represents the part of the rigid instance.
        /// </summary>
        public Part part
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs every frame. Overloadable to change disassemble logic.
        /// </summary>
        public virtual void Update()
        {
            // Written, 02.10.2018

            try
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 2, 1 << this.gameObject.layer) && this.part.isPartCollider(hitInfo.collider, PartInstanceTypeEnum.Rigid))
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
