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
        if (currentKeyboardState.IsKeyDown(Keys.F11) &&
            previousKeyboardState.IsKeyUp(Keys.F11)) // check if the F button was pressed for a single frame
        {
            isFullScreen = !isFullScreen;
            graphics.IsFullScreen = isFullScreen;
            graphics.ApplyChanges();
            
            Console.WriteLine("pressed the F key");
            Console.WriteLine( graphics.IsFullScreen);
            
        }
        previousKeyboardState = Keyboard.GetState();
    }

   
}