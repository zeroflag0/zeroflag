#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2007  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

namespace zeroflag.Windows
{
    using System;

    public enum HitTest
    {
        HTBORDER = 0x12,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 0x10,
        HTBOTTOMRIGHT = 0x11,
        HTCAPTION = 2,
        HTCLIENT = 1,
        HTCLOSE = 20,
        HTERROR = -2,
        HTGROWBOX = 4,
        HTHELP = 0x15,
        HTHSCROLL = 6,
        HTLEFT = 10,
        HTMAXBUTTON = 9,
        HTMENU = 5,
        HTMINBUTTON = 8,
        HTNOWHERE = 0,
        HTOBJECT = 0x13,
        HTREDUCE = 8,
        HTRIGHT = 11,
        HTSIZE = 4,
        HTSIZEFIRST = 10,
        HTSIZELAST = 0x11,
        HTSYSMENU = 3,
        HTTOP = 12,
        HTTOPLEFT = 13,
        HTTOPRIGHT = 14,
        HTTRANSPARENT = -1,
        HTVSCROLL = 7,
        HTZOOM = 9
    }
}
