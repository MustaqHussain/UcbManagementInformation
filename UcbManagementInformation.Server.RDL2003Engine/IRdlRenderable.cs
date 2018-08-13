using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UcbManagementInformation.Server.RDL2003Engine
{
    public interface IRdlRenderable
    {
        void Render2010(XmlWriter xmlWriter);
    }
}
