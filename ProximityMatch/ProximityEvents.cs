using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProximityMatch
{
    public class ProximityMatchEventArgs : EventArgs
    {
        public readonly long uniqueId;
        public ProximityMatchEventArgs(long _uniqueId)
        {
            uniqueId = _uniqueId;
        }
    }

    public class RemoveEventArgs : ProximityMatchEventArgs
    {
        public bool? status { get; protected set; }
        public RemoveEventArgs(long _uniqueId, bool? _status = null)
            : base(_uniqueId)
        {
            status = _status;
        }
    }
    public class PlotEventArgs : ProximityMatchEventArgs
    {
        public bool? status { get; protected set; }
        public readonly VectorNode vnode;
        public PlotEventArgs(VectorNode _vnode, bool? _status = null)
            : base(_vnode._vector.uniqueId)
        {
            vnode = _vnode;
            status = _status;
        }
    }

    public class UpdateEventArgs : ProximityMatchEventArgs
    {
        public bool? status { get; protected set; }
        public readonly VectorNode vnode;
        public UpdateEventArgs(VectorNode _vnode, bool? _status = null)
            : base(_vnode._vector.uniqueId)
        {
            vnode = _vnode;
            status = _status;
        }
    }
}
