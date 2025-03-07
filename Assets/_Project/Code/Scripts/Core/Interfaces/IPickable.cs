using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBPT.Core.Interface
{
    public interface IPickable 
    {
        public void PickedUp();
        public void Dragged();
        public void Dropped();
    }
}
