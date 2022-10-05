using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WindowsFormsApp1
{
    using System.Drawing;
    using GMap.NET.WindowsForms;
    using GMap.NET.WindowsForms.Markers;
    using GMap.NET;
    using System;
    using System.Runtime.Serialization;
    using System.Drawing.Drawing2D;

    [Serializable]
    public class GMapMarkerRect : GMapMarker, ISerializable
    {
        [NonSerialized]
        public Pen Pen;

        [NonSerialized]
        public GMarkerGoogle InnerMarker;

        public GMapMarkerRect(PointLatLng p)
           : base(p)
        {
            Pen = new Pen(Brushes.Blue, 5);

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(11, 11);
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2);
        }

        public override void OnRender(Graphics g)
        {
            g.DrawRectangle(Pen, new System.Drawing.Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height));
        }

        public override void Dispose()
        {
            if (Pen != null)
            {
                Pen.Dispose();
                Pen = null;
            }

            if (InnerMarker != null)
            {
                InnerMarker.Dispose();
                InnerMarker = null;
            }

            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected GMapMarkerRect(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
        }

        #endregion
    }
}