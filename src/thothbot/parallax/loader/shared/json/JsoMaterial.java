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

package thothbot.parallax.loader.shared.json;

import java.util.List;

public interface JsoMaterial 
{
	List<Double> getColorAmbient();
	
	List<Double> getColorDiffuse();
	
	List<Double> getColorSpecular();
	
	double getIllumination();
	
	double getOpticalDensity();
	
	boolean getDepthTest();
	
	boolean getDepthWrite();

	String getMapLight();
	
	String getMapBump();
	
	String getMapSpecular();
	
	List<Integer> getMapBumpRepeat();
	
	List<Integer> getMapSpecularRepeat();
	
	List<String> getMapBumpWrap();
	
	List<String> getMapSpecularWrap();
	
	double getMapBumpScale();
	
	int getMapBumpAnisotropy();
	
	int getMapSpecularAnisotropy();
	
	int getMapLightAnisotropy();
	
	String getShading();
	
	double getSpecularCoef();
	
	double getTransparency();
	
	boolean getTransparent();
	
	boolean getVertexColors();
}
