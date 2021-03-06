using UnityEngine;
using System;


public class UIToggleButton : UITouchableSprite
{
	public delegate void UIToggleButtonChanged( UIToggleButton sender, bool selected );
	public event UIToggleButtonChanged onToggle; // event for when we get a touchUpInside
	
	private UIUVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UIUVRect highlightedUVframe;
	public UIUVRect selectedUVframe;
	
	private bool _selected;
	

	#region Constructors
	
	public static UIToggleButton create( string filename, string selectedFilename, string highlightedFilename, int xPos, int yPos )
	{
		return create( UI.firstToolkit, filename, selectedFilename, highlightedFilename, xPos, yPos );
	}
	
	
	public static UIToggleButton create( UIToolkit manager, string filename, string selectedFilename, string highlightedFilename, int xPos, int yPos )
	{
		var textureInfo = manager.textureInfoForFilename( filename );
		var frame = new Rect( xPos, yPos, textureInfo.size.x, textureInfo.size.y );
		
		var selectedTI = manager.textureInfoForFilename( selectedFilename );
		var highlightedTI = manager.textureInfoForFilename( highlightedFilename );
		
		return new UIToggleButton( manager, frame, 1, textureInfo.uvRect, selectedTI.uvRect, highlightedTI.uvRect );
	}
	
	
	public UIToggleButton( UIToolkit manager, Rect frame, int depth, UIUVRect uvFrame, UIUVRect selectedUVframe, UIUVRect highlightedUVframe ):base( frame, depth, uvFrame )
	{
		this.selectedUVframe = selectedUVframe;
		
		// Save a copy of our uvFrame here so that when highlighting turns off we have the original UVs
		_normalUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UIUVRect.zero )
			highlightedUVframe = uvFrame;
		
		manager.addTouchableSprite( this );
	}
	
	#endregion;
	

	// Sets the uvFrame of the original GUISprite and resets the _normalUVFrame for reference when highlighting
	public override UIUVRect uvFrame
	{
		get { return _uvFrame; }
		set
		{
			_uvFrame = value;
			_normalUVframe = value;
			manager.updateUV( this );
		}
	}

	
	public override bool highlighted
	{
		set
		{
			// Only set if it is different than our current value
			if( _highlighted != value )
			{			
				_highlighted = value;
				
				if ( value )
					base.uvFrame = highlightedUVframe;
				else if( _selected )
					base.uvFrame = selectedUVframe;
				else
					base.uvFrame = _normalUVframe;
			}
		}
	}


	public bool selected
	{
		get { return _selected; }
		set
		{
			// Only set if it is different than our current value
			if( _selected != value )
			{			
				_selected = value;
				
				if ( value )
					base.uvFrame = selectedUVframe;
				else
					base.uvFrame = _normalUVframe;
			}
		}
	}


	// Touch handlers
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	public override void onTouchEnded( UIFakeTouch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#else
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
#endif
	{
		highlighted = false;
		
		// If the touch was inside our touchFrame and we have an action, call it
		if( touchWasInsideTouchFrame )
		{
			// Toggle our selected state
			this.selected = !_selected;
			
			// Let our delegate know things changed
			if( onToggle != null )
				onToggle( this, _selected );
		}
	}

}

