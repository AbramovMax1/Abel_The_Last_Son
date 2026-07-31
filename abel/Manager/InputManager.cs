using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Abel_The_Last_Son.Manager;

public class InputManager : IUpdateable 
{
    
    
    // ==========
    // Keyboard states
    private KeyboardState previousKeyboardState; // accuses previous Keyboard state to compare a single frame tap
    private KeyboardState currentKeyboardState; // accuses current Keyboard state to compare a single frame tap
    
    // ==========
    // flags and bool's
    private bool isFullScreen; // a flag to set fullScreen
    
    
    public void Start()
    {
        
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public void FullscreenFlip(GraphicsDeviceManager graphics)
    {
        // ======== F button ======== (fullScreen control)  
        currentKeyboardState = Keyboard.GetState();
        if (currentKeyboardState.IsKeyDown(Keys.F) &&
            previousKeyboardState.IsKeyUp(Keys.F)) // check if the F button was pressed for a single frame
        {
            if (isFullScreen) graphics.IsFullScreen = false; 
            if (!isFullScreen) graphics.IsFullScreen = true; 
            Console.WriteLine("pressed the F key");
            Console.WriteLine( graphics.IsFullScreen);
            graphics.ApplyChanges();
        }
        previousKeyboardState = Keyboard.GetState();
    }

   
}