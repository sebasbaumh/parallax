/*
 * Copyright 2012 Alex Usachev, thothbot@gmail.com
 * 
 * This file is part of Parallax project.
 * 
 * Parallax is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License as published by the 
 * Free Software Foundation, either version 3 of the License, or (at your 
 * option) any later version.
 * 
 * Parallax is distributed in the hope that it will be useful, but 
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
 * or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
 * for more details.
 * 
 * You should have received a copy of the GNU General Public License along with 
 * Parallax. If not, see http://www.gnu.org/licenses/.
 */

package thothbot.parallax.loader.shared.collada.dae;

import thothbot.parallax.core.shared.Log;

import com.google.gwt.xml.client.Node;
import com.google.gwt.xml.client.NodeList;

public class DaeColorOrTexture extends DaeElement 
{

	private boolean _isTexture;
	private float[] color;

	public DaeColorOrTexture(Node node) 
	{
		super(node);
	}

	@Override
	public void read() 
	{
		super.read();

		NodeList list = getNode().getChildNodes();
		for (int i = 0; i < list.getLength(); i++) 
		{
			Node child = list.item(i);
			String nodeName = child.getNodeName();

			if (nodeName.compareTo("color") == 0) 
			{
				Log.info(getNode().getNodeName() + " color: " + readFloatArray(child));
			} 
			else if (nodeName.compareTo("float") == 0) 
			{
				Log.info(getNode().getNodeName() +" float: " + readFloatArray(child)[0]);
			}
		}
	}

	public boolean isTexture() {
		return _isTexture;
	}
}